using Common;
using KISHelper.Common;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace KISHelper.License
{
    #region 验证相关
    public class LocalLicense
    {
        public string Key { get; set; } = "";   // 加密后的最新钥匙
        public string Code { get; set; } = "";   // 远程 code（明文即可）
    }
    public record RemoteKey(string Key, DateTime Expire, string Status);
    public record KeyConfig(string Code, List<RemoteKey> Keys);
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

        private static readonly string LocalPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.json");

        private const string LocalPwd = "DK00078"; // 用来加密钥匙

        /// <summary>
        /// 程序启动调用一次：网络通就同步远程，不通就纯本地
        /// </summary>
        public static async Task<bool> InitializeAsync()
        {
            try
            {
                await RefreshIfNeededAsync();
            }
            catch
            {
                // 网络失败就忽略，靠本地文件继续跑
            }
            return await CheckAsync(); // 只要本地文件合法即可
        }


        #region 加密过程
        /// <summary>
        /// 纯本地验证，不联网
        /// </summary>
        public static async Task<bool> CheckAsync()
        {
            try
            {
                var local = LoadLocal();
                string key = DecryptKey(local.Key);
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(local.Code))
                    return false; // 本地无效

                string remoteJson = await DownloadCodeAsync(key);
                var remote = JsonSerializer.Deserialize<KeyConfig>(remoteJson)!;
                return remote.Code == local.Code;
            }
            catch
            {
                // 网络或其他异常也视为不通过
                return false;
            }
        }
        /// <summary>
        /// 定时器可调用：钥匙快过期就后台刷新
        /// </summary>
        public static async Task RefreshIfNeededAsync()
        {
            var remote = await DownloadRemoteConfigAsync();
            if (remote == null) return; // 网络失败就放弃

            var local = LoadLocal();
            bool needUpdate = false;

            if (remote.Code != local.Code)
            {
                local.Code = remote.Code;
                needUpdate = true;
            }

            // 钥匙升级 or 快过期（<3 天）
            var now = DateTime.UtcNow;
            var best = remote.Keys
                             .Where(k => k.Status == "active" && k.Expire > now)
                             .OrderByDescending(k => k.Expire)
                             .FirstOrDefault();

            if (best != null &&
                (local.Key == "" ||
                 best.Expire < now.AddDays(3)))
            {
                local.Key = EncryptKey(best.Key);
                needUpdate = true;
            }

            if (needUpdate) SaveLocal(local);
        }
        private static LocalLicense LoadLocal()
        {
            if (!File.Exists(LocalPath))
                return new LocalLicense();
            var json = File.ReadAllText(LocalPath);
            return JsonSerializer.Deserialize<LocalLicense>(json)!;
        }

        private static void SaveLocal(LocalLicense lic)
        {
            File.WriteAllText(LocalPath,
                JsonSerializer.Serialize(lic, new JsonSerializerOptions { WriteIndented = true }));
        }

        // 加密/解密钥匙
        private static string EncryptKey(string plainKey) => StaticCrypto.Encrypt(plainKey, LocalPwd);
        private static string DecryptKey(string enc)
        {
            try
            {
                var key = StaticCrypto.Decrypt(enc, LocalPwd);
                return key;
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException or JsonException)
            {
                return ""; // 上层检测到空即认为不合法
            }
        }


        // 拉远程 JSON
        private static async Task<KeyConfig?> DownloadRemoteConfigAsync()
        {
            try
            {
                var local = LoadLocal();
                string key = DecryptKey(local.Key);
                string json = await DownloadCodeAsync(key);
                return JsonSerializer.Deserialize<KeyConfig>(json);
            }
            catch { return null; }
        }

        private static async Task<string> DownloadCodeAsync(string token)
        {
            string api = $"https://api.github.com/repos/{Owner}/{Config}/contents/{FilePath}";
            _hc.DefaultRequestHeaders.UserAgent.ParseAdd("KISHelper/1.0");
            if (!string.IsNullOrWhiteSpace(token))
                _hc.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Token", token);

            var resp = await _hc.GetStringAsync(api);
            using var doc = JsonDocument.Parse(resp);
            string b64 = doc.RootElement.GetProperty("content").GetString()!;
            byte[] data = Convert.FromBase64String(b64);
            return Encoding.UTF8.GetString(data);
        }

        #endregion

        #region 更新过程
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

        #endregion
    }
}
