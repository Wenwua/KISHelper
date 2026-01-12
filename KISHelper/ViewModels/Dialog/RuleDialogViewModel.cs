using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.ViewModels.Dialog
{
    public class RuleDialogViewModel : DialogViewModelBase
    {
        #region 必需参数
        private AccRule? accRule;
        public AccRule? AccRule
        {
            get => accRule;
            set 
            {
                if (accRule != null)
                {
                    accRule.PropertyChanged -= OnAccRulePropertyChanged;
                    
                }

                SetField(ref accRule, value);
                if (accRule != null)
                {
                    accRule.PropertyChanged += OnAccRulePropertyChanged;
                    SyncAffiliatedWithSelectedBook();
                    UpdateAllowDefaultDetailID_FFlex5();
                }
            } 
        }
        private void SyncAffiliatedWithSelectedBook()
        {
            if (AccRule != null)
                AccRule.Affiliated = AccBookSelected?.Name;
        }

        private ObservableCollection<AccountBook>? accountBooks;
        public ObservableCollection<AccountBook>? AccountBooks
        {
            get => accountBooks;
            set => SetField(ref accountBooks, value);
        }

        private AccountBook? accBookSelected;
        public AccountBook? AccBookSelected
        {
            get => accBookSelected;
            set 
            { 
                if(SetField(ref accBookSelected, value))
                {
                    var list = accDimension.Where(a => a.Affiliated == value?.Name).ToList();
                    DpmDimension = list == null ? null : new ObservableCollection<AccDimension>(list);
                    SyncAffiliatedWithSelectedBook();
                }
                
            }
        }

        private ObservableCollection<AccDimension>? accDimension;
        public ObservableCollection<AccDimension>? AccDimension
        {
            get => accDimension;
            set => SetField(ref accDimension, value);
        }

        private ObservableCollection<AccDimension>? dpmDimension=new();
        public ObservableCollection<AccDimension>? DpmDimension
        {
            get => dpmDimension;
            set => SetField(ref dpmDimension, value);
        }

        private bool allowDefaultDetailID_FFlex5;
        public bool AllowDefaultDetailID_FFlex5
        {
            get => allowDefaultDetailID_FFlex5;
            set => SetField(ref allowDefaultDetailID_FFlex5, value);
        }

        private readonly DataRepository _repository = new();
        #endregion

        #region 命令
        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }

        #endregion

        public RuleDialogViewModel()
        {
            OKCommand = new RelayCommand(OnOK);

            CancelCommand = new RelayCommand(OnCancel);

            var _accDimension = new ObservableCollection<AccDimension>(
                _repository.LoadData<AccDimension>("AccDimension"));
            var result = _accDimension.Where(a => a.DimensionType == "部门").ToList();
            AccDimension = new ObservableCollection<AccDimension>(result);
            var _accbooks = new ObservableCollection<AccountBook>(
                _repository.LoadData<AccountBook>("AccountBooks"));
            
            accountBooks = new ObservableCollection<AccountBook>(_accbooks);
            AccBookSelected = AccountBooks?.FirstOrDefault();
            AccRule = new AccRule();

        }

        #region 过程

        private void OnOK()
        {
            if (AccRule == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(AccRule.AccName))
            {
                MessageBox.Show("核算项目不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(AccRule.AccountID))
            {
                MessageBox.Show("科目编码不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(AccRule.AccFDC))
            {
                MessageBox.Show("借贷方向不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CloseDialog(true);

        }
        private void OnCancel()
        {
            CloseDialog(false);
        }

        private void OnAccRulePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // 当 AccFiexItem 改变时，更新 ComboBox 的可用状态
            if (e.PropertyName == nameof(AccRule.AccFiexItem))
            {
                UpdateAllowDefaultDetailID_FFlex5();
            }
        }
        private void UpdateAllowDefaultDetailID_FFlex5()
        {
            if (AccRule != null && !string.IsNullOrEmpty(AccRule.AccFiexItem)
                && AccRule.AccFiexItem.Contains("部门"))
            {
                AllowDefaultDetailID_FFlex5 = true;
            }
            else
            {
                AllowDefaultDetailID_FFlex5 = false;
            }
        }
        protected override void OnDialogClosed()
        {
            //  释放订阅
            if (accRule != null)
            {
                WeakEventManager<AccRule, PropertyChangedEventArgs>
                    .AddHandler(accRule, nameof(accRule.PropertyChanged), OnAccRulePropertyChanged);
                UpdateAllowDefaultDetailID_FFlex5();
            }
        }
        #endregion
    }
}
