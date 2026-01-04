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

        //凭证号
        private string? accNumber;
        [ExcelColumn("AccNumber", Order = 1)]
        public string? AccNumber
        {
            get => accNumber;
            set => SetField(ref accNumber, value);
        }

        //来源单号
        private string? billWayNumber;
        [ExcelColumn("BillWayNumber", Order = 2)]
        public string? BillWayNumber
        {
            get => billWayNumber;
            set => SetField(ref billWayNumber, value);
        }

        //核算类型
        private string? accType;
        [ExcelColumn("AccType", Order = 3)]
        public string? AccType
        {
            get => accType;
            set => SetField(ref accType, value);
        }

        //客户
        private string? detailID_FFlex6;
        [ExcelColumn("DetailID_FFlex6", Order = 4)]
        public string? DetailID_FFlex6
        {
            get => detailID_FFlex6;
            set => SetField(ref detailID_FFlex6, value);
        }

        //部门
        private string? detailID_FFlex5;
        [ExcelColumn("DetailID_FFlex5", Order = 5)]
        public string? DetailID_FFlex5
        {
            get => detailID_FFlex5;
            set => SetField(ref detailID_FFlex5, value);
        }

        //供应商
        private string? detailID_FFlex4;
        [ExcelColumn("DetailID_FFlex4", Order = 6)]
        public string? DetailID_FFlex4
        {
            get => detailID_FFlex4;
            set => SetField(ref detailID_FFlex4, value);
        }

        //金额
        private double aMOUNT;
        [ExcelColumn("AMOUNT", Order = 7)]
        public double AMOUNT
        {
            get => aMOUNT;
            set => SetField(ref aMOUNT, value);
        }

        //核销时间
        private string? tRANSDATE;
        [ExcelColumn("TRANSDATE", Order = 8)]
        public string? TRANSDATE
        {
            get => tRANSDATE;
            set => SetField(ref tRANSDATE, value);
        }

        //费用项目
        private string? detailID_FFlex9;
        [ExcelColumn("DetailID_FFlex9", Order = 9)]
        public string? DetailID_FFlex9
        {
            get => detailID_FFlex9;
            set => SetField(ref detailID_FFlex9, value);
        }

        //余额方向
        private string? balanceDirection;
        [ExcelColumn("BalanceDirection", Order = 10)]
        public string? BalanceDirection
        {
            get => balanceDirection;
            set => SetField(ref balanceDirection, value);
        }

        //是否零时数据
        public bool IsTemporary { get; set; } = false;
    }
}