using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        
        if (args.Length != 3)
        {
            Console.WriteLine("参数: <下载url> <主程序目录> <主进程PID>");
            return;
        }
        var url = args[0];
        var dir = args[1];
        var pid = int.Parse(args[2]);

        // 1. 等待主进程退出
        try
        {
            using (var main = Process.GetProcessById(pid))
                main.WaitForExit();
        }
        catch { /* 进程已消失 */ }

        // 2. 下载更新包
        var zip = Path.Combine(Path.GetTempPath(), "update.zip");
        using (var http = new HttpClient())
            await DownloadAsync(http, url, zip);

        // 4. 解压覆盖
        ZipFile.ExtractToDirectory(zip, dir, true); // true=允许覆盖
        File.Delete(zip);

        // 5. 重启主程序
        Process.Start(Path.Combine(dir, "KISHelper.exe"));
    }

    static async Task DownloadAsync(HttpClient http, string url, string file)
    {
        Console.WriteLine("正在下载更新，请不要关闭窗口！");
        using var s = await http.GetStreamAsync(url);
        using var fs = new FileStream(file, FileMode.Create, FileAccess.Write);
        await s.CopyToAsync(fs);
        Console.WriteLine("下载更新包完毕，正在更新！");
    }
}