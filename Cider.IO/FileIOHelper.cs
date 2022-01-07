using System.IO;
using System.Reflection;
using Cider.Global;

namespace Cider.IO
{
    public static class FileIOHelper
    {
        public static string SavedDirectory { get; } = "blocks";

        public static string TempDirectory { get; } = "tmp";

        public static string GetFullRuntimeDirectory(string directoryName)
        {
            return RuntimeArgs.RuntimeDirectory + directoryName + RuntimeArgs.Separator;
        }

        public static string GetFullSaveFilePath(string filename)
        {
            return GetFullRuntimeDirectory(SavedDirectory) + filename;
        }

        public static string GetFullTempFilePath(string filename)
        {
            return GetFullRuntimeDirectory(TempDirectory) + filename;
        }

        /// <summary>创建文件</summary>
        /// <param name="path">文件绝对路径</param>
        /// <returns>成功返回0 失败返回-1</returns>
        public static int CreateFile(string path)
        {
            FileStream? fs = null;
            int runtimeCode = 0;
            try
            {
                if (!File.Exists(path))
                    fs = File.Create(path);
            }
            catch
            {
                runtimeCode = -1;
            }
            finally
            {
                fs?.Dispose();
            }

            return runtimeCode;
        }

        /// <summary>写入文件</summary>
        /// <param name="path">文件绝对路径</param>
        /// <param name="buffer">文件内容</param>
        /// <returns>成功返回0 失败返回-1</returns>
        public static int WriteFile(string path, byte[] buffer)
        {
            FileStream? fs = null;
            int runtimeCode = 0;
            try
            {
                fs = File.Open(path, FileMode.OpenOrCreate);
                fs.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                runtimeCode = -1;
            }
            finally
            {
                fs?.Dispose();
            }

            return runtimeCode;
        }

        /// <summary>读取文件</summary>
        /// <param name="path">文件绝对路径</param>
        /// <param name="buffer">文件内容</param>
        /// <param name="offset">读取开始字节数</param>
        /// <returns>成功返回读取的字节数 失败返回-1</returns>
        public static int ReadFile(string path, byte[] buffer ,int offset)
        {
            FileStream? fs = null;
            int runtimeCode = 0;

            try
            {
                fs = File.Open(path, FileMode.Open);
                runtimeCode = fs.Read(buffer, offset, buffer.Length);
            }
            catch
            {
                runtimeCode = -1;
            }
            finally
            {
                fs?.Dispose();
            }

            return runtimeCode;
        }
    }
}