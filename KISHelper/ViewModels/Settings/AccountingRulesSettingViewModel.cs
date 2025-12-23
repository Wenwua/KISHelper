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
    public class AccountingRulesSettingViewModel : ViewModelBase
    {
        #region 绑定的类
        private AccRule? selectedItem;
        public AccRule? SelectedItem
        {
            get => selectedItem;
            set => SetField(ref selectedItem, value);
        }

        private readonly DataRepository _repository = new();
        public ObservableCollection<AccRule> AccRules { get; }
        #endregion
        public AccountingRulesSettingViewModel()
        {
            AccRules = new ObservableCollection<AccRule>(
                _repository.LoadData<AccRule>("AccRules"));
            AddRuleCommand = new RelayCommand(ExecuteAddRule);
            EditRuleCommand = new RelayCommand(ExecuteEditRule);
            DeleteRuleCommand = new RelayCommand(ExecuteDeleteRule);
            ImportRuleCommand = new RelayCommand(ImportRule);

        }

        #region 命令
        public RelayCommand AddRuleCommand { get; } 
        public RelayCommand EditRuleCommand { get; }
        public RelayCommand DeleteRuleCommand { get; }
        public ICommand ImportRuleCommand { get; }
        #endregion

        #region 过程
        private void ExecuteAddRule()
        {
            var dialog = new RuleDialog()
            {
                Owner = Application.Current.MainWindow,
                DataContext = new RuleDialogViewModel()
            };
            var vm = dialog.DataContext as RuleDialogViewModel;
            if (dialog.ShowDialog() == true && vm != null && vm.AccRule != null)
            {
                // 检查ID是否已存在
                if (AccRules.Any(a => a.AccName == vm.AccRule.AccName))
                {
                    MessageBox.Show($"核算项目 '{vm.AccRule.AccName}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                AccRules.Add(vm.AccRule);
                SaveAllData();
            }
        }

        private void ExecuteEditRule()
        {
            if (SelectedItem == null) return;
            var dialog = new RuleDialog() 
            {
                Owner = Application.Current.MainWindow,
                DataContext = new RuleDialogViewModel()
            };
            var vm = dialog.DataContext as RuleDialogViewModel;
            if (vm != null)
            {
                vm.AccRule = SelectedItem;
            }
            if (dialog.ShowDialog() == true && vm != null && vm.AccRule != null)
            {
                SelectedItem = vm.AccRule;
                SaveAllData();
            }
        }

        private void ExecuteDeleteRule()
        {
            if (SelectedItem == null) return;

            var result = MessageBox.Show($"确认删除核算规则 '{SelectedItem.AccName}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                AccRules.Remove(SelectedItem);
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

        private void SaveAllData()
        {
            _repository.SaveData("AccRules", AccRules.ToList());
        }
        #endregion
    }
}
