using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static KISHelper.Common.Npoi;

namespace KISHelper.Common
{
    public class BillInfo : ViewModelBase
    {

        //是否选中
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
        [ExcelColumn("凭证号", Order = 1)]
        public string? AccNumber
        {
            get => accNumber;
            set => SetField(ref accNumber, value);
        }

        private string? billWayNumber;
        [ExcelColumn("来源单号", Order = 2)]
        public string? BillWayNumber
        {
            get => billWayNumber;
            set => SetField(ref billWayNumber, value);
        }

        private string? accType;
        [ExcelColumn("核销类型", Order = 3)]
        public string? AccType
        {
            get => accType;
            set => SetField(ref accType, value);
        }

        private string? detailID_FFlex6;
        [ExcelColumn("客户", Order = 4)]
        public string? DetailID_FFlex6
        {
            get => detailID_FFlex6;
            set => SetField(ref detailID_FFlex6, value);
        }

        private string? detailID_FFlex5;
        [ExcelColumn("部门", Order = 5)]
        public string? DetailID_FFlex5
        {
            get => detailID_FFlex5;
            set => SetField(ref detailID_FFlex5, value);
        }

        private string? detailID_FFlex4;
        [ExcelColumn("供应商", Order = 6)]
        public string? DetailID_FFlex4
        {
            get => detailID_FFlex4;
            set => SetField(ref detailID_FFlex4, value);
        }


        private double aMOUNT;
        [ExcelColumn("核销金额", Order = 7)]
        public double AMOUNT
        {
            get => aMOUNT;
            set => SetField(ref aMOUNT, value);
        }

        private string? tRANSDATE;
        [ExcelColumn("核销日期", Order = 8)]
        public string? TRANSDATE
        {
            get => tRANSDATE;
            set => SetField(ref tRANSDATE, value);
        }

        private string? detailID_FFlex9;
        [ExcelColumn("费用项目", Order = 9)]
        public string? DetailID_FFlex9
        {
            get => detailID_FFlex9;
            set => SetField(ref detailID_FFlex9, value);
        }

        private string? detailID_FF100009;
        [ExcelColumn("银行账号", Order = 10)]
        public string? DetailID_FF100009
        {
            get => detailID_FF100009;
            set => SetField(ref detailID_FF100009, value);
        }

        private string? balanceDirection;
        [ExcelColumn("余额方向", Order = 11)]
        public string? BalanceDirection
        {
            get => balanceDirection;
            set => SetField(ref balanceDirection, value);
        }

        private string? fEXPLANATION;
        [ExcelColumn("摘要", Order = 12)]
        public string? FEXPLANATION
        {
            get => fEXPLANATION;
            set => SetField(ref fEXPLANATION, value);
        }

        //是否临时数据
        public bool IsTemporary { get; set; } = false;

        public bool IsBankBillInfo { get; set; } = false;
    }
}