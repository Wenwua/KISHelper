using KISHelper.ViewModels.Dialog;
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
    /// FolderBrowserDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FolderBrowserDialog : Window
    {
        public string? SelectedPath => ViewModel.SelectedPath;

        private FolderBrowserViewModel ViewModel => (FolderBrowserViewModel)DataContext;

        public FolderBrowserDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DataContext = new FolderBrowserViewModel();
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 设置初始焦点
            if (FolderTreeView.Items.Count > 0)
            {
                var firstItem = FolderTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                firstItem?.Focus();
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem treeViewItem &&
                treeViewItem.DataContext is FolderItemViewModel folderItem)
            {
                folderItem.LoadChildren();
            }
        }

        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            // 可以在这里添加折叠时的处理逻辑
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is FolderItemViewModel folderItem)
            {
                ViewModel.SelectedPath = folderItem.FullPath;
                if (folderItem.IsDirectory)
                {
                    ViewModel.NavigateToPath(folderItem.FullPath);
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedPath))
            {
                MessageBox.Show("请选择一个文件夹", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.IO.Directory.Exists(ViewModel.SelectedPath))
            {
                MessageBox.Show("选择的文件夹不存在", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
