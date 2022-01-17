using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Cider.Client
{
    [Verb("up", HelpText = "上传")]
    public class UploadOptions
    {
        [Option('f', "file", Required = true, HelpText ="需要上传的文件")]
        public string FilePath { get; set; }

        [Option('i', "ip", Required = true, HelpText = "连接的服务器")]
        public string Ip { get; set; }
    }

    [Verb("down", HelpText = "下载")]
    public class DownloadOption
    {
        [Option('p', "path", Required = false, HelpText = "文件下载到的路径")]
        public string? FilePath { get; set; }

        [Option('f', "file", Required = true, HelpText = "需要下载的文件名")]
        public string FileName { get; set; }

        [Option('i', "ip", Required = true, HelpText = "连接的服务器")]
        public string Ip { get; set; }

        [Option('o', "override", Required = false, HelpText = "是否覆盖文件")]
        public bool Override { get; set; } = false;
    }
}
