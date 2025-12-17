using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static KISHelper.Common.Npoi;

namespace KISHelper.Common
{
    public class BillInfo : ViewModelBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
        }

        public void SetSelectedSilently(bool value)
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                // 不触发 PropertyChanged，避免事件风暴
            }
        }

        private string? accNumber;
        [ExcelColumn("AccNumber", Order = 1)]
        public string? AccNumber
        {
            get => accNumber;
            set => SetField(ref accNumber, value);
        }

        private string? billWayNumber;
        [ExcelColumn("BillWayNumber", Order = 2)]
        public string? BillWayNumber
        {
            get => billWayNumber;
            set => SetField(ref billWayNumber, value);
        }

        private string? accType;
        [ExcelColumn("AccType", Order = 3)]
        public string? AccType
        {
            get => accType;
            set => SetField(ref accType, value);
        }

        private string? detailID_FFlex6;
        [ExcelColumn("DetailID_FFlex6", Order = 4)]
        public string? DetailID_FFlex6
        {
            get => detailID_FFlex6;
            set => SetField(ref detailID_FFlex6, value);
        }

        private string? detailID_FFlex5;
        [ExcelColumn("DetailID_FFlex5", Order = 5)]
        public string? DetailID_FFlex5
        {
            get => detailID_FFlex5;
            set => SetField(ref detailID_FFlex5, value);
        }

        private string? detailID_FFlex4;
        [ExcelColumn("DetailID_FFlex4", Order = 6)]
        public string? DetailID_FFlex4
        {
            get => detailID_FFlex4;
            set => SetField(ref detailID_FFlex4, value);
        }

        private double aMOUNT;
        [ExcelColumn("AMOUNT", Order = 7)]
        public double AMOUNT
        {
            get => aMOUNT;
            set => SetField(ref aMOUNT, value);
        }

        private string? tRANSDATE;
        [ExcelColumn("TRANSDATE", Order = 8)]
        public string? TRANSDATE
        {
            get => tRANSDATE;
            set => SetField(ref tRANSDATE, value);
        }

        public bool IsTemporary { get; set; } = false;
    }
}