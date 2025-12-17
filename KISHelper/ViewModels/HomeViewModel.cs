using KISHelper.Common;
using KISHelper.Services;
using KISHelper.Views.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using static KISHelper.Common.Npoi;

namespace KISHelper.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly DataRepository _repository = new();

        // 数据源（保持不变）
        public ObservableCollection<AccountBook> AccountBooks { get; } = new();
        public ObservableCollection<VoucherGroup> VoucherGroups { get; } = new();
        public ObservableCollection<AccRule> AccRules { get; } = new();
        public ObservableCollection<AccDimension> AccDimension { get; } = new();
        public ObservableCollection<AccDimension> BankList { get; } = new();

        // ✅ 核心数据集合（只保留这两个）
        public ObservableCollection<BillInfo> AllBills { get; } = new();          // 源数据
        public ObservableCollection<BillInfo> FilteredBills { get; } = new();     //  DataGrid绑定目标

        // 配置项
        public string? BookId { get; set; }
        public string? VoucherId { get; set; }
        public string? BankId { get; set; }
        public DateTime AccDate { get; set; } = DateTime.Today;

        // 命令
        public RelayCommand ImportExcelCommand { get; }
        public RelayCommand FilterCommand { get; }
        public RelayCommand AddBillCommand { get; }
        public RelayCommand SaveBillCommand { get; }
        public RelayCommand ExportBillCommand { get; }

        // 快速搜索文本
        private string _quickSearchText;
        public string QuickSearchText
        {
            get => _quickSearchText;
            set
            {
                SetField(ref _quickSearchText, value);
                ApplyFilter(); // 修改搜索时触发筛选（会清除临时数据）
            }
        }

        // 合计金额
        private double _totalAmount;
        public double TotalAmount
        {
            get => _totalAmount;
            set => SetField(ref _totalAmount, value);
        }

        // 全选状态
        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (SetField(ref _isAllSelected, value))
                {
                    // ✅ 直接操作 FilteredBills，无事件风暴
                    foreach (var bill in FilteredBills)
                    {
                        bill.SetSelectedSilently(value);
                    }
                    CalculateTotal();
                    UpdateIsAllSelectedState();
                }
            }
        }

        // 导出相关
        public ObservableCollection<Entity> ExportEntity { get; } = new();
        public ObservableCollection<BillInfo> SaveBills { get; } = new();
        private string? _filePath;
        private int _count = 1;

        public HomeViewModel()
        {
            ImportExcelCommand = new RelayCommand(async () => await ImportExcelAsync());
            FilterCommand = new RelayCommand(ApplyFilter);
            AddBillCommand = new RelayCommand(ShowAddBillDialog);
            SaveBillCommand = new RelayCommand(OnSave);
            ExportBillCommand = new RelayCommand(OnExport);

            RefreshData();
            ApplyFilter(); // 初始加载
        }

        public void RefreshData()
        {
            try
            {
                // 批量加载数据（减少通知）
                var books = _repository.LoadData<AccountBook>("AccountBooks");
                AccountBooks.Clear();
                foreach (var item in books) AccountBooks.Add(item);

                var vouchers = _repository.LoadData<VoucherGroup>("VoucherGroups");
                VoucherGroups.Clear();
                foreach (var item in vouchers) VoucherGroups.Add(item);

                var rules = _repository.LoadData<AccRule>("AccRules");
                AccRules.Clear();
                foreach (var item in rules) AccRules.Add(item);

                var dimensions = _repository.LoadData<AccDimension>("AccDimension");
                AccDimension.Clear();
                foreach (var item in dimensions) AccDimension.Add(item);

                // 筛选银行账号
                BankList.Clear();
                foreach (var item in dimensions.Where(d => d.DimensionType == "银行账号"))
                    BankList.Add(item);

                // 恢复初始值
                BookId = AccountBooks.FirstOrDefault()?.Id;
                VoucherId = VoucherGroups.FirstOrDefault()?.Id;
                BankId = BankList.FirstOrDefault()?.DimensionNumber;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据刷新失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 筛选逻辑（保留清除临时数据功能）
        private List<string> _searchTerms = new();

        public void ApplyFilter()
        {
            var sw = Stopwatch.StartNew();

                
            // 清空旧筛选结果
            FilteredBills.Clear();

            // 2. 准备搜索词
            _searchTerms = string.IsNullOrWhiteSpace(QuickSearchText)
                ? new List<string>()
                : QuickSearchText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(s => s.Trim().ToLower())
                                 .ToList();

            // 3. 筛选并填充（单次遍历）
            foreach (var bill in AllBills)
            {
                if (ShouldIncludeInFilter(bill))
                {
                    FilteredBills.Add(bill);
                }
            }

            // 4. 自动全选
            foreach (var bill in FilteredBills)
            {
                bill.SetSelectedSilently(true);
            }

            // 5. 更新状态
            UpdateIsAllSelectedState();
            CalculateTotal();

            sw.Stop();
            Debug.WriteLine($"筛选完成: {sw.ElapsedMilliseconds}ms, 结果数: {FilteredBills.Count}");
        }

        private bool ShouldIncludeInFilter(BillInfo bill)
        {
            if (bill.IsTemporary) return true; // 临时数据永远显示
            if (!_searchTerms.Any()) return true; // 无搜索词则全部显示

            return _searchTerms.Any(searchText =>
                bill.AccNumber?.ToLower().Contains(searchText) == true ||
                bill.BillWayNumber?.ToLower().Contains(searchText) == true ||
                bill.TRANSDATE?.ToLower().Contains(searchText) == true
            );
        }

        // ✅ 计算合计（极快）
        public void CalculateTotal()
        {
            TotalAmount = FilteredBills.Where(b => b.IsSelected).Sum(b => b.AMOUNT);
        }

        private void UpdateIsAllSelectedState()
        {
            var allSelected = FilteredBills.Any() && FilteredBills.All(b => b.IsSelected);
            SetField(ref _isAllSelected, allSelected);
        }

        // ✅ 异步导入Excel
        private async Task ImportExcelAsync()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel文件|*.xlsx;*.xls;*.xlsm|所有文件|*.*",
                Title = "选择要导入的Excel文件"
            };

            if (dialog.ShowDialog() != true) return;

            _filePath = Path.GetDirectoryName(dialog.FileName);

            try
            {
                var bills = await Task.Run(() =>
                    NpoiHelper.ReadFromExcel<BillInfo>(dialog.FileName, sheetName: "核销数据")
                );

                Application.Current.Dispatcher.Invoke(() =>
                {
                    // 清理旧订阅
                    foreach (var oldBill in AllBills)
                    {
                        oldBill.PropertyChanged -= OnBillPropertyChanged;
                    }

                    AllBills.Clear();

                    // 订阅并添加
                    foreach (var bill in bills)
                    {
                        bill.PropertyChanged += OnBillPropertyChanged;
                        AllBills.Add(bill);
                    }

                    ApplyFilter(); // 重新筛选
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnBillPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BillInfo.IsSelected))
            {
                CalculateTotal();
                UpdateIsAllSelectedState();
            }
        }

        private void ShowAddBillDialog()
        {
            var dialog = new AddBillDialog { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true && dialog.DataContext is AddBillDialog vm)
            {
                var newBill = new BillInfo
                {
                    AccType = vm.AccType,
                    DetailID_FFlex5 = vm.DetailID_FFlex5,
                    DetailID_FFlex6 = vm.DetailID_FFlex6,
                    AMOUNT = vm.AMOUNT,
                    IsSelected = true,
                    IsTemporary = true
                };

                newBill.PropertyChanged += OnBillPropertyChanged;


                FilteredBills.Add(newBill); // 立即显示在界面上

                CalculateTotal();
                UpdateIsAllSelectedState();
            }
        }

        private void OnSave()
        {
            var summary = FilteredBills
                .Where(b => b.IsSelected)
                .GroupBy(b => new { b.AccType, b.AccNumber, b.DetailID_FFlex6, b.DetailID_FFlex5 })
                .Select(g => new BillInfo
                {
                    AccType = g.Key.AccType,
                    AccNumber = g.Key.AccNumber,
                    DetailID_FFlex6 = g.Key.DetailID_FFlex6,
                    DetailID_FFlex5 = g.Key.DetailID_FFlex5,
                    AMOUNT = g.Sum(b => b.AMOUNT)
                })
                .ToList();

            bool isDone = false;
            Excel2KISHelper.ExcelToKIS(ExportEntity,
                new ObservableCollection<BillInfo>(summary),
                AccRules, AccDimension, BookId, VoucherId, BankId, AccDate,
                ExportEntity.Count, ref _count, ref isDone);

            if (isDone)
            {
                QuickSearchText = string.Empty;
                SaveBills.Clear();
                _count++;
                foreach (var item in summary) SaveBills.Add(item);
                ApplyFilter();
            }
        }

        private void OnExport()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                MessageBox.Show("请先导入Excel文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NpoiHelper.WriteToExcel(ExportEntity.ToList(),
                Path.Combine(_filePath, "引入数据.xlsx"),
                "凭证#单据头(FBillHead)");

            

            if (NpoiHelper.IsExported) 
            {
                SaveBills.Clear();
                ExportEntity.Clear();
                _count = 1;
                MessageBox.Show($"已导出文件：{_filePath}\\引入数据.xlsx");
            }
            
        }
    }
}