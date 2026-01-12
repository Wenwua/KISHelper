using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KISHelper.Common.Npoi;

namespace KISHelper.Common
{
    public class AccountBook:ViewModelBase
    {
        private string? _id;
        private string? _name;

        public string? Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string? Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

    }

    public class VoucherGroup : ViewModelBase
    {
        private string? _id;
        private string? _name;

        public string? Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string? Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }
    }

    public class AccRule : ViewModelBase
    {
        private string? affiliated;
        private string? accName;
        private string? accountID;
        private string? accFDC;
        private string? accFiexItem;
        private string? defaultDetailID_FFlex5;

        //所属组织
        [ExcelColumn("所属组织", Order = 1)]
        public string? Affiliated
        {
            get => affiliated;
            set => SetField(ref affiliated, value);
        }

        //名称
        [ExcelColumn("核算项目", Order = 2)]
        public string? AccName
        {
            get => accName;
            set => SetField(ref accName, value);
        }

        //科目
        [ExcelColumn("科目编码", Order = 3)]
        public string? AccountID
        {
            get => accountID;
            set => SetField(ref accountID, value);
        }

        //余额方向
        [ExcelColumn("余额方向", Order = 4)]
        public string? AccFDC
        {
            get => accFDC;
            set => SetField(ref accFDC, value);
        }

        //核算维度
        [ExcelColumn("核算维度", Order = 5)]
        public string? AccFiexItem
        {
            get => accFiexItem;
            set => SetField(ref accFiexItem, value);
        }

        //核算维度
        [ExcelColumn("默认部门", Order = 6)]
        public string? DefaultDetailID_FFlex5
        {
            get => defaultDetailID_FFlex5;
            set => SetField(ref defaultDetailID_FFlex5, value);
        }
    }

    public class AccDimension : ViewModelBase
    {
        private string? dimensionType;
        private string? dimensionName;
        private string? dimensionNumber;
        private string? accName;
        private string? branch;
        private string? interior;
        public string? affiliated;
        
        //所属组织
        [ExcelColumn("所属组织", Order = 1)]
        public string? Affiliated
        {
            get => affiliated;
            set => SetField(ref affiliated, value);
        }

        //维度类型
        [ExcelColumn("维度类型", Order = 2)]
        public string? DimensionType
        {
            get => dimensionType;
            set => SetField(ref dimensionType, value);
        }

        //维度名称
        [ExcelColumn("维度名称", Order = 3)]
        public string? DimensionName
        {
            get => dimensionName;
            set => SetField(ref dimensionName, value);
        }

        //维度编码
        [ExcelColumn("维度编码", Order = 4)]
        public string? DimensionNumber
        {
            get => dimensionNumber;
            set => SetField(ref dimensionNumber, value);
        }

        //核算规则目---只有银行账号需要指定核算规则
        [ExcelColumn("核算规则", Order = 5)]
        public string? AccName

        {
            get => accName;
            set => SetField(ref accName, value);
        }

        //所属部门---平台公司需要将所有维度核算到部门
        [ExcelColumn("所属部门", Order = 6)]
        public string? Branch

        {
            get => branch;
            set => SetField(ref branch, value);
        }

        //往来科目---当涉及平台公司内部挂账需要指定往来客户
        [ExcelColumn("往来主体", Order = 7)]
        public string? Interior

        {
            get => interior;
            set => SetField(ref interior, value);
        }

    }

    public class VoucherInfo
    {
        public AccountBook? Book { get; set; }
        public VoucherGroup? Voucher { get; set; }
        public AccDimension? Bank { get; set; }
        public DateTime Date { get; set; }
    }
}

