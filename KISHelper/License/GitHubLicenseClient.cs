using KISHelper.Common;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.License
{
    #region 验证相关
    public class LocalKey
    {
        public string Key { get; set; } = "";
    }
    public class LocalCode
    {
        public string Code { get; set; } = "";
    }

    public class RemoteCode
    {
        public string Code { get; set; } = "";
    }

    #endregion

    #region 更新相关
    public class UpdateInfo
    {
        public Version Version { get; set; }
        public string DownloadUrl { get; set; }
        public long Size { get; set; }
    }

    public class GithubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }
        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; }
    }
    public class Asset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
        [JsonPropertyName("size")]
        public long Size { get; set; }
    }

    #endregion

    public static class GitHubLicenseClient
    {
        private static readonly HttpClient _hc = new HttpClient();
        private const string Owner = "Wenwua";
        private const string Config = "KISHelperConfig";
        private const string REPO = "KISHelper";
        private const string FilePath = "license.json";
        private const string ASSET = "KISHelper-Release.zip";

        /// <summary>
        /// 主入口：验证本地 Key 能否拿到远程 Code，并比对
        /// </summary>
        public static async Task<bool> CheckAsync()
        {
            try
            {
                string key = LoadLocalKey();
                string localcode = LoadLocalCode();
                if (string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(localcode))
                    return false;

                string code = await DownloadCodeAsync(key);
                return code == localcode;
            }
            catch
            {
                return false;
            }
        }

        private static string LocalPath => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "license.json");

        private static string LoadLocalKey()
        {
            if (!File.Exists(LocalPath)) return "";
            var json = File.ReadAllText(LocalPath);
            return JsonSerializer.Deserialize<LocalKey>(json)?.Key ?? "";
        }
        private static string LoadLocalCode()
        {
            if (!File.Exists(LocalPath)) return "";
            var json = File.ReadAllText(LocalPath);
            return JsonSerializer.Deserialize<LocalCode>(json)?.Code ?? "";
        }

        private static async Task<string> DownloadCodeAsync(string token)
        {
            string api = $"https://api.github.com/repos/{Owner}/{Config}/contents/{FilePath}";

            _hc.DefaultRequestHeaders.UserAgent.ParseAdd("KISHelper/1.0");
            _hc.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Token", token);

            var resp = await _hc.GetStringAsync(api);
            var doc = JsonDocument.Parse(resp);
            string b64 = doc.RootElement.GetProperty("content").GetString();
            byte[] data = Convert.FromBase64String(b64);
            string json = Encoding.UTF8.GetString(data);

            return JsonSerializer.Deserialize<RemoteCode>(json)?.Code ?? "";
        }

        public static async Task<UpdateInfo> CheckUpdateAsync()
        {
            try
            {
                var url = $"https://api.github.com/repos/{Owner}/{REPO}/releases/latest";
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "KISHelper Updater"); // GitHub 要求 UA
                var json = await http.GetStringAsync(url);
                var options = new JsonSerializerOptions();
                var rel = JsonSerializer.Deserialize<GithubRelease>(json, options);

                var current = Assembly.GetExecutingAssembly().GetName().Version!;
                var latest = new Version(rel.TagName.TrimStart('v'));

                if (latest <= current) return null;

                var asset = rel.Assets
               .FirstOrDefault(a => a.Name.StartsWith("KISHelper-") &&
                                    a.Name.EndsWith("-win-x64.zip"));
                return new UpdateInfo
                {
                    Version = latest,
                    DownloadUrl = asset.BrowserDownloadUrl,
                    Size = asset.Size
                };
            }
            catch(Exception ex)
            {
                MessageBox.Show("检查更新失败:"+ex.Message);
                return null;
            }
            
        }

        public static async Task CheckUpdate()
        {
            var info = await CheckUpdateAsync();   // 就是上一段返回UpdateInfo的方法
            if (info == null) return;

            if (MessageBox.Show(
                    $"发现新版本 {info.Version}，是否更新？",
                    "更新提示",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var tmp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe");
                //File.Copy(upd, tmp, true);
                var baseDir = AppDomain.CurrentDomain.BaseDirectory
                          .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                Process.Start(new ProcessStartInfo(tmp)
                {
                    Arguments = @$"""{info.DownloadUrl}"" ""{baseDir}"" ""{Process.GetCurrentProcess().Id}"""
                });

                Application.Current.Shutdown(); // 立即退出主程序，让升级器接管
            }
        }
    }
}
