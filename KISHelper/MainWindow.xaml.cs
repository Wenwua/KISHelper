using KISHelper.License;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KISHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Check();
        }

        private async void Check()
        {
            bool authorized = await GitHubLicenseClient.CheckAsync();
            if (!authorized)
            {
                MessageBox.Show("授权失败，请检查本地 Key 或远程 Code。");
                Application.Current.Shutdown();
                return;
            }
            //检查更新
            await GitHubLicenseClient.CheckUpdate();
        }
    }


}