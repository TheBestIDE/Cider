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
        public void NumebrMultipyTest()
        {
            var mat = VandermondeMatrix.CreateInstance(3, 4, 8);
            GF gf = new GF(8, 0x3F);
            mat.NumebrMultipy(gf);
            var exp = VandermondeMatrix.CreateInstance(3, 4, 8);
            for (ulong i = 0; i < 3; i++)
            {
                for (ulong j = 0; j < 4; j++)
                {
                    Assert.AreEqual((exp[i, j] * gf).ToString(), mat[i, j].ToString());
                }
            }
        }

        [TestMethod()]
        public void AddTest()
        {
            var mat1 = VandermondeMatrix.CreateInstance(3, 4, 8);
            var mat2 = VandermondeMatrix.CreateInstance(3, 4, 8);
            var rslt = mat1 + mat2;
            for (ulong i = 0; i < 3; i++)
            {
                for (ulong j = 0; j < 4; j++)
                {
                    Assert.AreEqual((mat1[i, j] + mat2[i, j]).ToString(), rslt[i, j].ToString());
                }
            }
        }

        [TestMethod()]
        public void MultipyTest()
        {
            var mat1 = VandermondeMatrix.CreateInstance(3, 4, 8);
            var mat2 = VandermondeMatrix.CreateInstance(4, 3, 8);
            var rslt = mat1 * mat2;
            for (ulong i = 0; i < 3; i++)
            {
                for (ulong j = 0; j < 3; j++)
                {
                    GF gf = new GF(8L);
                    for (ulong k = 0; k < 4; k++)
                    {
                        gf += mat1[i, k] * mat2[k, j];
                    }
                    Assert.AreEqual(gf.ToString(), rslt[i, j].ToString());
                }
            }
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var mat1 = VandermondeMatrix.CreateInstance(5, 6, 8);
            var mat2 = VandermondeMatrix.CreateInstance(5, 6, 8);
            Assert.IsTrue(mat1 == mat2);
            var mat3 = VandermondeMatrix.CreateInstance(5, 6, 16);
            var mat4 = VandermondeMatrix.CreateInstance(5, 5, 8);
            Assert.IsTrue(mat1 != mat3);
            Assert.IsTrue(mat1 != mat4);
        }
    }
}