using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    
    public partial class AddBillDialog : Window
    {
        
        public AddBillDialog()
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
            this.Loaded -= OnLoaded;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            if (DataContext is DialogViewModelBase vm)
            {
                vm.RequestClose -= OnRequestClose;
            }
            this.Closed -= OnClosed;
        }
        private void OnRequestClose(object? sender, DialogCloseEventArgs e)
        {
            this.DialogResult = e.DialogResult;
            this.Close();
        }
    }
}
