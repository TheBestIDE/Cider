using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Tests
{
    [TestClass()]
    public class GFMatrixTests
    {
        [TestMethod()]
        public void SendRowTest()
        {
            GFMatrix matrix = new GFMatrix(5, 6, 8);
            var mat1 = VandermondeMatrix.CreateRowInstance(0, 6, 8);
            var mat2 = VandermondeMatrix.CreateRowInstance(3, 6, 8);
            var mat3 = VandermondeMatrix.CreateRowInstance(2, 6, 8);
            matrix.SendRow(0, mat1, 0);
            matrix.SendRow(1, mat2, 0);
            matrix.SendRow(2, mat3, 0);
            for (ulong j = 0; j < 6; j++)
            {
                Assert.AreEqual(mat1[0, j].ToString(), matrix[0, j].ToString());
            }
            for (ulong j = 0; j < 6; j++)
            {
                Assert.AreEqual(mat2[0, j].ToString(), matrix[1, j].ToString());
            }
            for (ulong j = 0; j < 6; j++)
            {
                Assert.AreEqual(mat3[0, j].ToString(), matrix[2, j].ToString());
            }
        }

        [TestMethod()]
        public void GetTransposeMatrixTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NumebrMultipyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MultipyTest()
        {
            Assert.Fail();
        }
    }
}