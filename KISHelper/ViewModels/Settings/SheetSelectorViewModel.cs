using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KISHelper.ViewModels.Settings
{
    public class SheetSelectorViewModel : ViewModelBase
    {
        public ObservableCollection<string> SheetNames { get; set; }

        private string _selectedSheetName;
        public string SelectedSheetName
        {
            get => _selectedSheetName;
            set => SetField(ref _selectedSheetName, value);
        }

        public RelayCommand ConfirmCommand { get; }

        public SheetSelectorViewModel(List<string> sheetNames)
        {
            SheetNames = new ObservableCollection<string>(sheetNames);
            ConfirmCommand = new RelayCommand(() => { /* 对话框关闭 */ });
        }
    }
}
