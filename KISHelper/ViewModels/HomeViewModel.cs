using KISHelper.Common;
using KISHelper.Services;
using KISHelper.ViewModels.Dialog;
using KISHelper.Views;
using KISHelper.Views.Dialog;
using MathNet.Numerics.RootFinding;
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

        private AccountBook? bookSelected;
        public AccountBook? BookSelected 
        {
            get => bookSelected;
            set 
            {
                SetField(ref bookSelected, value);
                RefreshOther();
            }
        }
        private VoucherGroup? voucherSelected;
        public VoucherGroup? VoucherSelected 
        {
            get => voucherSelected;
            set => SetField(ref voucherSelected, value);
        }
        private AccDimension? bankSelected;
        public AccDimension? BankSelected 
        {
            get => bankSelected;
            set => SetField(ref bankSelected, value);
        }
        public DateTime AccDate { get; set; } = DateTime.Today;

        //数据源
        public ObservableCollection<AccountBook>? AccountBooks { get; set; }
        public ObservableCollection<VoucherGroup>? VoucherGroups { get; set; }
        public ObservableCollection<AccRule>? AccRules { get; set; }


        private ObservableCollection<AccDimension>? _bankList;
        public ObservableCollection<AccDimension>? BankList
        {
            get => _bankList;
            set => SetField(ref _bankList, value);
        }
        public ObservableCollection<BillInfo>? AllBills { get; set; } = new();    //源数据

        //DataGrid绑定的数据源
        public ObservableCollection<BillInfo>? LeftBills { get; set; } = new();    // 左边DataGrid
        public ObservableCollection<BillInfo>? RightBills { get; set; } = new();   // 右边DataGrid


        //搜索框文本
        public string? quickSearchText = string.Empty;
        public string? QuickSearchText 
        { 
            get => quickSearchText;
            set=> SetField(ref quickSearchText, value);
        } 
        //左边的合计金额
        private double leftTotalAmount;
        public double LeftTotalAmount
        {
            get => leftTotalAmount;
            set 
            {
                SetField(ref leftTotalAmount, value);
            }
        }

        private double bankTotalAmount;
        public double BankTotalAmount
        {
            get => bankTotalAmount;
            set
            {
                SetField(ref bankTotalAmount, value);
            }
        }

        private double debitTotalAmount;
        public double DEBITTotalAmount
        {
            get => debitTotalAmount;
            set
            {
                SetField(ref debitTotalAmount, value);
            }
        }

        private double creditTotalAmount;
        public double CREDITTotalAmount
        {
            get => creditTotalAmount;
            set
            {
                SetField(ref creditTotalAmount, value);
            }
        }

        private bool isImporting;
        public bool IsImporting
        {
            get => isImporting;
            set
            {
                SetField(ref isImporting, value);
            }
        }

        private BillInfo? selectedBillInfo;
        public BillInfo? SelectedBillInfo
        {
            get => selectedBillInfo;
            set
            {
                SetField(ref selectedBillInfo, value);
            }
        }

        private bool isExporting;
        public bool IsExporting
        {
            get => isExporting;
            set
            {
                SetField(ref isExporting, value);
            }
        }

        #endregion


        #region 命令
        public RelayCommand ImportExcelCommand { get; }
        public RelayCommand FilterCommand { get; }
        public RelayCommand AddBillCommand { get; }
        public RelayCommand EditBillCommand { get; }
        public RelayCommand SaveBillCommand { get; }
        public RelayCommand ExportBillCommand { get; }
        public RelayCommand AddBankBillCommand { get; }
        public RelayCommand MoveSelectedToRightCommand { get; }
        public RelayCommand MoveSelectedToLeftCommand { get; }
        public RelayCommand<BillInfo> MoveItemToRightCommand { get; }
        public RelayCommand<BillInfo> MoveItemToLeftCommand { get; }
        public RelayCommand MergeDataCommand { get; }

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
            AddBillCommand = new RelayCommand(ExecuteAddBill);
            SaveBillCommand = new RelayCommand(OnSave);
            ExportBillCommand = new RelayCommand(OnExport);
            AddBankBillCommand = new RelayCommand(AddBankBillInfo);
            EditBillCommand = new RelayCommand(ExecuteEditBill);
            MergeDataCommand = new RelayCommand(OpenMergeData);

            // 初始化移动命令
            MoveSelectedToRightCommand = new RelayCommand(() => MoveSelectedItems(LeftBills, RightBills));
            MoveSelectedToLeftCommand = new RelayCommand(() => MoveSelectedItems(RightBills, LeftBills));
            MoveItemToRightCommand = new RelayCommand<BillInfo>(item => MoveSingleItem(item, LeftBills, RightBills));
            MoveItemToLeftCommand = new RelayCommand<BillInfo>(item => MoveSingleItem(item, RightBills, LeftBills));

            RefreshData();
            ApplyFilter(); // 初始加载
        }

        #region 查询逻辑
        public void RefreshData()
        {
            try
            {
                var tempBook = _repository.LoadData<AccountBook>("AccountBooks").ToList();
                var _accountBooks = new ObservableCollection<AccountBook>(tempBook);
                AccountBooks = _accountBooks;

                var tempvoucher = _repository.LoadData<VoucherGroup>("VoucherGroups").ToList();
                var _voucherGroups = new ObservableCollection<VoucherGroup>(tempvoucher);
                VoucherGroups = _voucherGroups;
                
                // 恢复初始值
                BookSelected = AccountBooks.FirstOrDefault();
                VoucherSelected = VoucherGroups.FirstOrDefault();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据刷新失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshOther()
        {
            var temprules = _repository.LoadData<AccRule>("AccRules").Where(p => p.Affiliated==BookSelected?.Name).ToList();
            var _accRules = new ObservableCollection<AccRule>(temprules);
            AccRules = _accRules;

            var tempdimensions = _repository.LoadData<AccDimension>("AccDimension").Where(p => p.Affiliated == BookSelected?.Name&&p.DimensionType=="银行账号").ToList();
            var _dimensions = new ObservableCollection<AccDimension> (tempdimensions);
            var _bankdimensions = new ObservableCollection<AccDimension>(_dimensions);
            BankList = _bankdimensions;
            BankSelected = BankList.FirstOrDefault();
        }


        public void ApplyFilter()
        {
            var sw = Stopwatch.StartNew();
            // 清空旧筛选结果
            if(LeftBills==null || RightBills == null) { return; }
            foreach(var bill in LeftBills)
            {
                bill.PropertyChanged -= OnLeftBillPropertyChanged;
            }
            LeftBills.Clear();

            // 准备搜索词
            _searchTerms = string.IsNullOrWhiteSpace(QuickSearchText)
                ? new List<string>()
                : QuickSearchText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(s => s.Trim().ToLower())
                                 .ToList();

            // 筛选并填充（排除已在右侧的数据）
            foreach (var bill in AllBills.Where(b => !RightBills.Contains(b)))
            {
                if (ShouldIncludeInFilter(bill))
                {
                    LeftBills.Add(bill);
                    bill.PropertyChanged += OnLeftBillPropertyChanged;
                }
            }

            // 自动全选
            foreach (var bill in LeftBills)
            {
                bill.SetSelectedSilently(true);
            }

            CalculateTotal();

            sw.Stop();
            Debug.WriteLine($"筛选完成: {sw.ElapsedMilliseconds}ms, 结果数: {LeftBills.Count}");
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
            LeftTotalAmount = LeftBills.Where(b => b.IsSelected).Sum(b => b.AMOUNT);
            DEBITTotalAmount = RightBills.Where(b => b.IsSelected && b.BalanceDirection == "借方").Sum(b => b.AMOUNT);
            CREDITTotalAmount = RightBills.Where(b => b.IsSelected && b.BalanceDirection == "贷方").Sum(b => b.AMOUNT);
            BankTotalAmount = Math.Abs(DEBITTotalAmount - CREDITTotalAmount);
        }

        private void OnLeftBillPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BillInfo.IsSelected))
            {
                CalculateTotal();
            }
        }

        #endregion

        #region 移动数据逻辑

        private void MoveSelectedItems(ObservableCollection<BillInfo> source, ObservableCollection<BillInfo> target)
        {
            var selectedItems = source.Where(b => b.IsSelected).ToList();
            MoveItems(selectedItems, source, target);
        }

        private void MoveSingleItem(BillInfo item, ObservableCollection<BillInfo> source, ObservableCollection<BillInfo> target)
        {
            if (item != null && source.Contains(item))
            {
                MoveItems(new List<BillInfo> { item }, source, target);
            }
        }

        private void MoveItems(List<BillInfo> items, ObservableCollection<BillInfo> source, ObservableCollection<BillInfo> target)
        {
            if (!items.Any()) return;

            // 解除事件绑定
            foreach (var item in items)
            {
                item.PropertyChanged -= OnLeftBillPropertyChanged;
                source.Remove(item);
                target.Add(item);
                item.PropertyChanged += OnLeftBillPropertyChanged;
            }

            CalculateTotal();
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

            IsImporting = true;

            _filePath = Path.GetDirectoryName(dialog.FileName);

            try
            {
                var bills = await Task.Run(() =>
                    NpoiHelper.ReadFromExcel<BillInfo>(dialog.FileName, sheetName: "核销数据")
                );

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (AllBills == null) { return; }
                    AllBills.Clear();
                    Dictionary<string, string> AccDic = AccRules.Where(r => r.AccName != null).ToDictionary(r => r.AccName!, r => r.AccFDC ??"");
                    foreach (var bill in bills)
                    {
                        if (AccDic.TryGetValue(bill.AccType, out var AccFDC))
                        {
                            bill.BalanceDirection = AccFDC;
                        }
                        AllBills.Add(bill);
                    }
                    ApplyFilter(); // 重新筛选
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsImporting = false;
            }
        }
        private void OpenMergeData() 
        {
            var mergeDataView = new MergeDataView();
            mergeDataView.Show();
        }

        private void ExecuteAddBill()
        {
            var dialog = new AddBillDialog
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddBillDialogViewModel{ AccountBook = BookSelected }
            };
            var vm = dialog.DataContext as AddBillDialogViewModel;
            if (dialog.ShowDialog() == true && vm!=null && vm.BillInfo!=null)
            {
                vm.BillInfo.IsSelected = true;
                vm.BillInfo.IsTemporary = true;
                vm.BillInfo.PropertyChanged += OnLeftBillPropertyChanged;

                RightBills!.Add(vm.BillInfo);
                CalculateTotal();
            }
        }

        private void ExecuteEditBill()
        {
            if (selectedBillInfo == null) return;
            var clone = selectedBillInfo.DeepClone();
            var dialog = new AddBillDialog
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddBillDialogViewModel { AccountBook = BookSelected }
            };
            var vm = dialog.DataContext as AddBillDialogViewModel;
            if (vm != null)
            {
                vm.BillInfo = clone;
            }
            if (dialog.ShowDialog() == true && vm != null && vm.BillInfo != null)
            {
                var idx = RightBills!.IndexOf(selectedBillInfo);
                RightBills[idx] = vm.BillInfo;
                CalculateTotal();
            }
        }

        private void AddBankBillInfo()
        {
            var dialog = new AddBankBillInfoDialog
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddBankBillInfoDialogViewModel()
            };
            var vm = dialog.DataContext as AddBankBillInfoDialogViewModel;
            if (dialog.ShowDialog() == true && vm != null)
            {
                BillInfo outbillinfo = new BillInfo();
                if (vm.OutBankSelected != null)
                {
                    outbillinfo.AccType = vm.OutBankSelected.AccName;
                    outbillinfo.DetailID_FFlex5 = vm.OutBankSelected.Branch;
                    outbillinfo.AMOUNT = vm.Amount;
                    outbillinfo.DetailID_FF100009 = vm.OutBankSelected.DimensionName;
                    outbillinfo.BillWayNumber = "资金调拨";
                    outbillinfo.BalanceDirection = "贷方";
                    outbillinfo.IsBankBillInfo = true;
                }

                VoucherInfo info = new VoucherInfo { Bank = vm.InBankSelected, Book = BookSelected, Voucher = VoucherSelected, Date = AccDate };
                var summary = new ObservableCollection<BillInfo>
                {
                    outbillinfo
                };
                ConvertKIS.AddToKIS(summary, info);

                if (ConvertKIS.IsFinished)
                {
                    SaveBills.Clear();
                    ConvertKIS.VoucherIndex++;
                    foreach (var item in summary) SaveBills.Add(item);
                }
            }
        }

        private ConvertKIS ConvertKIS = new();
        private void OnSave()
        {
            var summary = RightBills
                .Where(b => b.IsSelected)
                .GroupBy(b => new { b.AccType, b.AccNumber, b.DetailID_FFlex6, b.DetailID_FFlex5,b.DetailID_FFlex9, b.BalanceDirection,b.FEXPLANATION})
                .Select(g => new BillInfo
                {
                    AccType = g.Key.AccType,
                    AccNumber = g.Key.AccNumber,
                    DetailID_FFlex6 = g.Key.DetailID_FFlex6,
                    DetailID_FFlex5 = g.Key.DetailID_FFlex5,
                    DetailID_FFlex9=g.Key.DetailID_FFlex9,
                    BalanceDirection=g.Key.BalanceDirection,
                    FEXPLANATION=g.Key.FEXPLANATION,
                    AMOUNT = g.Sum(b => b.AMOUNT)
                })
                .ToList();
            VoucherInfo info = new VoucherInfo { Bank = BankSelected, Book = BookSelected, Voucher = VoucherSelected, Date = AccDate };
            ConvertKIS.AddToKIS(new ObservableCollection<BillInfo>(summary), info);

            if (ConvertKIS.IsFinished)
            {
                QuickSearchText = string.Empty;
                SaveBills.Clear();
                RightBills.Clear();
                ConvertKIS.VoucherIndex++;
                
                foreach (var item in summary) SaveBills.Add(item);
                ApplyFilter();
            }
            QuickSearchText = string.Empty;
        }

        private async void OnExport()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                var dialog = new FolderBrowserDialog();

                if (dialog.ShowDialog() == true)
                {
                    _filePath = dialog.SelectedPath;
                }
            }

            IsExporting = true;
            try
            {
                // 准备数据
                var data = ConvertKIS.Entities.ToList();
                var fullPath = Path.Combine(_filePath, DateTime.Now.ToString("yyyyMMddhhmmss")+"引入数据.xlsx");

                //耗时写文件丢后台
                bool ok = await Task.Run(() =>
                {
                    NpoiHelper.WriteToExcel(data, fullPath, "凭证#单据头(FBillHead)");
                    return NpoiHelper.IsExported;
                });

                // 回到 UI 线程清列表、弹窗
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ok)
                    {
                        SaveBills.Clear();
                        ConvertKIS.Clear();
                        MessageBox.Show($"已导出文件：{fullPath}");
                    }
                });
            }
            finally
            {
                IsExporting = false;
            }
        }

        #endregion
    }
}