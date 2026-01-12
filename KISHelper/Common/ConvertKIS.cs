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
        public ObservableCollection<Entity>? Entities { get; set; }
        public void Clear()
        {
            VoucherIndex = 1;
            RowIndex = 1;
            Entities?.Clear();
            IsFinished = false;
            InitializeComponent();
            
        }

        public ConvertKIS()
        {
            Entities = new ObservableCollection<Entity>();
            InitializeComponent();

            List<AccDimension> listDimension = _repository.LoadData<AccDimension>("AccDimension");
            dicDimension = listDimension.ToDictionary(k => (k.Affiliated , k.DimensionType, k.DimensionName));

            List<AccRule> listRules = _repository.LoadData<AccRule>("AccRules");
            dicRules = listRules.ToDictionary(r => (r.Affiliated,r.AccName));

            KeyStr = string.Empty;
            AccFiexItem = string.Empty;
        }

        private readonly DataRepository _repository = new();

        private void InitializeComponent()
        {
            Entities?.Add(new Entity
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
            Entities?.Add(new Entity
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
        }

        public void AddToKIS(ObservableCollection<BillInfo> billInfo, VoucherInfo voucherInfo)
        {
            try
            {
                IsFinished = false;
                IsHeader = true;
                AddBankBillInfo(billInfo, voucherInfo);
                //验证内部往来
                VerifyInterior(billInfo, voucherInfo);
                foreach (var item in billInfo)
                {
                    Entity entity = new Entity();
                    WriteHeader(entity, voucherInfo);
                    entity.FEntity = RowIndex.ToString();
                    entity.FEXPLANATION = GetFEXPLANATION(item, voucherInfo);
                    entity.FAccountID = GetAccID(item, voucherInfo);//获取科目要放在获取核算维度之前，需要通过科目确认核算维度
                    entity.FDetailID_FFlex6 = GetCustom(item, voucherInfo);
                    entity.FDetailID_FFlex5 = GetDepartment(item, voucherInfo);
                    entity.FDetailID_FFlex4 = GetOrder(item, voucherInfo);
                    entity.FDetailID_FFLEX9 = GetExpense(item, voucherInfo);
                    entity.FDetailID_FF100009 = GetBank(item, voucherInfo);
                    entity.FCURRENCYID = "PRE001";
                    entity.FEXCHANGERATETYPE = "HLTX01_SYS";
                    amount = item.AMOUNT.Round(2);
                    entity.FAMOUNTFOR = amount.ToString();
                    if (item.BalanceDirection=="借方") 
                    { 
                        entity.FDEBIT = amount.ToString(); 
                    } 
                    else 
                    { 
                        entity.FCREDIT = amount.ToString(); 
                    }
                    Entities?.Add(entity);
                    RowIndex++;
                }

                IsFinished = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        //把银行信息添加到凭证里
        private void AddBankBillInfo(ObservableCollection<BillInfo> billInfo, VoucherInfo voucherInfo)
        {
            BillInfo bankinfo = new BillInfo();
            bankinfo.IsBankBillInfo = true;
            double DEBITTotal, CREDITTotal;
            DEBITTotal = billInfo.Where(b => b.BalanceDirection == "借方").Sum(b => b.AMOUNT);
            CREDITTotal = billInfo.Where(b => b.BalanceDirection == "贷方").Sum(b => b.AMOUNT);
            bankinfo.AMOUNT = Math.Abs(DEBITTotal - CREDITTotal);
            //如果借贷方余额相等，就没有银行信息
            if (bankinfo.AMOUNT == 0) return;
            bankinfo.BalanceDirection = DEBITTotal < CREDITTotal ? "借方" : "贷方";
            //资金调拨判断
            if (billInfo.Count == 1 && billInfo.First().IsBankBillInfo == true) bankinfo.BillWayNumber = "资金调拨";

            if (voucherInfo.Bank != null)
            {
                bankinfo.AccType = voucherInfo.Bank.AccName;
                bankinfo.DetailID_FFlex6 = voucherInfo.Bank.DimensionName;
                bankinfo.DetailID_FFlex5 = voucherInfo.Bank.Branch;
                bankinfo.DetailID_FFlex4 = voucherInfo.Bank.DimensionName;
                bankinfo.DetailID_FF100009 = voucherInfo.Bank.DimensionName;
            }

            billInfo.Add(bankinfo);
        }

        private void WriteHeader(Entity entity,VoucherInfo voucherInfo)
        {
            if (IsHeader)
            {
                entity.GL_VOUCHER = (10000 + VoucherIndex).ToString();
                entity.FAccountBookID = voucherInfo?.Book?.Id;
                entity.FDate = voucherInfo?.Date.ToString("yyyy/MM/dd");
                entity.FYEAR = voucherInfo?.Date.Year.ToString();
                entity.FPERIOD = voucherInfo?.Date.Month.ToString();
                entity.FVOUCHERGROUPID = voucherInfo?.Voucher?.Id;
                entity.FVOUCHERGROUPNO = VoucherIndex.ToString();
                entity.FACCBOOKORGID = voucherInfo?.Book?.Id;
            }
            IsHeader = false;
        }

        private string? GetFEXPLANATION(BillInfo item,VoucherInfo voucherInfo)
        {
            if (!string.IsNullOrWhiteSpace(item.FEXPLANATION))
            {
                return item.FEXPLANATION;
            }

            if (!string.IsNullOrWhiteSpace(item.AccNumber))
            {
                item.AccNumber = "凭证号：" + item.AccNumber;
            }
            else
            {
                item.AccNumber = "金额："+item.AMOUNT;
            }
            //如果核算类型已经有了收、付的关键字，就不要在摘要里写收付字样了

            if (item.IsBankBillInfo)
            {
                KeyStr = item.BalanceDirection == "借方" ? "收" : "付";
            }
            else
            {
                KeyStr = item.BalanceDirection == "借方" ? "付" : "收";
            }
            
            if (!string.IsNullOrEmpty(item.AccType) &&
                (item.AccType.StartsWith("收") || item.AccType.StartsWith("付")))
            {
                KeyStr = string.Empty;
            }
            
            if(item.BillWayNumber=="资金调拨")
            {
                return KeyStr = voucherInfo?.Bank?.DimensionName+"资金调拨";
            }
            if(item.IsBankBillInfo)
            {
                return KeyStr = voucherInfo?.Bank?.DimensionName + KeyStr +"款汇总," + item.AccNumber;
            }
            else
            {
                return KeyStr = voucherInfo?.Bank?.DimensionName + KeyStr + item.DetailID_FFlex5 + item.DetailID_FFlex6 + item.AccType + item.AccNumber;
            }
        }

        private string? GetAccID(BillInfo item,VoucherInfo info)
        {
            if (dicRules.TryGetValue((info.Book?.Name,item.AccType), out var r))
            {
                AccFiexItem = r.AccFiexItem;
                return r.AccountID;
            }
            else
            {
                MessageBox.Show(item.AccType + "没有设置核算规则");
                return null;

            }
        }

        private string? GetCustom(BillInfo item, VoucherInfo info)
        {
            if (AccFiexItem!.Contains("客户"))
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
                string key = item.IsBankBillInfo ? "银行账号" : "客户";
                if (dicDimension.TryGetValue((info.Book?.Name, key, KeyStr), out var c))
                    return c.DimensionNumber;
            }
            return null;
        }

        private string? GetDepartment(BillInfo item, VoucherInfo info)
        {
            //部门
            if (AccFiexItem!.Contains("部门"))
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
                if (dicDimension.TryGetValue((info.Book?.Name, "部门", KeyStr), out var d))
                    return d.DimensionNumber;
            }
            return null;
        }

        private string? GetOrder(BillInfo item, VoucherInfo info)
        {
            if (AccFiexItem!.Contains("供应商"))
            {

                if (string.IsNullOrWhiteSpace(item.DetailID_FFlex4))
                {
                    KeyStr = "零星供应商";
                }
                else
                {
                    KeyStr = item.DetailID_FFlex4;
                }
                string key = item.IsBankBillInfo ? "银行账号" : "供应商";
                if (dicDimension.TryGetValue((info.Book?.Name, key, KeyStr), out var o))
                    return o.DimensionNumber;
            }
            return null;
        }

        private string? GetExpense(BillInfo item, VoucherInfo info)
        {
            if (AccFiexItem!.Contains("费用项目"))
            {
                KeyStr = item.DetailID_FFlex9 ?? string.Empty;
                if (dicDimension.TryGetValue((info.Book?.Name, "费用项目", KeyStr), out var c))
                    return c.DimensionNumber;
            }
            return null;
        }

        private string? GetBank(BillInfo item, VoucherInfo info)
        {
            if (AccFiexItem!.Contains("银行账号"))
            {
                KeyStr = item.DetailID_FF100009 ?? string.Empty;
                if (dicDimension.TryGetValue((info.Book?.Name, "银行账号", KeyStr), out var c))
                    return c.DimensionNumber;
            }
            return null;
        }


        //验证内部往来
        private void VerifyInterior(ObservableCollection<BillInfo> billInfo,VoucherInfo voucherInfo)
        {
            //验证银行账号是否设置往来科目
            if (string.IsNullOrWhiteSpace(voucherInfo?.Bank?.Branch))
                return;

            //根据字典匹配往来维度编码，然后筛选部门与银行维度编码不一致的集合并汇总。
            string? branchInterior = dicDimension.TryGetValue((voucherInfo.Book?.Name,"部门", voucherInfo.Bank.Branch), out var bInfo)
                      ? bInfo.Interior
                      : null;

            var summary =
                from b in billInfo
                let key = (voucherInfo.Book?.Name, "部门", b.DetailID_FFlex5)
                where dicDimension.TryGetValue(key, out var rowInfo) &&   // 当前行能查到
                      branchInterior != null &&                          // 银行分行也能查到
                      !string.Equals(rowInfo.Interior, branchInterior,   // 两个 Interior 不相等
                                     StringComparison.OrdinalIgnoreCase)
                group b by new { b.DetailID_FFlex5, b.BalanceDirection } into g
                select new BillInfo
                {
                    AccType = "一般往来",
                    DetailID_FFlex5 = g.Key.DetailID_FFlex5,
                    BalanceDirection = g.Key.BalanceDirection,
                    AMOUNT = g.Sum(x => x.AMOUNT)
                };

            //没有内容即都是一个主体内部的，不需要做往来挂账
            if (!summary.Any())
                return;

            string wlNumber = DateTime.Now.ToString("yyyyMMddHHmmss");

            foreach(var item in summary)
            {
                for(int i=0; i<=1; i++)
                {
                    BillInfo bill = new BillInfo();
                    bill.AccNumber = wlNumber;
                    bill.AccType = item.AccType;
                    bill.DetailID_FFlex5 = i == 0 ? item.DetailID_FFlex5 : voucherInfo.Bank.Branch;
                    if (dicDimension.TryGetValue((voucherInfo.Book?.Name, "部门", bill.DetailID_FFlex5), out var Int))
                    {
                        bill.DetailID_FFlex6 = Int.Interior;
                    }
                    if (i == 0)
                    {
                        bill.BalanceDirection = item.BalanceDirection;
                    }
                    else
                    {
                        bill.BalanceDirection = item.BalanceDirection == "借方" ? "贷方" : "借方";
                    }
                    bill.AMOUNT = item.AMOUNT;
                    billInfo.Add(bill);
                }
                
            }
        }


        public bool IsFinished;

        private bool IsHeader =true;

        private double amount;

        private string? KeyStr;

        private string? AccFiexItem;

        private Dictionary<(string? Affiliated, string? DimensionType, string? DimensionName), AccDimension> dicDimension;

        private Dictionary<(string? Affiliated, string? AccName), AccRule> dicRules;
    }
}
