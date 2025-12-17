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
    public class AccountingRulesSettingViewModel : ViewModelBase
    {
        private readonly DataRepository _repository = new();
        public ObservableCollection<AccRule> AccRules { get; }

        public AccountingRulesSettingViewModel()
        {
            AccRules = new ObservableCollection<AccRule>(
                _repository.LoadData<AccRule>("AccRules"));
        }

        // ========== 命令 ==========
        private ICommand _addRuleCommand;
        public ICommand AddRuleCommand => _addRuleCommand ??= new RelayCommand(ExecuteAddRule);

        private ICommand _editRuleCommand;
        public ICommand EditRuleCommand => _editRuleCommand ??= new RelayCommand<AccRule>(ExecuteEditRule);

        private ICommand _deleteRuleCommand;
        public ICommand DeleteRuleCommand => _deleteRuleCommand ??= new RelayCommand<AccRule>(ExecuteDeleteRule);

        private ICommand importRuleCommand;
        public ICommand ImportRuleCommand => importRuleCommand ??= new RelayCommand(ImportRule);

        private ICommand downloadSampleCommand;
        public ICommand DownloadSampleCommand => downloadSampleCommand ??= new RelayCommand(DownloadSample);

        private void ExecuteAddRule()
        {
            var dialog = new RuleDialog("添加");
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // 检查ID是否已存在
                if (AccRules.Any(a => a.AccName == dialog.AccName))
                {
                    MessageBox.Show($"核算项目 '{dialog.AccName}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AccRules.Add(new AccRule { 
                    AccName = dialog.AccName, 
                    AccountID = dialog.AccountID, 
                    AccFDC=dialog.AccFDC,
                    AccFiexItem=dialog.AccFiexItem,
                    DefaultDetailID_FFlex5=dialog.DefaultDetailID_FFlex5 });
                SaveAllData();
            }
        }

        private void ExecuteEditRule(AccRule accRule)
        {
            if (accRule == null) return;
            var dialog = new RuleDialog("编辑", accRule.AccName, accRule.AccountID,accRule.AccFDC,accRule.AccFiexItem);
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // 如果AccName改了，检查是否与其他冲突
                if (dialog.AccName != accRule.AccName && AccRules.Any(a => a.AccName == dialog.AccName))
                {
                    MessageBox.Show($"核算项目 '{dialog.AccName}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                accRule.AccName = dialog.AccName;
                accRule.AccountID= dialog.AccountID;
                accRule.AccFDC= dialog.AccFDC;
                accRule.AccFiexItem= dialog.AccFiexItem;
                accRule.DefaultDetailID_FFlex5 = dialog.DefaultDetailID_FFlex5;
                SaveAllData();
            }
        }

        private void ExecuteDeleteRule(AccRule accRule)
        {
            if (accRule == null) return;

            var result = MessageBox.Show($"确认删除核算规则 '{accRule.AccName}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                AccRules.Remove(accRule);
                SaveAllData();
            }
        }

        private void ImportRule()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel文件|*.xlsx;*.xls;*.xlsm|所有文件|*.*",
                Title = "选择要导入的Excel文件"

            };
            if (openFileDialog.ShowDialog() != true) return;
            try
            {
                var rules = NpoiHelper.ReadFromExcel<AccRule>(openFileDialog.FileName, sheetIndex: 0);
                foreach (var rule in rules) 
                {
                    var existing = AccRules.FirstOrDefault(a =>
                    a.AccName == rule.AccName);

                    if (existing != null)
                    {
                        AccRules.Remove(existing);
                    }

                    AccRules.Add(rule);
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
            _repository.SaveData("AccRules", AccRules.ToList());
        }
    }
}
