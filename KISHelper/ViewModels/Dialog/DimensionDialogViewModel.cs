using KISHelper.Common;
using System;
using System.Collections.Generic;
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
            set => SetField(ref accDimension, value);
        }
        #endregion

        #region 命令
        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }

        #endregion

        public DimensionDialogViewModel()
        {
            OKCommand = new RelayCommand(OnOK);

            CancelCommand = new RelayCommand(OnCancel);
        }

        #region 过程

        private void OnOK()
        {
            if (AccDimension == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(AccDimension.DimensionType))
            {
                MessageBox.Show("维度类型不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(AccDimension.DimensionName))
            {
                MessageBox.Show("维度名称不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(AccDimension.DimensionNumber))
            {
                MessageBox.Show("维度编码不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
