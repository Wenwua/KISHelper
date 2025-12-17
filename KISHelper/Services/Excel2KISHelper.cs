using KISHelper.Common;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.Services
{
    public class Excel2KISHelper
    {
        public static void Explor(string path)
        {
            
        }

        public static void ExcelToKIS(ObservableCollection<Entity> entities,ObservableCollection<BillInfo>billInfo,ObservableCollection<AccRule> accRules, ObservableCollection<AccDimension> accDimensions,
            string BookId, string VoucherId, string BankId,DateTime AccDate, int irow, ref int count,ref bool isdone)
        {

            double DEBITTotal=0, CREDITTotal=0;
            double amount ;
            bool isDebit ;  
            bool isPositive;

            if (count == 1)
            {
                InitializeComponent(ref entities);
            }
            int iStart=1;
            string KeyStr,BankName=string.Empty, AccFiexItem=string.Empty, 
                DetailID_FFlex6=string.Empty,DetailID_FFlex5 = string.Empty, 
                DetailID_FFlex4=string.Empty,BankAccID = string.Empty;

            //查询银行信息
            var bankResult = accDimensions
                    .Where(b => b.DimensionNumber == BankId)
                    .Select(b => new
                    {
                        b.AccID,
                        b.DimensionName
                    }).ToList();
            if (bankResult.Any())
            {
                BankName = bankResult[0].DimensionName;
                BankAccID= bankResult[0].AccID;
            }

            foreach (var item in billInfo)
            {
                irow++;
                Entity entity = new Entity();
                //单据头设置
                if (iStart == 1)
                {
                    entity.GL_VOUCHER = (10000 + count).ToString();
                    entity.FAccountBookID = BookId;
                    entity.FDate = AccDate.ToString("yyyy/MM/dd");
                    entity.FYEAR = AccDate.Year.ToString();
                    entity.FPERIOD = AccDate.Month.ToString();
                    entity.FVOUCHERGROUPID = VoucherId;
                    entity.FVOUCHERGROUPNO = count.ToString();
                    entity.FACCBOOKORGID = BookId;
                }

                //单据体设置
                entity.FEntity = irow.ToString();
                
                //查询核算规则
                var ruleResult = accRules
                    .Where(b => b.AccName == item.AccType)
                    .Select(b => new 
                    {
                        b.AccountID,
                        b.AccFDC,
                        b.AccFiexItem
                    }).ToList(); ;

                if(ruleResult.Any())
                {
                    amount = item.AMOUNT.Round(2);
                    isDebit = ruleResult[0].AccFDC == "借方";      // 是否为借方

                    KeyStr = isDebit? "付" : "收";

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
                entity.FEXPLANATION = BankName + KeyStr+item.DetailID_FFlex5+item.DetailID_FFlex6+item.AccType+item.AccNumber;


                //客户
                if (AccFiexItem.Contains("客户"))
                {
                    //如果核算维度有客户维度，判断客户字段是否为空，空值则按零星客户核算
                    if (string.IsNullOrWhiteSpace(item.DetailID_FFlex6))
                    {
                        DetailID_FFlex6 = "零星客户";
                    }
                    else
                    {
                        DetailID_FFlex6 = item.DetailID_FFlex6;
                    }
                    var result = accDimensions
                    .Where(b => b.DimensionType == "客户" && b.DimensionName == DetailID_FFlex6)
                    .Select(b => new
                    {
                        b.DimensionNumber
                    }).ToList();

                    if (result.Any()) { entity.FDetailID_FFlex6 = result[0].DimensionNumber; }

                }


                //部门
                if (AccFiexItem.Contains("部门"))
                {
                    //如果核算维度有客户维度，判断客户字段是否为空，空值则按零星客户核算
                    if (string.IsNullOrWhiteSpace(item.DetailID_FFlex5))
                    {
                        DetailID_FFlex5 = "归集部门";
                    }
                    else
                    {
                        DetailID_FFlex5 = item.DetailID_FFlex5;
                    }
                    var result = accDimensions
                    .Where(b => b.DimensionType == "部门" && b.DimensionName == DetailID_FFlex5)
                    .Select(b => new
                    {
                        b.DimensionNumber
                    }).ToList();

                    if (result.Any()) { entity.FDetailID_FFlex5 = result[0].DimensionNumber; }

                }

                //供应商
                if (AccFiexItem.Contains("供应商"))
                {
                    //如果核算维度有客户维度，判断客户字段是否为空，空值则按零星客户核算
                    if (string.IsNullOrWhiteSpace(item.DetailID_FFlex4))
                    {
                        DetailID_FFlex4 = "零星供应商";
                    }
                    else
                    {
                        DetailID_FFlex4 = item.DetailID_FFlex4;
                    }
                    var result = accDimensions
                    .Where(b => b.DimensionType == "供应商" && b.DimensionName == DetailID_FFlex4)
                    .Select(b => new
                    {
                        b.DimensionNumber
                    }).ToList();

                    if (result.Any()) { entity.FDetailID_FFlex4 = result[0].DimensionNumber; }

                }

                entity.FCURRENCYID = "PRE001";
                entity.FEXCHANGERATETYPE = "HLTX01_SYS";

                //原币金额

                entity.FAMOUNTFOR = item.AMOUNT.Round(2).ToString();

                entities.Add(entity);

                iStart++;
                

            }

            irow++;
            //现金账户
            Entity TotalEntity = new Entity();
            TotalEntity.FEntity = irow.ToString();
            TotalEntity.FEXPLANATION = BankName+ AccDate.ToString("yyyy/MM/dd") +"收支明细";
            TotalEntity.FAccountID = BankAccID;
            TotalEntity.FDetailID_FF100009 = BankId;
            TotalEntity.FCURRENCYID = "PRE001";
            TotalEntity.FEXCHANGERATETYPE = "HLTX01_SYS";
            TotalEntity.FAMOUNTFOR = Math.Abs(DEBITTotal - CREDITTotal).ToString();
            if(DEBITTotal > CREDITTotal)
            {
                TotalEntity.FCREDIT = TotalEntity.FAMOUNTFOR;
            }
            else
            {
                TotalEntity.FDEBIT = TotalEntity.FAMOUNTFOR;
            }
            entities.Add(TotalEntity);
            isdone = true;
        }

        private static void InitializeComponent(ref ObservableCollection<Entity> entities)
        {
            entities.Add(new Entity
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
            entities.Add(new Entity
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
    }
}
