using KISHelper.Common;
using KISHelper.Services;
using KISHelper.ViewModels.Dialog;
using KISHelper.Views.Dialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KISHelper.ViewModels.Settings
{
    
    public class BasicInformationSettingViewModel:ViewModelBase
    {
        #region 绑定的类
        private AccountBook? selectedBook;
        public AccountBook? SelectedBook
        {
            get => selectedBook;
            set => SetField(ref selectedBook, value);
        }
        private VoucherGroup? selectedVoucher;
        public VoucherGroup? SelectedVoucher
        {
            get => selectedVoucher;
            set => SetField(ref selectedVoucher, value);
        }
        private readonly DataRepository _repository = new();
        public ObservableCollection<AccountBook> AccountBooks { get; }
        public ObservableCollection<VoucherGroup> VoucherGroups { get; }
        #endregion

        public BasicInformationSettingViewModel()
        {
            // 加载数据
            AccountBooks = new ObservableCollection<AccountBook>(
                _repository.LoadData<AccountBook>("AccountBooks")
            );
            VoucherGroups = new ObservableCollection<VoucherGroup>(
                _repository.LoadData<VoucherGroup>("VoucherGroups")
            );
            //绑定事件
            AddAccountCommand = new RelayCommand(ExecuteAddAccount);
            EditAccountCommand = new RelayCommand(ExecuteEditAccount);
            DeleteAccountCommand = new RelayCommand(ExecuteDeleteAccount);
            AddVoucherCommand = new RelayCommand(ExecuteAddVoucher);
            EditVoucherCommand = new RelayCommand(ExecuteEditVoucher);
            DeleteVoucherCommand = new RelayCommand(ExecuteDeleteVoucher);
        }

        #region 命令组
        public RelayCommand AddAccountCommand { get; }
        public RelayCommand EditAccountCommand { get; }
        public RelayCommand DeleteAccountCommand { get; }
        public RelayCommand AddVoucherCommand { get; }
        public RelayCommand EditVoucherCommand { get; }
        public RelayCommand DeleteVoucherCommand { get; }
        #endregion

        #region 账簿组方法
        private void ExecuteAddAccount()
        {
            var dialog = new AccBookDialog() 
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AccBookDialogViewModel()
            };
            var vm = dialog.DataContext as AccBookDialogViewModel;
            if (dialog.ShowDialog() == true && vm != null)
            {
                // 检查ID是否已存在
                if (AccountBooks.Any(a => a.Id == vm.Id))
                {
                    MessageBox.Show($"ID '{vm.Id}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                AccountBooks.Add(new AccountBook { Id = vm.Id, Name = vm.Name });
                SaveAllData();
            }
        }
        private void ExecuteEditAccount()
        {
            if (SelectedBook == null) return;
            var dialog = new AccBookDialog() 
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AccBookDialogViewModel()
            };
            var vm = dialog.DataContext as AccBookDialogViewModel;
            if (vm != null)
            {
                vm.Id = SelectedBook.Id;
                vm.Name = SelectedBook.Name;
            }
            if (dialog.ShowDialog() == true && vm != null)
            {

                SelectedBook.Id = vm.Id;
                SelectedBook.Name = vm.Name;
                SaveAllData();
            }
        }
        private void ExecuteDeleteAccount()
        {
            if (SelectedBook == null) return;

            var result = MessageBox.Show($"确认删除账簿 '{SelectedBook.Name}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                AccountBooks.Remove(SelectedBook);
                SaveAllData();
            }
        }
        #endregion

        #region 凭证组方法
        private void ExecuteAddVoucher()
        {
            var dialog = new AccBookDialog()
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AccBookDialogViewModel()
            };
            var vm = dialog.DataContext as AccBookDialogViewModel;
            if (dialog.ShowDialog() == true && vm!=null)
            {
                if (VoucherGroups.Any(v => v.Id == vm.Id))
                {
                    MessageBox.Show($"ID '{vm.Id}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                VoucherGroups.Add(new VoucherGroup { Id = vm.Id, Name = vm.Name });
                SaveAllData();
            }
        }
        private void ExecuteEditVoucher()
        {
            if (SelectedVoucher == null) return;
            var dialog = new AccBookDialog()
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AccBookDialogViewModel()
            };
            var vm = dialog.DataContext as AccBookDialogViewModel;
            if (vm != null)
            {
                vm.Id = SelectedVoucher.Id;
                vm.Name = SelectedVoucher.Name;
            }
            if (dialog.ShowDialog() == true && vm!= null)
            {
                SelectedVoucher.Id = vm.Id;
                SelectedVoucher.Name = vm.Name;
                SaveAllData();
            }
        }
        private void ExecuteDeleteVoucher()
        {
            if (SelectedVoucher == null) return;

            var result = MessageBox.Show($"确认删除凭证字 '{SelectedVoucher.Name}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                VoucherGroups.Remove(SelectedVoucher);
                SaveAllData();
            }
        }
        private void SaveAllData()
        {
            _repository.SaveData("AccountBooks", AccountBooks.ToList());
            _repository.SaveData("VoucherGroups", VoucherGroups.ToList());
        }
        #endregion
    }
}
