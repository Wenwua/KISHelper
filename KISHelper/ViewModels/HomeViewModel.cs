using KISHelper.Common;
using KISHelper.Services;
using KISHelper.ViewModels.Dialog;
using KISHelper.Views.Dialog;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using static KISHelper.Common.Npoi;

namespace KISHelper.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        #region 必要的属性

        public AccountBook? BookSelected { get; set; }
        public VoucherGroup? VoucherSelected { get; set; }
        public AccDimension? BankSelected { get; set; }
        public DateTime AccDate { get; set; } = DateTime.Today;

        //数据源
        public ObservableCollection<AccountBook>? AccountBooks { get; set; } = new();
        public ObservableCollection<VoucherGroup>? VoucherGroups { get; set; } = new();
        public ObservableCollection<AccRule>? AccRules { get; set; } = new();
        public ObservableCollection<AccDimension>? AccDimension { get; set; } = new();
        public ObservableCollection<AccDimension>? BankList { get; set; } = new();
        public ObservableCollection<BillInfo>? AllBills { get; set; } = new();    //源数据

        //DataGrid绑定的数据源
        public ObservableCollection<BillInfo>? FilteredBills { get; set; } = new();    //源数据


        //搜索框文本
        public string? quickSearchText = string.Empty;
        public string? QuickSearchText 
        { 
            get => quickSearchText;
            set=> SetField(ref quickSearchText, value);
        } 
        //合计金额
        private double totalAmount;
        public double TotalAmount 
        {
            get => totalAmount;
            set 
            {
                SetField(ref totalAmount, value);
            }
        }

        #endregion


        #region 命令
        public RelayCommand ImportExcelCommand { get; }
        public RelayCommand FilterCommand { get; }
        public RelayCommand AddBillCommand { get; }
        public RelayCommand SaveBillCommand { get; }
        public RelayCommand ExportBillCommand { get; }
        #endregion

        // 导出相关
        public ObservableCollection<BillInfo> SaveBills { get; } = new();

        private readonly DataRepository _repository = new();

        private string? _filePath;

        private List<string> _searchTerms = new();

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

        #region 查询逻辑
        public void RefreshData()
        {
            try
            {
                AccountBooks.Clear();
                AccountBooks=new ObservableCollection<AccountBook>(_repository.LoadData<AccountBook>("AccountBooks"));
                VoucherGroups.Clear();
                VoucherGroups = new ObservableCollection<VoucherGroup>(_repository.LoadData<VoucherGroup>("VoucherGroups"));
                AccRules.Clear();
                AccRules = new ObservableCollection<AccRule>(_repository.LoadData<AccRule>("AccRules"));
                AccDimension.Clear();
                AccDimension = new ObservableCollection<AccDimension>(_repository.LoadData<AccDimension>("AccDimension"));
                // 筛选银行账号
                BankList.Clear();
                BankList.Add(AccDimension.FirstOrDefault(a => a.DimensionType == "银行账号"));
                // 恢复初始值
                BookSelected = AccountBooks.FirstOrDefault();
                VoucherSelected = VoucherGroups.FirstOrDefault();
                BankSelected = BankList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据刷新失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        public void ApplyFilter()
        {
            var sw = Stopwatch.StartNew();
            // 清空旧筛选结果
            foreach(var bill in FilteredBills)
            {
                bill.PropertyChanged -= OnBillPropertyChanged;
            }
            FilteredBills.Clear();

            // 准备搜索词
            _searchTerms = string.IsNullOrWhiteSpace(QuickSearchText)
                ? new List<string>()
                : QuickSearchText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(s => s.Trim().ToLower())
                                 .ToList();

            // 筛选并填充（单次遍历）
            foreach (var bill in AllBills)
            {
                if (ShouldIncludeInFilter(bill))
                {
                    FilteredBills.Add(bill);
                    bill.PropertyChanged += OnBillPropertyChanged;
                }
            }

            // 自动全选
            foreach (var bill in FilteredBills)
            {
                bill.SetSelectedSilently(true);
            }

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

        // 计算合计
        public void CalculateTotal()
        {
            TotalAmount = FilteredBills.Where(b => b.IsSelected).Sum(b => b.AMOUNT);
        }

        private void OnBillPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BillInfo.IsSelected))
            {
                CalculateTotal();
            }
        }

        #endregion

        #region 按钮命令
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
                    AllBills.Clear();
                    foreach (var bill in bills)
                    {
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

        private void ShowAddBillDialog()
        {
            var dialog = new AddBillDialog 
            { 
                Owner = Application.Current.MainWindow,
                DataContext = new AddBillDialogViewModel()
            };
            var vm = dialog.DataContext as AddBillDialogViewModel;
            if (dialog.ShowDialog() == true && vm!=null && vm.BillInfo!=null)
            {
                vm.BillInfo.IsSelected = true;
                vm.BillInfo.IsTemporary = true;
                vm.BillInfo.PropertyChanged += OnBillPropertyChanged;
                FilteredBills.Add(vm.BillInfo);
                CalculateTotal();
            }
        }

        private ConvertKIS ConvertKIS = new();
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
            VoucherInfo info = new VoucherInfo { Bank = BankSelected, Book = BookSelected, Voucher = VoucherSelected, Date = AccDate };
            ConvertKIS.AddToKIS(new ObservableCollection<BillInfo>(summary), info);

            if (ConvertKIS.IsFinished)
            {
                QuickSearchText = string.Empty;
                SaveBills.Clear();
                ConvertKIS.VoucherIndex++;
                foreach (var item in summary) SaveBills.Add(item);
                ApplyFilter();
            }
            QuickSearchText = string.Empty;
        }

        private void OnExport()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                MessageBox.Show("请先导入Excel文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NpoiHelper.WriteToExcel(ConvertKIS.Entities.ToList(),
                Path.Combine(_filePath, "引入数据.xlsx"),
                "凭证#单据头(FBillHead)");

            if (NpoiHelper.IsExported) 
            {
                SaveBills.Clear();
                ConvertKIS.Clear();
                MessageBox.Show($"已导出文件：{_filePath}\\引入数据.xlsx");
            }
            
        }

        #endregion
    }
}