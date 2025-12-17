using KISHelper.Common;
using KISHelper.Views.Common;
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
        private readonly DataRepository _repository = new();
        public ObservableCollection<AccDimension> AccDimension { get; }

        public AccountingDimensionSettingViewModel()
        {
            AccDimension = new ObservableCollection<AccDimension>(
                _repository.LoadData<AccDimension>("AccDimension"));
        }

        // ========== 命令 ==========
        private ICommand _addDimensionCommand;
        public ICommand AddDimensionCommand => _addDimensionCommand ??= new RelayCommand(ExecuteAddDimension);

        private ICommand _editDimensionCommand;
        public ICommand EditDimensionCommand => _editDimensionCommand ??= new RelayCommand<AccDimension>(ExecuteEditDimension);

        private ICommand _deleteDimensionCommand;
        public ICommand DeleteDimensionCommand => _deleteDimensionCommand ??= new RelayCommand<AccDimension>(ExecuteDeleteDimension);

        private ICommand importDimensionCommand;
        public ICommand ImportDimensionCommand => importDimensionCommand ??= new RelayCommand(ImportDimension);

        private ICommand downloadSampleCommand;
        public ICommand DownloadSampleCommand => downloadSampleCommand ??= new RelayCommand(DownloadSample);

        private void ExecuteAddDimension()
        {
            var dialog = new DimensionDialog("添加");
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // 检查关键字是否已存在
                if (AccDimension.Any(a => a.DimensionType+a.DimensionName == dialog.DimensionType+dialog.DimensionName))
                {
                    MessageBox.Show($"核算维度 '{dialog.DimensionType}'：'{dialog.DimensionName}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AccDimension.Add(new AccDimension { DimensionType= dialog.DimensionType, DimensionName = dialog.DimensionName, DimensionNumber = dialog.DimensionNumber,AccID = dialog.AccID });
                SaveAllData();
            }
        }

        private void ExecuteEditDimension(AccDimension accDimension)
        {
            if (accDimension == null) return;
            var dialog = new DimensionDialog("编辑", accDimension.DimensionType, accDimension.DimensionName, accDimension.DimensionNumber, accDimension.AccID);
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // 如果AccName改了，检查是否与其他冲突
                if (AccDimension.Any(a =>
                    a.DimensionType == dialog.DimensionType &&
                    a.DimensionName == dialog.DimensionName))
                {
                    MessageBox.Show($"核算项目 {dialog.DimensionType}'：'{dialog.DimensionName}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                accDimension.DimensionType = dialog.DimensionType;
                accDimension.DimensionName = dialog.DimensionName;
                accDimension.DimensionNumber = dialog.DimensionNumber;
                accDimension.AccID = dialog.AccID;
                SaveAllData();
            }
        }

        private void ExecuteDeleteDimension(AccDimension accDimension)
        {
            if (accDimension == null) return;

            var result = MessageBox.Show($"确认删除核算维度 '{accDimension.DimensionName}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                AccDimension.Remove(accDimension);
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

        private void DownloadSample()
        {

        }

        private void SaveAllData()
        {
            _repository.SaveData("AccDimension", AccDimension.ToList());
        }
    }
}
