using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using KISHelper.Views.Dialog;
using System.Collections.ObjectModel;
using System.IO;
using System.Globalization;
using System.Windows.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using static KISHelper.Common.Npoi;
using System.Reflection;
using NPOI.SS.Formula.Functions;

namespace KISHelper.ViewModels
{
    public class MergeDataViewModel : ViewModelBase
    {
        #region 属性
        public ObservableCollection<SheetStructure> SheetStructures { get; set; } = new();
        private SheetStructure? selectedItem;
        public SheetStructure? SectedItem
        {
            get => selectedItem;
            set => SetField(ref selectedItem, value);
        }

        private CancellationTokenSource? _cts;          // 取消令牌
        private bool _isRunning;                        // 防重入
        private int _progress;                            // 0-100

        public int Progress
        {
            get => _progress;
            set => SetField(ref _progress, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetField(ref _isRunning, value);
        }
        #endregion

        #region 命令
        public RelayCommand SelectPathCommand { get; }
        public RelayCommand AddRowCommand { get; }
        public RelayCommand DeleteSelectedRowsCommand { get; }
        public RelayCommand MergeCommand { get; }
        #endregion

        #region 构造
        public MergeDataViewModel()
        {
            SelectPathCommand = new RelayCommand(SelectPath);
            AddRowCommand = new RelayCommand(Add);
            DeleteSelectedRowsCommand = new RelayCommand(Delete);
            var lists = _repository.LoadData<SheetStructure>("SheetStructures").ToList();
            SheetStructures = new ObservableCollection<SheetStructure>(lists);
            MergeCommand = new RelayCommand(MergeData);
        }
        #endregion

        #region 字段/仓库
        private readonly DataRepository _repository = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SheetStructure.json"));
        public List<string> CompareOperators => new() { "包含", "等于" };
        private string? forderPath;
        #endregion

        #region 方法
        private void SelectPath()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == true) forderPath = dialog.SelectedPath;
        }

        private void Add() => SheetStructures.Add(new SheetStructure { IsContains = true });

        private void Delete()
        {
            if (selectedItem != null) SheetStructures.Remove(selectedItem);
            SaveData();
        }

        private void SaveData() => _repository.SaveData("SheetStructures", SheetStructures.ToList());

        //==================== 合并 + 导出 ====================
        private async void MergeData()   // async void 仅用于事件/命令
        {
            if (IsRunning) return;        // 防重入
            if (string.IsNullOrEmpty(forderPath) || !Directory.Exists(forderPath))
            {
                MessageBox.Show("请先选择有效文件夹"); return;
            }

            SaveData();
            IsRunning = true;
            Progress = 0;
            _cts = new CancellationTokenSource();

            try
            {
                // 异步合并 + 报告进度
                var rows = await Task.Run(() => MergeToGenericRows(_cts.Token, new Progress<int>(v => Progress = v)));
                if (rows.Count == 0)
                {
                    MessageBox.Show("没有匹配的文件或数据被清洗掉"); return;
                }
                // 导出同样带进度
                await Task.Run(() => ExportToExcel(rows, _cts.Token, new Progress<int>(v => Progress = v)));
                MessageBox.Show($"完成，共 {rows.Count} 行");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("已取消合并");
            }
            catch (Exception ex)
            {
                MessageBox.Show("合并失败：" + ex.Message);
            }
            finally
            {
                IsRunning = false;
                Progress = 0;
                _cts?.Dispose();
                _cts = null;
            }
        }
        private List<string> _globalHeaders = new();
        /// <summary>
        /// 合并文件 + 报告进度（0-100）
        /// </summary>
        private List<GenericRow> MergeToGenericRows(CancellationToken ct, IProgress<int>? progress = null)
        {
            var allFiles = Directory.EnumerateFiles(forderPath, "*.xlsx", SearchOption.TopDirectoryOnly)
                           .Concat(Directory.EnumerateFiles(forderPath, "*.xls", SearchOption.TopDirectoryOnly))
                           .ToList();

            var result = new List<GenericRow>();
            var processed = new HashSet<string>();
            _globalHeaders.Clear();

            int total = allFiles.Count;
            int done = 0;

            foreach (var file in allFiles)
            {
                ct.ThrowIfCancellationRequested();

                var fileName = Path.GetFileNameWithoutExtension(file);
                var cfg = SheetStructures.FirstOrDefault(s =>
                    s.IsContains ? fileName.Contains(s.FileName) : fileName == s.FileName);
                if (cfg == null || processed.Contains(file)) { done++; continue; }

                processed.Add(file);

                #region 读取文件
                var fieldMap = JsonSerializer.Deserialize<Dictionary<string, int>>(cfg.FieldIndex);
                if (fieldMap == null) { done++; continue; }

                foreach (var key in fieldMap.Keys)
                    if (!_globalHeaders.Contains(key)) _globalHeaders.Add(key);

                IWorkbook workbook;
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    workbook = file.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)
                        ? (IWorkbook)new HSSFWorkbook(fs) : new XSSFWorkbook(fs);

                ISheet sheet = workbook.GetSheetAt(cfg.SheetIndex - 1);
                if (sheet == null) { done++; continue; }

                for (int r = 1; r <= sheet.LastRowNum; r++)
                {
                    ct.ThrowIfCancellationRequested();
                    IRow row = sheet.GetRow(r);
                    if (row == null) continue;

                    var genericRow = new GenericRow();
                    foreach (var (field, col) in fieldMap)
                    {
                        ICell cell = row.GetCell(col - 1);
                        object? val = cell?.CellType switch
                        {
                            CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
                                                ? cell.DateCellValue
                                                : cell.NumericCellValue,
                            CellType.Boolean => cell.BooleanCellValue,
                            _ => cell?.ToString()?.Trim() ?? string.Empty
                        };
                        genericRow[field] = val ?? string.Empty;
                    }
                    // 数据清洗（金额/日期）
                    bool hasAmount = fieldMap.ContainsKey("核销金额");
                    bool hasDate = fieldMap.ContainsKey("核销日期");
                    if (hasAmount || hasDate)
                    {
                        if (hasAmount && !double.TryParse(genericRow["核销金额"]?.ToString(), out _))
                            continue;
                        if (hasDate && !DateTime.TryParse(genericRow["核销日期"]?.ToString(), out _))
                            continue;
                    }
                    // 动态核销类型
                    if (!string.IsNullOrWhiteSpace(cfg.AccType))
                    {
                        genericRow["核销类型"] = cfg.AccType;
                        if (!_globalHeaders.Contains("核销类型")) _globalHeaders.Add("核销类型");
                    }
                    result.Add(genericRow);
                }
                done++;
                progress?.Report(done * 50 / total);   // 合并占 50%
            }
            progress?.Report(50);
            return result;
        }

        /// <summary>
        /// 导出 Excel + 报告进度（50-100）
        /// </summary>
        private void ExportToExcel(List<GenericRow> rows, CancellationToken ct, IProgress<int>? progress = null)
        {
            if (!rows.Any()) return;
            ct.ThrowIfCancellationRequested();

            using var workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("核销数据");

            // 表头
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < _globalHeaders.Count; i++)
                headerRow.CreateCell(i).SetCellValue(_globalHeaders[i]);

            int total = rows.Count;
            for (int r = 0; r < total; r++)
            {
                ct.ThrowIfCancellationRequested();
                IRow dataRow = sheet.CreateRow(r + 1);
                for (int c = 0; c < _globalHeaders.Count; c++)
                {
                    object? val = rows[r][_globalHeaders[c]];
                    WriteCellValue(dataRow.CreateCell(c), val, null);
                }
                if (r % 1000 == 0) progress?.Report(50 + (r + 1) * 50 / total);
            }

            string savePath = Path.Combine(forderPath, DateTime.Now.ToString("yyyyMMddHHmmss") + "合并后的数据.xlsx");
            using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                workbook.Write(fs);

            progress?.Report(100);
        }
        #endregion

        private static void WriteCellValue(ICell cell, object value, string? format)
        {
            if (value == null) return;
            switch (value)
            {
                case string s: cell.SetCellValue(s); break;
                case double d: cell.SetCellValue(d); break;
                case DateTime dt:
                    cell.SetCellValue(dt);
                    if (!string.IsNullOrEmpty(format))
                    {
                        var style = cell.Sheet.Workbook.CreateCellStyle();
                        var df = cell.Sheet.Workbook.CreateDataFormat();
                        style.DataFormat = df.GetFormat(format);
                        cell.CellStyle = style;
                    }
                    break;
                default: cell.SetCellValue(value.ToString() ?? ""); break;
            }
        }

        private List<BillInfo> ToBillInfo(List<GenericRow> rows)
        {
            return rows.Select(r => new BillInfo
            {
                AccNumber = r.GetValue<string>("凭证号"),
                BillWayNumber = r.GetValue<string>("来源单号"),
                AMOUNT = r.GetValue<double>("核销金额"),
                TRANSDATE = r.GetValue<string>("核销日期"),
                AccType = "大车费核销", 
                IsTemporary = false
            }).ToList();
        }
    }

    // 通用行模型
    public class GenericRow
    {
        public Dictionary<string, object> Fields { get; } = new();
        public object? this[string field]
        {
            get => Fields.TryGetValue(field, out var v) ? v : null;
            set => Fields[field] = value ?? string.Empty;
        }
        public T? GetValue<T>(string field)
        {
            var v = this[field];
            if (v == null) return default;
            try { return (T)Convert.ChangeType(v, typeof(T)); }
            catch { return default; }
        }
    }

    #endregion

    public class BoolToCompareTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? "包含" : "等于";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (string)value == "包含";
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (Visibility)value == Visibility.Visible;
    }
}