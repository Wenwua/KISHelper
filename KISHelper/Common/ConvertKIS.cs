using MathNet.Numerics;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.Common
{
    public class ConvertKIS
    {
        public int VoucherIndex = 1;

        private int RowIndex = 1;
        public ObservableCollection<Entity>? Entities { get; set; } = new();
        public void Clear()
        {
            VoucherIndex = 1;
            RowIndex = 1;
            Entities.Clear();
            IsFinished = false;
            InitializeComponent();
            
        }

        public ConvertKIS()
        {
            InitializeComponent();
        }

        private readonly DataRepository _repository = new();
        public ObservableCollection<AccRule>? AccRules { get; set; } = new();
        public ObservableCollection<AccDimension>? AccDimension { get; set; } = new();

        private void InitializeComponent()
        {
            Entities.Add(new Entity
            {
                GL_VOUCHER = "FBillHead(GL_VOUCHER)",
                FAccountBookID = "FAccountBookID",
                FAccountBookID_Name = "FAccountBookID#Name",
                FDate = "FDate",
                FBUSDATE = "FBUSDATE",
                FYEAR = "FYEAR",
                FPERIOD = "FPERIOD",
                FVOUCHERGROUPID = "FVOUCHERGROUPID",
                FVOUCHERGROUPID_Name = "FVOUCHERGROUPID#Name",
                FVOUCHERGROUPNO = "FVOUCHERGROUPNO",
                FATTACHMENTS = "FATTACHMENTS",
                FISADJUSTVOUCHER = "FISADJUSTVOUCHER",
                FACCBOOKORGID = "FACCBOOKORGID",
                FACCBOOKORGID_Name = "FACCBOOKORGID#Name",
                FSourceBillKey = "FSourceBillKey",
                FSourceBillKey_Name = "FSourceBillKey#Name",
                FIMPORTVERSION = "FIMPORTVERSION",
                Split = "*Split*1",
                FEntity = "FEntity",
                FEXPLANATION = "FEXPLANATION",
                FAccountID = "FAccountID",
                FAccountID_Name = "FAccountID#Name",
                FDetailID_FF100006 = "FDetailID#FF100006",
                FDetailID_FF100006_Name = "FDetailID#FF100006#Name",
                FDetailID_FF100007 = "FDetailID#FF100007",
                FDetailID_FF100007_Name = "FDetailID#FF100007#Name",
                FDetailID_FF100005 = "FDetailID#FF100005",
                FDetailID_FF100005_Name = "FDetailID#FF100005#Name",
                FDetailID_FF100003 = "FDetailID#FF100003",
                FDetailID_FF100003_Name = "FDetailID#FF100003#Name",
                FDetailID_FF100004 = "FDetailID#FF100004",
                FDetailID_FF100004_Name = "FDetailID#FF100004#Name",
                FDetailID_FF100012 = "FDetailID#FF100012",
                FDetailID_FF100012_Name = "FDetailID#FF100012#Name",
                FDetailID_FF100014 = "FDetailID#FF100014",
                FDetailID_FF100014_Name = "FDetailID#FF100014#Name",
                FDetailID_FF100010 = "FDetailID#FF100010",
                FDetailID_FF100010_Name = "FDetailID#FF100010#Name",
                FDetailID_FF100008 = "FDetailID#FF100008",
                FDetailID_FF100008_Name = "FDetailID#FF100008#Name",
                FDetailID_FF100009 = "FDetailID#FF100009",
                FDetailID_FF100009_Name = "FDetailID#FF100009#Name",
                FDetailID_FFLEX13 = "FDetailID#FFLEX13",
                FDetailID_FFLEX13_Name = "FDetailID#FFLEX13#Name",
                FDetailID_FFlex6 = "FDetailID#FFlex6",
                FDetailID_FFlex6_Name = "FDetailID#FFlex6#Name",
                FDetailID_FFlex7 = "FDetailID#FFlex7",
                FDetailID_FFlex7_Name = "FDetailID#FFlex7#Name",
                FDetailID_FFlex5 = "FDetailID#FFlex5",
                FDetailID_FFlex5_Name = "FDetailID#FFlex5#Name",
                FDetailID_FFlex4 = "FDetailID#FFlex4",
                FDetailID_FFlex4_Name = "FDetailID#FFlex4#Name",
                FDetailID_FFLEX11 = "FDetailID#FFLEX11",
                FDetailID_FFLEX11_Name = "FDetailID#FFLEX11#Name",
                FDetailID_FFLEX12 = "FDetailID#FFLEX12",
                FDetailID_FFLEX12_Name = "FDetailID#FFLEX12#Name",
                FDetailID_FFlex10 = "FDetailID#FFlex10",
                FDetailID_FFlex10_Name = "FDetailID#FFlex10#Name",
                FDetailID_FFlex8 = "FDetailID#FFlex8",
                FDetailID_FFlex8_Name = "FDetailID#FFlex8#Name",
                FDetailID_FFLEX9 = "FDetailID#FFLEX9",
                FDetailID_FFLEX9_Name = "FDetailID#FFLEX9#Name",
                FCURRENCYID = "FCURRENCYID",
                FCURRENCYID_Name = "FCURRENCYID#Name",
                FEXCHANGERATETYPE = "FEXCHANGERATETYPE",
                FEXCHANGERATETYPE_Name = "FEXCHANGERATETYPE#Name",
                FEXCHANGERATE = "FEXCHANGERATE",
                FUnitId = "FUnitId",
                FUnitId_Name = "FUnitId#Name",
                FPrice = "FPrice",
                FQty = "FQty",
                FAMOUNTFOR = "FAMOUNTFOR",
                FDEBIT = "FDEBIT",
                FCREDIT = "FCREDIT",
                FSettleTypeID = "FSettleTypeID",
                FSettleTypeID_Name = "FSettleTypeID#Name",
                FSETTLENO = "FSETTLENO",
                FBUSNO = "FBUSNO",
                FEXPORTENTRYID = "FEXPORTENTRYID"
            });
            Entities.Add(new Entity
            {
                GL_VOUCHER = "*单据头(序号)",
                FAccountBookID = "*(单据头)账簿#编码",
                FAccountBookID_Name = "(单据头)账簿#名称",
                FDate = "*(单据头)日期",
                FBUSDATE = "(单据头)业务日期",
                FYEAR = "(单据头)会计年度",
                FPERIOD = "(单据头)期间",
                FVOUCHERGROUPID = "*(单据头)凭证字#编码",
                FVOUCHERGROUPID_Name = "(单据头)凭证字#名称",
                FVOUCHERGROUPNO = "*(单据头)凭证号",
                FATTACHMENTS = "(单据头)附件数",
                FISADJUSTVOUCHER = "(单据头)是否调整期凭证",
                FACCBOOKORGID = "(单据头)核算组织#编码",
                FACCBOOKORGID_Name = "(单据头)核算组织#名称",
                FSourceBillKey = "(单据头)业务类型#编码",
                FSourceBillKey_Name = "(单据头)业务类型#名称",
                FIMPORTVERSION = "(单据头)引入版本号",
                Split = "间隔列",
                FEntity = "*分录(序号)",
                FEXPLANATION = "(分录)摘要",
                FAccountID = "*(分录)科目编码#编码",
                FAccountID_Name = "(分录)科目编码#名称",
                FDetailID_FF100006 = "(分录)无形资产#编码",
                FDetailID_FF100006_Name = "(分录)无形资产#名称(Null)",
                FDetailID_FF100007 = "(分录)在建工程#编码",
                FDetailID_FF100007_Name = "(分录)在建工程#名称(Null)",
                FDetailID_FF100005 = "(分录)理财产品#编码",
                FDetailID_FF100005_Name = "(分录)理财产品#名称(Null)",
                FDetailID_FF100003 = "(分录)车牌#编码",
                FDetailID_FF100003_Name = "(分录)车牌#名称(Null)",
                FDetailID_FF100004 = "(分录)股东名录#编码",
                FDetailID_FF100004_Name = "(分录)股东名录#名称(Null)",
                FDetailID_FF100012 = "(分录)租赁项目#编码",
                FDetailID_FF100012_Name = "(分录)租赁项目#名称(Null)",
                FDetailID_FF100014 = "(分录)现金账号#编码",
                FDetailID_FF100014_Name = "(分录)现金账号#名称(Null)",
                FDetailID_FF100010 = "(分录)库存商品#编码",
                FDetailID_FF100010_Name = "(分录)库存商品#名称(Null)",
                FDetailID_FF100008 = "(分录)开票状态#编码",
                FDetailID_FF100008_Name = "(分录)开票状态#名称(Null)",
                FDetailID_FF100009 = "(分录)银行账号#编码",
                FDetailID_FF100009_Name = "(分录)银行账号#名称(Null)",
                FDetailID_FFLEX13 = "(分录)客户分组#编码",
                FDetailID_FFLEX13_Name = "(分录)客户分组#名称(Null)",
                FDetailID_FFlex6 = "(分录)客户#编码",
                FDetailID_FFlex6_Name = "(分录)客户#名称(Null)",
                FDetailID_FFlex7 = "(分录)员工#编码",
                FDetailID_FFlex7_Name = "(分录)员工#名称(Null)",
                FDetailID_FFlex5 = "(分录)部门#编码",
                FDetailID_FFlex5_Name = "(分录)部门#名称(Null)",
                FDetailID_FFlex4 = "(分录)供应商#编码",
                FDetailID_FFlex4_Name = "(分录)供应商#名称(Null)",
                FDetailID_FFLEX11 = "(分录)组织机构#编码",
                FDetailID_FFLEX11_Name = "(分录)组织机构#名称(Null)",
                FDetailID_FFLEX12 = "(分录)物料分组#编码",
                FDetailID_FFLEX12_Name = "(分录)物料分组#名称(Null)",
                FDetailID_FFlex10 = "(分录)资产类别#编码",
                FDetailID_FFlex10_Name = "(分录)资产类别#名称(Null)",
                FDetailID_FFlex8 = "(分录)物料#编码",
                FDetailID_FFlex8_Name = "(分录)物料#名称(Null)",
                FDetailID_FFLEX9 = "(分录)费用项目#编码",
                FDetailID_FFLEX9_Name = "(分录)费用项目#名称(Null)",
                FCURRENCYID = "*(分录)币别#编码",
                FCURRENCYID_Name = "(分录)币别#名称",
                FEXCHANGERATETYPE = "*(分录)汇率类型#编码",
                FEXCHANGERATETYPE_Name = "(分录)汇率类型#名称",
                FEXCHANGERATE = "(分录)汇率",
                FUnitId = "(分录)单位#编码",
                FUnitId_Name = "(分录)单位#名称",
                FPrice = "(分录)单价",
                FQty = "(分录)数量",
                FAMOUNTFOR = "(分录)原币金额",
                FDEBIT = "(分录)借方金额",
                FCREDIT = "(分录)贷方金额",
                FSettleTypeID = "(分录)结算方式#编码",
                FSettleTypeID_Name = "(分录)结算方式#名称",
                FSETTLENO = "(分录)结算号",
                FBUSNO = "(分录)业务编号",
                FEXPORTENTRYID = "(分录)现金流量#分录ID"
            });
            AccRules = new ObservableCollection<AccRule>(_repository.LoadData<AccRule>("AccRules"));
            AccDimension = new ObservableCollection<AccDimension>(_repository.LoadData<AccDimension>("AccDimension"));
        }

        public void AddToKIS(ObservableCollection<BillInfo> billInfo, VoucherInfo voucherInfo)
        {
            IsFinished = false;
            double DEBITTotal = 0, CREDITTotal = 0;
            foreach (var item in billInfo)
            {
                //单据头设置
                Entity entity = new Entity();
                if (IsHeader)
                {
                    entity.GL_VOUCHER = (10000 + VoucherIndex).ToString();
                    entity.FAccountBookID = voucherInfo.Book.Id;
                    entity.FDate = voucherInfo.Date.ToString("yyyy/MM/dd");
                    entity.FYEAR = voucherInfo.Date.Year.ToString();
                    entity.FPERIOD = voucherInfo.Date.Month.ToString();
                    entity.FVOUCHERGROUPID = voucherInfo.Voucher.Id;
                    entity.FVOUCHERGROUPNO = VoucherIndex.ToString();
                    entity.FACCBOOKORGID = voucherInfo.Book.Id;
                }
                IsHeader = false;
                entity.FEntity = RowIndex.ToString();

                var ruleResult = AccRules
                    .Where(b => b.AccName == item.AccType)
                    .Select(b => new
                    {
                        b.AccountID,
                        b.AccFDC,
                        b.AccFiexItem
                    }).ToList();

                if (ruleResult.Any())
                {
                    amount = item.AMOUNT.Round(2);
                    isDebit = ruleResult[0].AccFDC == "借方";      // 是否为借方

                    KeyStr = isDebit ? "付" : "收";

                    if (isDebit)
                    {
                        entity.FDEBIT = amount.ToString();
                        DEBITTotal += amount;
                    }
                    else
                    {
                        entity.FCREDIT = amount.ToString();
                        CREDITTotal += amount;
                    }

                    entity.FAccountID = ruleResult[0].AccountID;
                    AccFiexItem = ruleResult[0].AccFiexItem;
                }
                else
                {
                    MessageBox.Show(item.AccType + "没有设置核算规则");
                    return;

                }

                //摘要
                if (!string.IsNullOrWhiteSpace(item.AccNumber))
                {
                    item.AccNumber = "凭证号：" + item.AccNumber;
                }
                //如果核算类型已经有了收、付的关键字，就不要在摘要里写收付字样了
                if (item.AccType.StartsWith("收") || item.AccType.StartsWith("付"))
                {
                    KeyStr = string.Empty;
                }
                entity.FEXPLANATION = voucherInfo.Bank.DimensionName + KeyStr + item.DetailID_FFlex5 + item.DetailID_FFlex6 + item.AccType + item.AccNumber;

                //客户
                if (AccFiexItem.Contains("客户"))
                {
                    //如果核算维度有客户维度，判断客户字段是否为空，空值则按零星客户核算
                    if (string.IsNullOrWhiteSpace(item.DetailID_FFlex6))
                    {
                        KeyStr = "零星客户";
                    }
                    else
                    {
                        KeyStr = item.DetailID_FFlex6;
                    }
                    var result = AccDimension
                    .Where(b => b.DimensionType == "客户" && b.DimensionName == KeyStr)
                    .Select(b => new
                    {
                        b.DimensionNumber
                    }).ToList();

                    if (result.Any()) { entity.FDetailID_FFlex6 = result[0].DimensionNumber; }
                }

                //部门
                if (AccFiexItem.Contains("部门"))
                {
                    //如果核算维度有部门维度，判断部门字段是否为空，空值则按归集客户核算
                    if (string.IsNullOrWhiteSpace(item.DetailID_FFlex5))
                    {
                        KeyStr = "归集部门";
                    }
                    else
                    {
                        KeyStr = item.DetailID_FFlex5;
                    }
                    var result = AccDimension
                    .Where(b => b.DimensionType == "部门" && b.DimensionName == KeyStr)
                    .Select(b => new
                    {
                        b.DimensionNumber
                    }).ToList();

                    if (result.Any()) { entity.FDetailID_FFlex5 = result[0].DimensionNumber; }
                }

                //供应商
                if (AccFiexItem.Contains("供应商"))
                {

                    if (string.IsNullOrWhiteSpace(item.DetailID_FFlex4))
                    {
                        KeyStr = "零星供应商";
                    }
                    else
                    {
                        KeyStr = item.DetailID_FFlex4;
                    }
                    var result = AccDimension
                    .Where(b => b.DimensionType == "供应商" && b.DimensionName == KeyStr)
                    .Select(b => new
                    {
                        b.DimensionNumber
                    }).ToList();

                    if (result.Any()) { entity.FDetailID_FFlex4 = result[0].DimensionNumber; }
                }

                //费用项目
                if (AccFiexItem.Contains("费用项目"))
                {
                    KeyStr = item.DetailID_FFlex9;
                    var result = AccDimension
                    .Where(b => b.DimensionType == "费用项目" && b.DimensionName == KeyStr)
                    .Select(b => new
                    {
                        b.DimensionNumber
                    }).ToList();

                    if (result.Any()) { entity.FDetailID_FFLEX9 = result[0].DimensionNumber; }
                }

                entity.FCURRENCYID = "PRE001";
                entity.FEXCHANGERATETYPE = "HLTX01_SYS";

                //原币金额

                entity.FAMOUNTFOR = item.AMOUNT.Round(2).ToString();

                Entities.Add(entity);

                RowIndex++;
            }
            RowIndex++;
            Entity TotalEntity = new Entity();
            TotalEntity.FEntity = RowIndex.ToString();
            TotalEntity.FEXPLANATION = voucherInfo.Bank.DimensionName + voucherInfo.Date.ToString("yyyy/MM/dd") + "收支明细";
            TotalEntity.FAccountID = voucherInfo.Bank.AccID;

            //判断现金账户核算维度

            if (voucherInfo.Bank.BankDimension.Contains("银行账号")) { TotalEntity.FDetailID_FF100009 = voucherInfo.Bank.DimensionNumber; }
            if (voucherInfo.Bank.BankDimension.Contains("客户")) { TotalEntity.FDetailID_FFlex6 = voucherInfo.Bank.DimensionNumber; }
            if (voucherInfo.Bank.BankDimension.Contains("部门")) { TotalEntity.FDetailID_FFlex5 = voucherInfo.Bank.DimensionNumber; }
            if (voucherInfo.Bank.BankDimension.Contains("供应商")) { TotalEntity.FDetailID_FFlex4 = voucherInfo.Bank.DimensionNumber; }
            if (voucherInfo.Bank.BankDimension.Contains("费用项目")) { TotalEntity.FDetailID_FFLEX9 = voucherInfo.Bank.DimensionNumber; }
            TotalEntity.FCURRENCYID = "PRE001";
            TotalEntity.FEXCHANGERATETYPE = "HLTX01_SYS";
            TotalEntity.FAMOUNTFOR = Math.Abs(DEBITTotal - CREDITTotal).ToString();
            if (DEBITTotal > CREDITTotal)
            {
                TotalEntity.FCREDIT = TotalEntity.FAMOUNTFOR;
            }
            else
            {
                TotalEntity.FDEBIT = TotalEntity.FAMOUNTFOR;
            }
            Entities.Add(TotalEntity);

            IsFinished = true;
            IsHeader = true;


        }

        public bool IsFinished;

        private bool IsHeader =true;

        private bool isDebit;

        private double amount;

        private string KeyStr;

        private string AccFiexItem;
    }
}
