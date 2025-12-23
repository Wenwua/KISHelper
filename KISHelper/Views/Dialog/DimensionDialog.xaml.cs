using KISHelper.Common;
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

namespace KISHelper.Views.Dialog
{
    /// <summary>
    /// DimensionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DimensionDialog : Window
    {
        public DimensionDialog()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
            this.Closed += OnClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DialogViewModelBase vm)
            {
                vm.RequestClose += OnRequestClose;
            }
            this.Loaded -= OnLoaded; // 清理自身事件
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            if (DataContext is DialogViewModelBase vm)
            {
                vm.RequestClose -= OnRequestClose; // 清理ViewModel事件
            }
            this.Closed -= OnClosed; //  清理自身事件
        }
        private void OnRequestClose(object? sender, DialogCloseEventArgs e)
        {
            this.DialogResult = e.DialogResult;
            this.Close();
        }
    }
}
