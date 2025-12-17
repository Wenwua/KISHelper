using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KISHelper.Common.Npoi;

namespace KISHelper.Common
{
    public class Entity
    {
        [ExcelColumn("GL_VOUCHER", Order = 1)]
        public string GL_VOUCHER { get; set; }
        [ExcelColumn("FAccountBookID", Order = 2)]
        public string FAccountBookID { get; set; }
        [ExcelColumn("FAccountBookID_Name", Order = 3)]
        public string FAccountBookID_Name { get; set; }
        [ExcelColumn("FDate", Order = 4)]
        public string FDate { get; set; }
        [ExcelColumn("FBUSDATE", Order = 5)]
        public string FBUSDATE { get; set; }
        [ExcelColumn("FYEAR", Order = 6)]
        public string FYEAR { get; set; }
        [ExcelColumn("FPERIOD", Order = 7)]
        public string FPERIOD { get; set; }
        [ExcelColumn("FVOUCHERGROUPID", Order = 8)]
        public string FVOUCHERGROUPID { get; set; }
        [ExcelColumn("FVOUCHERGROUPID_Name", Order = 9)]
        public string FVOUCHERGROUPID_Name { get; set; }
        [ExcelColumn("FVOUCHERGROUPNO", Order = 10)]
        public string FVOUCHERGROUPNO { get; set; }
        [ExcelColumn("FATTACHMENTS", Order = 11)]
        public string FATTACHMENTS { get; set; }
        [ExcelColumn("FISADJUSTVOUCHER", Order = 12)]
        public string FISADJUSTVOUCHER { get; set; }
        [ExcelColumn("FACCBOOKORGID", Order = 13)]
        public string FACCBOOKORGID { get; set; }
        [ExcelColumn("FACCBOOKORGID_Name", Order = 14)]
        public string FACCBOOKORGID_Name { get; set; }
        [ExcelColumn("FSourceBillKey", Order = 15)]
        public string FSourceBillKey { get; set; }
        [ExcelColumn("FSourceBillKey_Name", Order = 16)]
        public string FSourceBillKey_Name { get; set; }
        [ExcelColumn("FIMPORTVERSION", Order = 17)]
        public string FIMPORTVERSION { get; set; }
        [ExcelColumn("Split", Order = 18)]
        public string Split { get; set; }
        [ExcelColumn("FEntity", Order = 19)]
        public string FEntity { get; set; }
        [ExcelColumn("FEXPLANATION", Order = 20)]
        public string FEXPLANATION { get; set; }
        [ExcelColumn("FAccountID", Order = 21)]
        public string FAccountID { get; set; }
        [ExcelColumn("FAccountID_Name", Order = 22)]
        public string FAccountID_Name { get; set; }
        [ExcelColumn("FDetailID_FF100006", Order = 23)]
        public string FDetailID_FF100006 { get; set; }
        [ExcelColumn("FDetailID_FF100006_Name", Order = 24)]
        public string FDetailID_FF100006_Name { get; set; }
        [ExcelColumn("FDetailID_FF100007", Order = 25)]
        public string FDetailID_FF100007 { get; set; }
        [ExcelColumn("FDetailID_FF100007_Name", Order = 26)]
        public string FDetailID_FF100007_Name { get; set; }
        [ExcelColumn("FDetailID_FF100005", Order = 27)]
        public string FDetailID_FF100005 { get; set; }
        [ExcelColumn("FDetailID_FF100005_Name", Order = 28)]
        public string FDetailID_FF100005_Name { get; set; }
        [ExcelColumn("FDetailID_FF100003", Order = 29)]
        public string FDetailID_FF100003 { get; set; }
        [ExcelColumn("FDetailID_FF100003_Name", Order = 30)]
        public string FDetailID_FF100003_Name { get; set; }
        [ExcelColumn("FDetailID_FF100004", Order = 31)]
        public string FDetailID_FF100004 { get; set; }
        [ExcelColumn("FDetailID_FF100004_Name", Order = 32)]
        public string FDetailID_FF100004_Name { get; set; }
        [ExcelColumn("FDetailID_FF100012", Order = 33)]
        public string FDetailID_FF100012 { get; set; }
        [ExcelColumn("FDetailID_FF100012_Name", Order = 34)]
        public string FDetailID_FF100012_Name { get; set; }
        [ExcelColumn("FDetailID_FF100014", Order = 35)]
        public string FDetailID_FF100014 { get; set; }
        [ExcelColumn("FDetailID_FF100014_Name", Order = 36)]
        public string FDetailID_FF100014_Name { get; set; }
        [ExcelColumn("FDetailID_FF100010", Order = 37)]
        public string FDetailID_FF100010 { get; set; }
        [ExcelColumn("FDetailID_FF100010_Name", Order = 38)]
        public string FDetailID_FF100010_Name { get; set; }
        [ExcelColumn("FDetailID_FF100008", Order = 39)]
        public string FDetailID_FF100008 { get; set; }
        [ExcelColumn("FDetailID_FF100008_Name", Order = 40)]
        public string FDetailID_FF100008_Name { get; set; }
        [ExcelColumn("FDetailID_FF100009", Order = 41)]
        public string FDetailID_FF100009 { get; set; }
        [ExcelColumn("FDetailID_FF100009_Name", Order = 42)]
        public string FDetailID_FF100009_Name { get; set; }
        [ExcelColumn("FDetailID_FFLEX13", Order = 43)]
        public string FDetailID_FFLEX13 { get; set; }
        [ExcelColumn("FDetailID_FFLEX13_Name", Order = 44)]
        public string FDetailID_FFLEX13_Name { get; set; }
        [ExcelColumn("FDetailID_FFlex6", Order = 45)]
        public string FDetailID_FFlex6 { get; set; }
        [ExcelColumn("FDetailID_FFlex6_Name", Order = 46)]
        public string FDetailID_FFlex6_Name { get; set; }
        [ExcelColumn("FDetailID_FFlex7", Order = 47)]
        public string FDetailID_FFlex7 { get; set; }
        [ExcelColumn("FDetailID_FFlex7_Name", Order = 48)]
        public string FDetailID_FFlex7_Name { get; set; }
        [ExcelColumn("FDetailID_FFlex5", Order = 49)]
        public string FDetailID_FFlex5 { get; set; }
        [ExcelColumn("FDetailID_FFlex5_Name", Order = 50)]
        public string FDetailID_FFlex5_Name { get; set; }
        [ExcelColumn("FDetailID_FFlex4", Order = 51)]
        public string FDetailID_FFlex4 { get; set; }
        [ExcelColumn("FDetailID_FFlex4_Name", Order = 52)]
        public string FDetailID_FFlex4_Name { get; set; }
        [ExcelColumn("FDetailID_FFLEX11", Order = 53)]
        public string FDetailID_FFLEX11 { get; set; }
        [ExcelColumn("FDetailID_FFLEX11_Name", Order = 54)]
        public string FDetailID_FFLEX11_Name { get; set; }
        [ExcelColumn("FDetailID_FFLEX12", Order = 55)]
        public string FDetailID_FFLEX12 { get; set; }
        [ExcelColumn("FDetailID_FFLEX12_Name", Order = 56)]
        public string FDetailID_FFLEX12_Name { get; set; }
        [ExcelColumn("FDetailID_FFlex10", Order = 57)]
        public string FDetailID_FFlex10 { get; set; }
        [ExcelColumn("FDetailID_FFlex10_Name", Order = 58)]
        public string FDetailID_FFlex10_Name { get; set; }
        [ExcelColumn("FDetailID_FFlex8", Order = 59)]
        public string FDetailID_FFlex8 { get; set; }
        [ExcelColumn("FDetailID_FFlex8_Name", Order = 60)]
        public string FDetailID_FFlex8_Name { get; set; }
        [ExcelColumn("FDetailID_FFLEX9", Order = 61)]
        public string FDetailID_FFLEX9 { get; set; }
        [ExcelColumn("FDetailID_FFLEX9_Name", Order = 62)]
        public string FDetailID_FFLEX9_Name { get; set; }
        [ExcelColumn("FCURRENCYID", Order = 63)]
        public string FCURRENCYID { get; set; }
        [ExcelColumn("FCURRENCYID_Name", Order = 64)]
        public string FCURRENCYID_Name { get; set; }
        [ExcelColumn("FEXCHANGERATETYPE", Order = 65)]
        public string FEXCHANGERATETYPE { get; set; }
        [ExcelColumn("FEXCHANGERATETYPE_Name", Order = 66)]
        public string FEXCHANGERATETYPE_Name { get; set; }
        [ExcelColumn("FEXCHANGERATE", Order = 67)]
        public string FEXCHANGERATE { get; set; }
        [ExcelColumn("FUnitId", Order = 68)]
        public string FUnitId { get; set; }
        [ExcelColumn("FUnitId_Name", Order = 69)]
        public string FUnitId_Name { get; set; }
        [ExcelColumn("FPrice", Order = 70)]
        public string FPrice { get; set; }
        [ExcelColumn("FQty", Order = 71)]
        public string FQty { get; set; }
        [ExcelColumn("FAMOUNTFOR", Order = 72)]
        public string FAMOUNTFOR { get; set; }
        [ExcelColumn("FDEBIT", Order = 73)]
        public string FDEBIT { get; set; }
        [ExcelColumn("FCREDIT", Order = 74)]
        public string FCREDIT { get; set; }
        [ExcelColumn("FSettleTypeID", Order = 75)]
        public string FSettleTypeID { get; set; }
        [ExcelColumn("FSettleTypeID_Name", Order = 76)]
        public string FSettleTypeID_Name { get; set; }
        [ExcelColumn("FSETTLENO", Order = 77)]
        public string FSETTLENO { get; set; }
        [ExcelColumn("FBUSNO", Order = 78)]
        public string FBUSNO { get; set; }
        [ExcelColumn("FEXPORTENTRYID", Order = 79)]
        public string FEXPORTENTRYID { get; set; }


    }
}
