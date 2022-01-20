using CommandLine;

namespace Cider.Client
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var exitCode = Parser.Default.ParseArguments<UploadOptions, DownloadOption>(args)
                .MapResult(
                    (UploadOptions o) => Upload(o),
                    (DownloadOption o) => Download(o),
                    error => -1);
            return exitCode;
        }

        public static int Upload(UploadOptions options)
        {
            var client = new CommunicateClient(options.Ip);
            client.Upload(options.FilePath);
            //Console.WriteLine("上传服务器：" + options.Ip);
            //Console.WriteLine("上传的文件：" + options.FilePath);
            return 0;
        }

        public static int Download(DownloadOption options)
        {
            var client = new CommunicateClient(options.Ip);
            client.Download(options.FileName, options.FilePath, options.Override);
            //Console.WriteLine("下载服务器：" + options.Ip);
            //Console.WriteLine("文件下载到：" + (string.IsNullOrEmpty(options.FilePath) ? "./" : options.FilePath));
            //Console.WriteLine("下载文件名：" + options.FileName);
            //Console.WriteLine("是否覆盖文件：" + (options.Override ? "是" : "否"));
            return 0;
        }
    }
}