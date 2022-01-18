using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.IO.Tests
{
    [TestClass()]
    public class FileIOHelperTests
    {
        [TestMethod()]
        public void GetFullRuntimeDirectoryTest()
        {
            var path = FileIOHelper.GetFullRuntimeDirectory("blocks");
            Assert.AreEqual("D:\\Code\\Csharp\\Cider\\Cider.ServerTests\\bin\\Debug\\net6.0\\blocks\\", path);
        }

        [TestMethod()]
        public void GetFullSaveFilePathTest()
        {
            var path = FileIOHelper.GetFullSaveFilePath("1234");
            Assert.AreEqual("D:\\Code\\Csharp\\Cider\\Cider.ServerTests\\bin\\Debug\\net6.0\\blocks\\1234", path);
        }

        [TestMethod()]
        public void GetFullTempFilePathTest()
        {
            var path = FileIOHelper.GetFullTempFilePath("1234");
            Assert.AreEqual("D:\\Code\\Csharp\\Cider\\Cider.ServerTests\\bin\\Debug\\net6.0\\tmp\\1234", path);
        }

        [TestMethod()]
        public void CreateFileTest()
        {
            var path = FileIOHelper.GetFullTempFilePath("1234");
            var rtnCode = FileIOHelper.CreateFile(path);
            Assert.AreEqual(0, rtnCode);
        }

        [TestMethod()]
        public void WriteFileTest()
        {
            var path = FileIOHelper.GetFullTempFilePath("1234");
            byte[] bytes = new byte[1024];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)i;
            }
            var rtnCode = FileIOHelper.WriteFile(path, bytes);
            Assert.AreEqual(0, rtnCode);
        }

        [TestMethod()]
        public void ReadFileTest()
        {
            byte[] bytes = new byte[1024];
            var path = FileIOHelper.GetFullTempFilePath("1234");
            var rtnCode = FileIOHelper.ReadFile(path, bytes, 0);
            Assert.AreEqual(1024, rtnCode);
            for (int i = 0; i < 1024; i++)
            {
                Assert.AreEqual((byte)i, bytes[i]);
            }
        }
    }
}