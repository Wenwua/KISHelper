using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.Common
{
    public class Npoi
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class ExcelColumnAttribute : Attribute
        {
            public string ColumnName { get; }
            public int Order { get; set; } = 999;
            public string Format { get; set; }

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public ExcelColumnAttribute(string columnName)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            {
                ColumnName = columnName;
            }
        }

        public static class NpoiHelper
        {

            /// <summary>
            /// 从Excel读取数据
            /// </summary>
            public static List<T> ReadFromExcel<T>(string filePath, int sheetIndex = 0, int headerRowIndex = 0)
                where T : new()
            {
                return ReadExcelInternal<T>(filePath, sheetIndex, null, headerRowIndex);
            }

            /// <summary>
            /// 从Excel读取数据（按Sheet名称）
            /// </summary>
            public static List<T> ReadFromExcel<T>(string filePath, string sheetName, int headerRowIndex = 0)
                where T : new()
            {
                if (string.IsNullOrWhiteSpace(sheetName))
                    throw new ArgumentException("Sheet名称不能为空", nameof(sheetName));

                return ReadExcelInternal<T>(filePath, -1, sheetName, headerRowIndex);
            }

            /// <summary>
            /// 写入数据到Excel
            /// </summary>
            public static void WriteToExcel<T>(List<T> data, string filePath, string sheetName = "Sheet1")
            {
                try
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet(sheetName);

                    // 获取并排序所有带ExcelColumn特性的属性
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    var properties = typeof(T).GetProperties()
                        .Select(p => new { Prop = p, Attr = p.GetCustomAttribute<ExcelColumnAttribute>() })
                        .Where(p => p.Attr != null)
                        .OrderBy(p => p.Attr.Order)
                        .ToList();
#pragma warning restore CS8602 // 解引用可能出现空引用。

                    // 创建标题行,引入格式有特定要求，手动创建标题行
                    //var headerRow = sheet.CreateRow(0);
                    //for (int i = 0; i < properties.Count; i++)
                    //{headerRow.CreateCell(i).SetCellValue(properties[i].Attr.ColumnName);}

                    // 写入数据行
                    for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
                    {
                        var dataRow = sheet.CreateRow(rowIndex);
                        var item = data[rowIndex];

                        for (int colIndex = 0; colIndex < properties.Count; colIndex++)
                        {
                            var propInfo = properties[colIndex];
                            var value = propInfo.Prop.GetValue(item);

#pragma warning disable CS8602 // 解引用可能出现空引用。
                            WriteCellValue(dataRow.CreateCell(colIndex), value, propInfo.Attr.Format);
#pragma warning restore CS8602 // 解引用可能出现空引用。
                        }
                    }

                    // 自动调整列宽
                    for (int i = 0; i < properties.Count; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }

                    // 写入文件
                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                    }
                    IsExported = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    IsExported = false;
                }

            }

            public static bool IsExported;
            // ========= 私有辅助方法 =========

            private static IWorkbook GetWorkbook(Stream stream, string filePath)
            {
                return Path.GetExtension(filePath).ToLower() == ".xls"
                    ? (IWorkbook)new HSSFWorkbook(stream)
                    : new XSSFWorkbook(stream);
            }

            private static int FindColumnIndex(IRow headerRow, string columnName)
            {
                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    if (headerRow.GetCell(i)?.ToString()?.Trim() == columnName)
                        return i;
                }
                return -1;
            }

            private static void SetPropertyValue<T>(T item, PropertyInfo prop, ICell cell)
            {
                if (cell == null) return;

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                object value = cell.CellType switch
                {
                    CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
                        ? cell.DateCellValue
                        : cell.NumericCellValue,
                    CellType.Boolean => cell.BooleanCellValue,
                    _ => cell.StringCellValue
                };
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

                try
                {
                    // 类型转换（处理可空类型）
                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(item, value?.ToString());
                    }
                    else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                    {
                        prop.SetValue(item, Convert.ToDouble(value));
                    }
                    else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        prop.SetValue(item, Convert.ToDateTime(value));
                    }
                    // 可继续扩展其他类型...
                }
                catch { /* 转换失败时忽略 */ }
            }

            private static void WriteCellValue(ICell cell, object value, string format)
            {
                if (value == null) return;

                switch (value)
                {
                    case string str:
                        cell.SetCellValue(str);
                        break;
                    case double dbl:
                        cell.SetCellValue(dbl);
                        break;
                    case DateTime dt:
                        cell.SetCellValue(dt);
                        if (!string.IsNullOrEmpty(format))
                        {
                            var cellStyle = cell.Sheet.Workbook.CreateCellStyle();
                            var dataFormat = cell.Sheet.Workbook.CreateDataFormat();
                            cellStyle.DataFormat = dataFormat.GetFormat(format);
                            cell.CellStyle = cellStyle;
                        }
                        break;
                    default:
                        cell.SetCellValue(value.ToString());
                        break;
                }
            }

            private static List<T> ReadExcelInternal<T>(string filePath, int sheetIndex, string? sheetName, int headerRowIndex)
    where T : new()
            {
                var result = new List<T>();

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = GetWorkbook(fs, filePath);

                    // 🔥 智能获取工作表（关键代码）
                    ISheet sheet = GetSheet(workbook, sheetIndex, sheetName);
                    if (sheet == null)
                    {
                        throw new ArgumentException(
                            $"Sheet '{sheetName ?? sheetIndex.ToString()}' 不存在，请检查Excel文件");
                    }

                    // 以下是读取逻辑
                    var headerRow = sheet.GetRow(headerRowIndex);
                    var propertyMap = new Dictionary<int, PropertyInfo>();

                    foreach (var prop in typeof(T).GetProperties())
                    {
                        var attr = prop.GetCustomAttribute<ExcelColumnAttribute>();
                        if (attr != null)
                        {
                            int columnIndex = FindColumnIndex(headerRow, attr.ColumnName);
                            if (columnIndex != -1)
                            {
                                propertyMap[columnIndex] = prop;
                            }
                        }
                    }

                    // 读取数据行
                    for (int rowIndex = headerRowIndex + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var dataRow = sheet.GetRow(rowIndex);
                        if (dataRow == null) continue;

                        var item = new T();
                        foreach (var kvp in propertyMap)
                        {
                            var cell = dataRow.GetCell(kvp.Key);
                            SetPropertyValue(item, kvp.Value, cell);
                        }
                        result.Add(item);
                    }
                }
                return result;
            }

            private static ISheet GetSheet(IWorkbook workbook, int sheetIndex, string sheetName)
            {
                // 优先级：sheetName > sheetIndex
                if (!string.IsNullOrEmpty(sheetName))
                {
                    return workbook.GetSheet(sheetName); // 返回null如果找不到
                }

                if (sheetIndex >= 0 && sheetIndex < workbook.NumberOfSheets)
                {
                    return workbook.GetSheetAt(sheetIndex);
                }

#pragma warning disable CS8603 // 可能返回 null 引用。
                return null; // 两个参数都无效
#pragma warning restore CS8603 // 可能返回 null 引用。
            }

            // 🔥 新增委托：用于弹出选择对话框
            public static Func<string, List<string>, string> SheetSelectorCallback { get; set; }

            private static ISheet GetSheetWithFallback(IWorkbook workbook, string sheetName, string filePath)
            {
                // 尝试获取工作表
                var sheet = workbook.GetSheet(sheetName);
                if (sheet != null) return sheet;

                // 工作表不存在，触发回调
                if (SheetSelectorCallback != null)
                {
                    // 获取所有工作表名称
                    var allSheets = new List<string>();
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                    {
                        allSheets.Add(workbook.GetSheetName(i));
                    }

                    // 弹出对话框让用户选择
                    var selectedSheet = SheetSelectorCallback(filePath, allSheets);

                    // 用户取消选择
                    if (string.IsNullOrEmpty(selectedSheet))
                    {
                        return null; // 返回null，外层会处理
                    }

                    // 使用用户选择的工作表
                    return workbook.GetSheet(selectedSheet);
                }

                return null;
            }
        }

    }
}
