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

namespace KISHelper.Views.Common
{
    /// <summary>
    /// DimensionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DimensionDialog : Window
    {
        public string DimensionType { get; set; }
        public string DimensionName { get; set; }
        public string DimensionNumber { get; set; }
        public string AccID { get; set; }
        public DimensionDialog(string title, string dimensionType = "", string dimensionName = "", string dimensionNumber = "", string accID = "")
        {
            InitializeComponent();
            Title = title;
            DimensionType = dimensionType;
            DimensionName = dimensionName;
            DimensionNumber = dimensionNumber;
            AccID = accID;
            DataContext = this;
            TypeBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DimensionType))
            {
                MessageBox.Show("维度类型不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(DimensionName))
            {
                MessageBox.Show("维度名称不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(DimensionNumber))
            {
                MessageBox.Show("维度编码不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
