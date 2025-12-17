using KISHelper.Common;
using KISHelper.Services;
using KISHelper.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KISHelper.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        // 使用独立的导航服务（不是单例）
        public ChildNavigationService Navigator { get; } = new ChildNavigationService();

        public ICommand NavigateCommand { get; }

        public SettingsViewModel()
        {
            NavigateCommand = new RelayCommand<string>(OnNavigate);

            // 注册子页面
            Navigator.Register<BasicInformationSettingViewModel>("BasicInformationSetting");
            Navigator.Register<AccountingRulesSettingViewModel>("AccountingRulesSetting");
            Navigator.Register<AccountingDimensionSettingViewModel>("AccountingDimensionSetting");

            // 自动导航到默认子页面
            Navigator.NavigateTo("BasicInformationSetting");

        }

        private void OnNavigate(string pageName)
        {
            Navigator.NavigateTo(pageName); // 直接传递名称
        }
    }
}
