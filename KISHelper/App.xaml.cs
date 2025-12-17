using KISHelper.Common;
using KISHelper.Services;
using KISHelper.ViewModels;
using KISHelper.ViewModels.Settings;
using System.Configuration;
using System.Data;
using System.Windows;

namespace KISHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var navigator = NavigationService.Instance;


            // 注册所有页面
            navigator.Register<HomeViewModel>("Home");
            navigator.Register<SettingsViewModel>("Settings");
            navigator.Register<AboutViewModel>("About");

            navigator.Register<BasicInformationSettingViewModel>("BasicInformationSetting");
            navigator.Register<AccountingRulesSettingViewModel>("AccountingRulesSetting");
            navigator.Register<AccountingDimensionSettingViewModel>("AccountingDimensionSetting");


            // 启动主窗口
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

}
