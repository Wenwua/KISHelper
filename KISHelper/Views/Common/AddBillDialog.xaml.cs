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
        public static readonly DependencyProperty AllowDetailID_FFlex5Property =
        DependencyProperty.Register(
            nameof(AllowDetailID_FFlex5),
            typeof(bool),
            typeof(AddBillDialog),
            new PropertyMetadata(false));
        public static readonly DependencyProperty AllowDetailID_FFlex6Property =
        DependencyProperty.Register(
            nameof(AllowDetailID_FFlex6),
            typeof(bool),
            typeof(AddBillDialog),
            new PropertyMetadata(false));
        public static readonly DependencyProperty DetailID_FFlex5Property =
        DependencyProperty.Register(
            nameof(DetailID_FFlex5),
            typeof(string),
            typeof(AddBillDialog),
            new PropertyMetadata(string.Empty));

        public string AccType { get; set; }
        public string DetailID_FFlex6 { get; set; }
        public double AMOUNT { get; set; }
        public ObservableCollection<AccRule> AccRules { get; set; }
        public ObservableCollection<AccDimension> AccDimension { get; set; }
        public ObservableCollection<AccDimension> DpmList { get; set; }
        public ObservableCollection<AccDimension> CustomList { get; set; }

        private readonly DataRepository _repository = new();

        public bool AllowDetailID_FFlex5
        {
            get => (bool)GetValue(AllowDetailID_FFlex5Property);
            set => SetValue(AllowDetailID_FFlex5Property, value);
        }

        public bool AllowDetailID_FFlex6
        {
            get => (bool)GetValue(AllowDetailID_FFlex6Property);
            set => SetValue(AllowDetailID_FFlex6Property, value);
        }

        public string DetailID_FFlex5
        {
            get => (string)GetValue(DetailID_FFlex5Property);
            set => SetValue(DetailID_FFlex5Property, value);
        }

        private AccRule _selectedAccRule;
        public AccRule SelectedAccRule
        {
            get => _selectedAccRule;
            set
            {
                _selectedAccRule = value;
                if (value != null)
                {
                    // 同时更新多个属性
                    AccType = value.AccName;
                    if (value.AccFiexItem.Contains("部门"))
                    {
                        AllowDetailID_FFlex5 = true;
                        var result = AccRules
                            .Where(a => a.AccName == AccType).ToList();
                        if (result.Any()) { DetailID_FFlex5 = result[0].DefaultDetailID_FFlex5; }
                    }
                    else { AllowDetailID_FFlex5 = false; }
                    AllowDetailID_FFlex6 = value.AccFiexItem?.Contains("客户") ?? false;
                }
            }
        }
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
