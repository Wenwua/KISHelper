using KISHelper.Common;
using KISHelper.Views.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.ViewModels.Dialog
{
    public class AccBookDialogViewModel: DialogViewModelBase
    {
        #region 必需参数
        private string? id;
        public string? Id
        {
            get => id;
            set => SetField(ref id, value);
        }

        private string? name;
        public string? Name
        {
            get => name;
            set => SetField(ref name, value);
        }
        #endregion

        #region 命令
        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }

        #endregion

        public AccBookDialogViewModel()
        {
            OKCommand = new RelayCommand(OnOK);

            CancelCommand = new RelayCommand(OnCancel);
        }

        #region 过程

        private void OnOK()
        {

            if (string.IsNullOrWhiteSpace(Id))
            {
                MessageBox.Show("ID不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("名称不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CloseDialog(true);

        }
        private void OnCancel()
        {
            CloseDialog(false);
        }
        #endregion

    }
}
