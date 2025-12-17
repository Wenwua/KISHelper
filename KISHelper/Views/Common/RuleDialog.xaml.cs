using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace KISHelper.Views.Common
{
    /// <summary>
    /// RuleDialog.xaml 的交互逻辑
    /// </summary>
    public partial class RuleDialog : Window
    {
        public string AccName { get; set; }
        public string AccountID { get; set; }
        public string AccFDC { get; set; }
        public string AccFiexItem { get; set; }
        public RuleDialog(string title, string accname = "", string accountID = "",string accFDC = "", string accFiexItem = "")
        {
            InitializeComponent();
            Title = title;
            AccName = accname;
            AccountID = accountID;
            AccFDC = accFDC;
            AccFiexItem = accFiexItem;
            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AccName))
            {
                MessageBox.Show("核算类型不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(AccountID))
            {
                MessageBox.Show("科目编码不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(AccFDC))
            {
                MessageBox.Show("余额方向不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
