using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.ViewModels.Dialog
{
    public class DimensionDialogViewModel : DialogViewModelBase
    {
        #region 必需参数
        private AccDimension? accDimension;
        public AccDimension? AccDimension
        {
            get => accDimension;
            set 
            {
                if (accDimension != null) { accDimension.PropertyChanged -= OnDimensionPropertyChanged; }
                SetField(ref accDimension, value);
                if (accDimension != null) 
                { 
                    accDimension.PropertyChanged += OnDimensionPropertyChanged;
                    OnDimensionPropertyChanged(accDimension,new PropertyChangedEventArgs(nameof(AccDimension.DimensionType)));
                    if (DpmDimension != null)
                    {
                        dpmSelected = DpmDimension.FirstOrDefault(a => a.DimensionName == value?.Branch);
                    }
                    if (CustomDimension != null)
                    {
                        interiorSelected=CustomDimension.FirstOrDefault(a => a.DimensionName == value?.Interior);
                    }
                    if (AccountBooks != null)
                    {
                        AccBookSelected = AccountBooks.FirstOrDefault(a => a.Name == value?.Affiliated);
                    }
                    
                }
            }
        }
        //如果是银行类型，就需要给银行设置核算科目（根据卡号判断是现金还是银行存款还是其他货币资金）
        public bool IsAccName => AccDimension?.DimensionType == "银行账号";

        //如果是银行或部门，设置内部往来编码
        public bool IsInterior => AccDimension?.DimensionType == "部门";

        private ObservableCollection<AccRule>? accRules;
        public ObservableCollection<AccRule>? AccRules
        {
            get => accRules;
            set => SetField(ref accRules, value);
        }

        private ObservableCollection<AccDimension>? dpmDimension;
        public ObservableCollection<AccDimension>? DpmDimension
        {
            get => dpmDimension;
            set => SetField(ref dpmDimension, value);
        }

        private ObservableCollection<AccDimension>? customDimension;
        public ObservableCollection<AccDimension>? CustomDimension
        {
            get => customDimension;
            set => SetField(ref customDimension, value);
        }

        private AccRule? accRuleSelected;
        public AccRule? AccRuleSelected
        {
            get => accRuleSelected;
            set
            {
                SetField(ref accRuleSelected, value);
                if (accDimension != null)
                {
                    accDimension.AccName = value?.AccName;
                }

            }
        }

        private AccDimension? dpmSelected;
        public AccDimension? DpmSelected
        {
            get => dpmSelected;
            set
            {
                SetField(ref dpmSelected, value);
                if (accDimension != null)
                {
                    accDimension.Branch = value?.DimensionName;
                }
                
            }
        }

        private AccDimension? interiorSelected;
        public AccDimension? InteriorSelected
        {
            get => interiorSelected;
            set
            {
                SetField(ref interiorSelected, value);
                if (accDimension != null)
                {
                    accDimension.Interior = value?.DimensionName;
                }

            }
        }

        private ObservableCollection<AccountBook>? accountBooks;
        public ObservableCollection<AccountBook>? AccountBooks
        {
            get => accountBooks;
            set => SetField(ref accountBooks, value);
        }

        private AccountBook? accBookSelected;
        public AccountBook? AccBookSelected
        {
            get => accBookSelected;
            set
            {
                SetField(ref accBookSelected, value);
                if (accDimension != null)
                    accDimension.Affiliated = value?.Name;

            }
        }

        #endregion

        #region 命令
        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }

        #endregion


        private readonly DataRepository _repository = new();
        public DimensionDialogViewModel()
        {
            AccDimension = new AccDimension();

            OKCommand = new RelayCommand(OnOK);

            CancelCommand = new RelayCommand(OnCancel);

            var tmpDimension = new ObservableCollection<AccDimension>(
                _repository.LoadData<AccDimension>("AccDimension"));

            var filteredList = tmpDimension.Where(d => d.DimensionType == "部门").ToList();

            DpmDimension = new ObservableCollection<AccDimension>(filteredList);

            filteredList = tmpDimension.Where(d => d.DimensionType == "客户").ToList();

            CustomDimension = new ObservableCollection<AccDimension>(filteredList);

            AccRules = new ObservableCollection<AccRule>(
                _repository.LoadData<AccRule>("AccRules"));

            var _accbooks = new ObservableCollection<AccountBook>(
                _repository.LoadData<AccountBook>("AccountBooks"));

            accountBooks = new ObservableCollection<AccountBook>(_accbooks);

        }

        #region 事件判断
        private void OnDimensionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccDimension.DimensionType))
            {
                OnPropertyChanged(nameof(IsAccName));
                OnPropertyChanged(nameof(IsInterior));
            }
        }
        
        #endregion

        #region 过程

        private void OnOK()
        {
            if (Validate())
                CloseDialog(true);

           

        }
        private void OnCancel() => CloseDialog(false);

        private bool Validate()
        {
            if (AccDimension == null) return false;

            if (string.IsNullOrWhiteSpace(AccDimension.DimensionType))
            {
                MessageBox.Show("维度类型不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(AccDimension.DimensionName))
            {
                MessageBox.Show("维度名称不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(AccDimension.DimensionNumber))
            {
                MessageBox.Show("维度编码不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(AccDimension.AccName)&&AccDimension.DimensionType=="银行账号")
            {
                MessageBox.Show("银行账号需要设置核算规则！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        #endregion
    }
}
