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
                        dpmSelected = DpmDimension.FirstOrDefault(a => a.DimensionName == value.Branch);
                    }
                    
                }
            }
        }
        //如果是银行类型，就需要给银行设置核算科目（根据卡号判断是现金还是银行存款还是其他货币资金）
        public bool IsAccID => AccDimension?.DimensionType == "银行账号";

        public bool IsBanch => AccDimension?.DimensionType is "银行账号" or "部门";

        //如果是银行或部门，设置内部往来编码
        public bool IsInterior => AccDimension?.DimensionType == "部门";

        private ObservableCollection<AccDimension>? dpmDimension;
        public ObservableCollection<AccDimension>? DpmDimension
        {
            get => dpmDimension;
            set => SetField(ref dpmDimension, value);
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
        }

        #region 事件判断
        private void OnDimensionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccDimension.DimensionType))
            {
                OnPropertyChanged(nameof(IsAccID));
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
            return true;
        }
        #endregion
    }
}
