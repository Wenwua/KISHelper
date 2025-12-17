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
    /// InputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AccBookDialog : Window
    {
        public string Id { get; set; }
        public string _Name { get; set; }

        public AccBookDialog(string title, string id = "", string name = "")
        {
            InitializeComponent();
            Title = title;
            Id = id;
            _Name = name;
            DataContext = this;
            NameBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                MessageBox.Show("ID不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(_Name))
            {
                MessageBox.Show("名称不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                NameBox.Focus();
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
