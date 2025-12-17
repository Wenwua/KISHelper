using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace KISHelper.Common
{
    public class DataRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;

        public DataRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        /// <summary>
        /// 保存任意类型的数据列表
        /// </summary>
        /// <param name="key">数据标识（如"AccountBooks"）</param>
        /// <param name="data">数据列表</param>
        public void SaveData<T>(string key, List<T> data)
        {
            // 加载现有数据
            var allData = LoadRawData();

            // 更新/添加数据
            allData[key] = JsonSerializer.SerializeToDocument(data, _options).RootElement;

            // 写回文件
            string json = JsonSerializer.Serialize(allData, _options);
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            File.WriteAllText(_filePath, json, Encoding.UTF8);
        }

        /// <summary>
        /// 加载指定类型的数据列表
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <returns>数据列表</returns>
        public List<T> LoadData<T>(string key)
        {
            var allData = LoadRawData();
            if (allData.TryGetValue(key, out var element))
            {
                return JsonSerializer.Deserialize<List<T>>(element.GetRawText(), _options)
                       ?? new List<T>();
            }
            return new List<T>();
        }

        /// <summary>
        /// 加载所有原始数据
        /// </summary>
        private Dictionary<string, JsonElement> LoadRawData()
        {
            if (!File.Exists(_filePath))
                return new Dictionary<string, JsonElement>();

            try
            {
                string json = File.ReadAllText(_filePath, Encoding.UTF8);
                using var doc = JsonDocument.Parse(json);
                var result = new Dictionary<string, JsonElement>();
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    result[property.Name] = property.Value.Clone();
                }
                return result;
            }
            catch
            {
                return new Dictionary<string, JsonElement>();
            }
        }

    }
}
