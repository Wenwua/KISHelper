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
        private string? accName;
        private string? accountID;
        private string? accFDC;
        private string? accFiexItem;

        //名称
        [ExcelColumn("名称", Order = 1)]
        public string? AccName
        {
            get => accName;
            set => SetField(ref accName, value);
        }

        //科目
        [ExcelColumn("科目", Order = 2)]
        public string? AccountID
        {
            get => accountID;
            set => SetField(ref accountID, value);
        }

        //余额方向
        [ExcelColumn("余额方向", Order = 3)]
        public string? AccFDC
        {
            get => accFDC;
            set => SetField(ref accFDC, value);
        }

        //核算维度
        [ExcelColumn("核算维度", Order = 4)]
        public string? AccFiexItem
        {
            get => accFiexItem;
            set => SetField(ref accFiexItem, value);
        }
    }

    public class AccDimension : ViewModelBase
    {
        private string? dimensionType;
        private string? dimensionName;
        private string? dimensionNumber;
        private string? accId;

        //维度类型
        [ExcelColumn("维度类型", Order = 1)]
        public string? DimensionType
        {
            get => dimensionType;
            set => SetField(ref dimensionType, value);
        }

        //维度名称
        [ExcelColumn("维度名称", Order = 2)]
        public string? DimensionName
        {
            get => dimensionName;
            set => SetField(ref dimensionName, value);
        }

        //维度编码
        [ExcelColumn("维度编码", Order = 3)]
        public string? DimensionNumber
        {
            get => dimensionNumber;
            set => SetField(ref dimensionNumber, value);
        }

        //核算科目---只有银行账号需要指定现金科目或是银行科目
        [ExcelColumn("核算科目", Order = 4)]
        public string? AccID

        {
            get => accId;
            set => SetField(ref accId, value);
        }
    }
}

