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
using System.Xml.Linq;

namespace KISHelper.Views.Common
{
    /// <summary>
    /// RuleDialog.xaml 的交互逻辑
    /// </summary>
    public partial class RuleDialog : Window
    {
        public static readonly DependencyProperty AllowDefaultDetailID_FFlex5Property =
        DependencyProperty.Register(
            nameof(AllowDefaultDetailID_FFlex5),
            typeof(bool),
            typeof(RuleDialog),
            new PropertyMetadata(false));

        public string AccName { get; set; }
        public string AccountID { get; set; }
        public string AccFDC { get; set; }
        public bool AllowDefaultDetailID_FFlex5
        {
            get => (bool)GetValue(AllowDefaultDetailID_FFlex5Property);
            set => SetValue(AllowDefaultDetailID_FFlex5Property, value);
        }

        private string defaultDetailID_FFlex5;
        public string DefaultDetailID_FFlex5
        {
            get => defaultDetailID_FFlex5;
            set => defaultDetailID_FFlex5 = value;
        }

        private string accFiexItem;
        public string AccFiexItem
        {
            get => accFiexItem;
            set
            {
                accFiexItem = value;
                // 关键：当值改变时更新依赖属性
                AllowDefaultDetailID_FFlex5 = !string.IsNullOrEmpty(value) && value.Contains("部门");
            }
        }
        public ObservableCollection<AccDimension> AccDimension { get; } = new();

        private readonly DataRepository _repository = new();
        public RuleDialog(string title, string accname = "", string accountID = "", string accFDC = "", string accFiexItem = "")
        {
            InitializeComponent();
            Title = title;
            AccName = accname;
            AccountID = accountID;
            AccFDC = accFDC;
            AccFiexItem = accFiexItem;
            if (accFiexItem.Contains("部门")) AllowDefaultDetailID_FFlex5 = true;
            DataContext = this;
            var result = new ObservableCollection<AccDimension>(_repository.LoadData<AccDimension>("AccDimension"))
                .Where(a => a.DimensionType == "部门").ToList();
            foreach(var item in result)
            {
                AccDimension.Add(item);
            }
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
