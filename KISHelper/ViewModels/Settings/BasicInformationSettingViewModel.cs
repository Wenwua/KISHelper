using KISHelper.Common;
using KISHelper.Services;
using KISHelper.Views.Common;
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
        private readonly DataRepository _repository = new();

        public ObservableCollection<AccountBook> AccountBooks { get; }
        public ObservableCollection<VoucherGroup> VoucherGroups { get; }

        public BasicInformationSettingViewModel()
        {
            // 加载数据（只需要指定key和类型）
            AccountBooks = new ObservableCollection<AccountBook>(
                _repository.LoadData<AccountBook>("AccountBooks")
            );
            VoucherGroups = new ObservableCollection<VoucherGroup>(
                _repository.LoadData<VoucherGroup>("VoucherGroups")
            );
        }

        // ========== 账户簿命令 ==========
        private ICommand _addAccountCommand;
        public ICommand AddAccountCommand => _addAccountCommand ??= new RelayCommand(ExecuteAddAccount);

        private ICommand _editAccountCommand;
        public ICommand EditAccountCommand => _editAccountCommand ??= new RelayCommand<AccountBook>(ExecuteEditAccount);

        private ICommand _deleteAccountCommand;
        public ICommand DeleteAccountCommand => _deleteAccountCommand ??= new RelayCommand<AccountBook>(ExecuteDeleteAccount);

        // ========== 凭证组命令 ==========
        private ICommand _addVoucherCommand;
        public ICommand AddVoucherCommand => _addVoucherCommand ??= new RelayCommand(ExecuteAddVoucher);

        private ICommand _editVoucherCommand;
        public ICommand EditVoucherCommand => _editVoucherCommand ??= new RelayCommand<VoucherGroup>(ExecuteEditVoucher);

        private ICommand _deleteVoucherCommand;
        public ICommand DeleteVoucherCommand => _deleteVoucherCommand ??= new RelayCommand<VoucherGroup>(ExecuteDeleteVoucher);


        // ========== 执行方法 ==========
        private void ExecuteAddAccount()
        {
            var dialog = new AccBookDialog("添加账簿");
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // 检查ID是否已存在
                if (AccountBooks.Any(a => a.Id == dialog.Id))
                {
                    MessageBox.Show($"ID '{dialog.Id}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AccountBooks.Add(new AccountBook { Id = dialog.Id, Name = dialog._Name });
                SaveAllData();
            }
        }

        private void ExecuteEditAccount(AccountBook account)
        {
            if (account == null) return;
            var dialog = new AccBookDialog("编辑账簿", account.Id, account.Name);
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // 如果ID改了，检查是否与其他冲突
                if (dialog.Id != account.Id && AccountBooks.Any(a => a.Id == dialog.Id))
                {
                    MessageBox.Show($"ID '{dialog.Id}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                account.Id = dialog.Id;
                account.Name = dialog._Name;
                SaveAllData();
            }
        }

        private void ExecuteDeleteAccount(AccountBook account)
        {
            if (account == null) return;

            var result = MessageBox.Show($"确认删除账簿 '{account.Name}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                AccountBooks.Remove(account);
                SaveAllData();
            }
        }

        // 凭证组方法（与账户簿完全相同逻辑）
        private void ExecuteAddVoucher()
        {
            var dialog = new AccBookDialog("添加凭证字");
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                if (VoucherGroups.Any(v => v.Id == dialog.Id))
                {
                    MessageBox.Show($"ID '{dialog.Id}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                VoucherGroups.Add(new VoucherGroup { Id = dialog.Id, Name = dialog._Name });
                SaveAllData();
            }
        }

        private void ExecuteEditVoucher(VoucherGroup voucher)
        {
            if (voucher == null) return;
            var dialog = new AccBookDialog("编辑凭证字", voucher.Id, voucher.Name);
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Id != voucher.Id && VoucherGroups.Any(v => v.Id == dialog.Id))
                {
                    MessageBox.Show($"ID '{dialog.Id}' 已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                voucher.Id = dialog.Id;
                voucher.Name = dialog._Name;
                SaveAllData();
            }
        }

        private void ExecuteDeleteVoucher(VoucherGroup voucher)
        {
            if (voucher == null) return;

            var result = MessageBox.Show($"确认删除凭证字 '{voucher.Name}' 吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                VoucherGroups.Remove(voucher);
                SaveAllData();
            }
        }

        private void SaveAllData()
        {
            _repository.SaveData("AccountBooks", AccountBooks.ToList());
            _repository.SaveData("VoucherGroups", VoucherGroups.ToList());
        }
    }
}
