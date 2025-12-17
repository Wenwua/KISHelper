using KISHelper.Common;
using KISHelper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KISHelper.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public NavigationService Navigator => NavigationService.Instance;

        public ICommand NavigateCommand { get; }

        public MainWindowViewModel()
        {
            NavigateCommand = new RelayCommand<string>(OnNavigate);

            // 自动导航到首页
            Navigator.NavigateTo("Home"); // 使用注册名称
        }

        private void OnNavigate(string pageName)
        {
            Navigator.NavigateTo(pageName); // 直接传递名称

            if (pageName == "Home")
            {
                var homeViewModel = Navigator.GetCachedViewModel("Home") as HomeViewModel;
                homeViewModel?.RefreshData();
            }
        }
    }
}
