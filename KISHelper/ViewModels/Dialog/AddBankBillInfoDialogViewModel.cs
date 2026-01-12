using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.ViewModels.Dialog
{
    public class AddBankBillInfoDialogViewModel : DialogViewModelBase
    {
        #region 参数
        private ObservableCollection<AccDimension>? bankDimension;
        public ObservableCollection<AccDimension>? BankDimension
        {
            get => bankDimension;
            set => SetField(ref bankDimension, value);
        }

        private AccDimension? outBankSelected;
        public AccDimension? OutBankSelected
        {
            get => outBankSelected;
            set => SetField(ref outBankSelected, value);
        }

        private AccDimension? inBankSelected;
        public AccDimension? InBankSelected
        {
            get => inBankSelected;
            set => SetField(ref inBankSelected, value);
        }

        private double aMount;

        public double Amount
        {
            get => aMount;
            set => SetField(ref aMount, value);
        }


        #endregion
        #region 命令
        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }

        #endregion
        private readonly DataRepository _repository = new();
        public AddBankBillInfoDialogViewModel()
        {

            OKCommand = new RelayCommand(OnOK);

            CancelCommand = new RelayCommand(OnCancel);

            var tmpDimension = new ObservableCollection<AccDimension>(
                _repository.LoadData<AccDimension>("AccDimension"));

            var filteredList = tmpDimension.Where(d => d.DimensionType == "银行账号").ToList();

            bankDimension = new ObservableCollection<AccDimension>(filteredList);

            outBankSelected = bankDimension.FirstOrDefault();

            inBankSelected = bankDimension.FirstOrDefault();
        }

        #region 过程

        private void OnOK()
        {
            if(Validate())
            CloseDialog(true);
        }
        private void OnCancel()
        {
            CloseDialog(false);
        }

        private bool Validate()
        {
            if (Amount == 0)
            {
                MessageBox.Show("金额不能为零！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        #endregion
    }
}
