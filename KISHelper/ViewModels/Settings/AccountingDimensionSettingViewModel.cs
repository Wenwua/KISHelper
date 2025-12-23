using KISHelper.Common;
using KISHelper.ViewModels.Dialog;
using KISHelper.Views.Dialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static KISHelper.Common.Npoi;

namespace KISHelper.ViewModels.Settings
{
    public class AccountingDimensionSettingViewModel : ViewModelBase
    {
        #region 绑定的类
        private AccDimension? selectedItem;
        public AccDimension? SelectedItem
        {
            get => selectedItem;
            set => SetField(ref selectedItem, value);
        }
        private readonly DataRepository _repository = new();
        public ObservableCollection<AccDimension> AccDimension { get; }
        #endregion

        public AccountingDimensionSettingViewModel()
        {
            AccDimension = new ObservableCollection<AccDimension>(
                _repository.LoadData<AccDimension>("AccDimension"));
            AddDimensionCommand = new RelayCommand(ExecuteAddDimension);
            EditDimensionCommand = new RelayCommand(ExecuteEditDimension);
            DeleteDimensionCommand = new RelayCommand(ExecuteDeleteDimension);
            ImportDimensionCommand = new RelayCommand(ImportDimension);
        }

        #region 命令

        public RelayCommand AddDimensionCommand { get; }
        public RelayCommand EditDimensionCommand { get; }
        public RelayCommand DeleteDimensionCommand { get; }
        public RelayCommand ImportDimensionCommand { get; }

        #endregion

        #region 过程
        private void ExecuteAddDimension()
        {
            var dialog = new DimensionDialog()
            {
                Owner = Application.Current.MainWindow,
                DataContext = new DimensionDialogViewModel()
            };
            var vm = dialog.DataContext as DimensionDialogViewModel;
            if (dialog.ShowDialog() == true && vm!=null && vm.AccDimension != null)
            {
                // 检查关键字是否已存在
                if (AccDimension.Any(a => a.DimensionType== vm.AccDimension.DimensionType && a.DimensionName==vm.AccDimension.DimensionName))
                {
                    MessageBox.Show($"核算维度 '{vm.AccDimension.DimensionType}'：'{vm.AccDimension.DimensionName}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                AccDimension.Add(vm.AccDimension);
                SaveAllData();
            }
        }

        private void ExecuteEditDimension()
        {
            if (SelectedItem == null) return;
            var dialog = new DimensionDialog()
            {
                Owner = Application.Current.MainWindow,
                DataContext = new DimensionDialogViewModel()
            };
            var vm = dialog.DataContext as DimensionDialogViewModel;
            if (vm != null)
            {
                vm.AccDimension = SelectedItem;
            }
            if (dialog.ShowDialog() == true && vm != null && vm.AccDimension != null)
            {
                SelectedItem = vm.AccDimension;
                SaveAllData();
            }
        }

        private void ExecuteDeleteDimension()
        {
            if (SelectedItem == null) return;

            var result = MessageBox.Show($"确认删除核算维度 '{SelectedItem.DimensionName}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                AccDimension.Remove(SelectedItem);
                SaveAllData();
            }
        }

        private void ImportDimension()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel文件|*.xlsx;*.xls;*.xlsm|所有文件|*.*",
                Title = "选择要导入的Excel文件"

            };
            if (openFileDialog.ShowDialog() != true) return;
            try
            {
                var accs = NpoiHelper.ReadFromExcel<AccDimension>(openFileDialog.FileName, sheetIndex: 0);
                foreach (var acc in accs)
                {
                    var existing = AccDimension.FirstOrDefault(a =>
                    a.DimensionType == acc.DimensionType &&
                    a.DimensionName == acc.DimensionName);

                    if (existing != null)
                    {
                        AccDimension.Remove(existing);
                    }

                    AccDimension.Add(acc);
                }
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveAllData()
        {
            _repository.SaveData("AccDimension", AccDimension.ToList());
        }

        #endregion
    }
}
