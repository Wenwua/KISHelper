using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KISHelper.Common
{
    public class SheetStructure : ViewModelBase, IDataErrorInfo
    {
		private string? fileName;

		public string? FileName
        {
			get { return fileName; }
			set { fileName = value; }
		}

		private bool isContains;

		public bool IsContains
		{
			get { return isContains; }
			set { isContains = value;}
		}

		private byte sheetIndex=1;

		public byte SheetIndex
		{
			get { return sheetIndex; }
			set { sheetIndex = value; }
		}

		private string? accType;

		public string? AccType
		{
			get { return accType; }
			set { accType = value; }
		}

		private string? fieldIndex;

		public string? FieldIndex
		{
			get { return fieldIndex; }
			set { fieldIndex = value; }
		}

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(FieldIndex))
                {
                    return ValidateFieldIndex(FieldIndex);
                }
                return string.Empty;
            }
        }

        private string ValidateFieldIndex(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;          // 未输入，暂不报错

            try
            {
                // 反序列化成 键→数字 的字典
                var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(value!)!;

                if (dict.Count == 0)
                    return "JSON 对象不能为空";

                var nonZeroValues = new HashSet<int>();

                foreach (var (key, val) in dict)
                {
                    if (val <= 0)
                        return $"键“{key}”的值不能小于0";

                    if (!nonZeroValues.Add(val))
                        return $"键“{key}”的值 {val} 与前面重复";
                }

                return string.Empty; // 全部通过
            }
            catch (JsonException ex)
            {
                return "JSON 格式错误：" + ex.Message;
            }
        }
    }
}
