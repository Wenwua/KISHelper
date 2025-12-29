using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace KISHelper.ViewModels.Dialog
{
    public class AddBillDialogViewModel : DialogViewModelBase
    {
        #region 必要的属性
        private BillInfo? billInfo =new();
        public BillInfo? BillInfo
        {
            get => billInfo;
            set => SetField(ref billInfo, value);
        }

        private ObservableCollection<AccRule>? accRules;
        public ObservableCollection<AccRule>? AccRules
        {
            get => accRules;
            set => SetField(ref accRules, value);
        }

        private ObservableCollection<AccDimension>? accDimension;
        public ObservableCollection<AccDimension>? AccDimension
        {
            get => accDimension;
            set => SetField(ref accDimension, value);
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

        private ObservableCollection<AccDimension>? costItemDimension;
        public ObservableCollection<AccDimension>? CostItemDimension
        {
            get => costItemDimension;
            set => SetField(ref costItemDimension, value);
        }

        private bool allowDetailID_FFlex5;
        public bool AllowDetailID_FFlex5
        {
            get => allowDetailID_FFlex5;
            set 
            { 
                SetField(ref allowDetailID_FFlex5, value);
            }
        }

        private bool allowDetailID_FFlex6;
        public bool AllowDetailID_FFlex6
        {
            get => allowDetailID_FFlex6;
            set => SetField(ref allowDetailID_FFlex6, value);
        }

        private bool allowDetailID_FFlex9;
        public bool AllowDetailID_FFlex9
        {
            get => allowDetailID_FFlex9;
            set => SetField(ref allowDetailID_FFlex9, value);
        }

        private AccRule? accRuleSelected;
        public AccRule? AccRuleSelected
        {
            get => accRuleSelected;
            set
            {
                SetField(ref accRuleSelected, value);
                if (BillInfo != null)
                    BillInfo.AccType = value?.AccName;
                if (value!=null && value.AccFiexItem!= null && value.AccFiexItem.Contains("部门"))
                {
                    AllowDetailID_FFlex5 = true;
                    var result = DpmDimension.FirstOrDefault(a => a.DimensionName == value.DefaultDetailID_FFlex5);
                    DpmSelected = result;
                    
                }
                AllowDetailID_FFlex6 = value?.AccFiexItem?.Contains("客户") ?? false;
                AllowDetailID_FFlex9 = value?.AccFiexItem?.Contains("费用项目") ?? false;
            }
        }

        private AccDimension? dpmSelected;
        public AccDimension? DpmSelected
        {
            get => dpmSelected;
            set 
            { 
                SetField(ref dpmSelected, value);
                if(BillInfo!= null)
                BillInfo.DetailID_FFlex5 = value?.DimensionName;
            }
        }

        private AccDimension? customSelected;
        public AccDimension? CustomSelected
        {
            get => customSelected;
            set 
            { 
                SetField(ref customSelected, value);
                if (BillInfo != null)
                    BillInfo.DetailID_FFlex6 = value?.DimensionName;
            }
        }

        private AccDimension? costItemSelected;
        public AccDimension? CostItemSelected
        {
            get => costItemSelected;
            set
            {
                SetField(ref costItemSelected, value);
                if (BillInfo != null)
                    BillInfo.DetailID_FFlex9 = value?.DimensionName;
            }
        }

        #endregion

        private readonly DataRepository _repository = new();


        public AddBillDialogViewModel()
        {
            OKCommand = new RelayCommand(OnOK);

            CancelCommand = new RelayCommand(OnCancel);

            AccRules = new ObservableCollection<AccRule>(
                _repository.LoadData<AccRule>("AccRules"));

            AccDimension = new ObservableCollection<AccDimension>(
                _repository.LoadData<AccDimension>("AccDimension"));

            var filteredList = AccDimension.Where(d => d.DimensionType == "部门").ToList();

            DpmDimension = new ObservableCollection<AccDimension>(filteredList);

            filteredList = AccDimension.Where(d => d.DimensionType == "客户").ToList();

            CustomDimension = new ObservableCollection<AccDimension>(filteredList);

            filteredList = AccDimension.Where(d => d.DimensionType == "费用项目").ToList();

            CostItemDimension = new ObservableCollection<AccDimension>(filteredList);
        }

        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void OnOK()
        {
            if (BillInfo == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(BillInfo.AccType))
            {
                MessageBox.Show("核算项目不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (BillInfo.AMOUNT==0)
            {
                MessageBox.Show("金额不能为零！！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CloseDialog(true);

        }
        private void OnCancel()
        {
            CloseDialog(false);
        }
    }
}
