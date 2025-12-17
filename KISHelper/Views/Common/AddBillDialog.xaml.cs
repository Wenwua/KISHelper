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

namespace KISHelper.Views.Common
{
    /// <summary>
    /// AddBillDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddBillDialog : Window
    {
        public string AccType { get; set; }
        public string DetailID_FFlex6 { get; set; }
        public string DetailID_FFlex5 { get; set; }
        public double AMOUNT { get; set; }
        public ObservableCollection<AccRule> AccRules { get; set; }
        public ObservableCollection<AccDimension> AccDimension { get; set; }
        public ObservableCollection<AccDimension> DpmList { get; set; }
        public ObservableCollection<AccDimension> CustomList { get; set; }
        private readonly DataRepository _repository = new();
        public AddBillDialog()
        {
            InitializeComponent();
            AccRules = new ObservableCollection<AccRule>(
                _repository.LoadData<AccRule>("AccRules"));
            AccDimension = new ObservableCollection<AccDimension>(
                _repository.LoadData<AccDimension>("AccDimension"));

            var filteredList = AccDimension.Where(d => d.DimensionType == "部门").ToList();
            DpmList = new ObservableCollection<AccDimension>(filteredList);
            filteredList = AccDimension.Where(d => d.DimensionType == "客户").ToList();
            CustomList = new ObservableCollection<AccDimension>(filteredList);
            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AccType))
            {
                MessageBox.Show("核算类型不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (AMOUNT==0)
            {
                MessageBox.Show("金额不能为零！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
