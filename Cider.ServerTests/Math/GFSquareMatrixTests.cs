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
    public class GFSquareMatrixTests
    {
        [TestMethod()]
        public void GFSquareMatrixTest()
        {
            GFSquareMatrix mat = new GFSquareMatrix(3, 8);
            Assert.IsNotNull(mat);
            Assert.AreEqual(3UL, mat.Row);
            Assert.AreEqual(3UL, mat.Column);
            Assert.AreEqual(8L, mat.BitLength);
            Assert.AreEqual(3UL, mat.Dimension);
        }

        [TestMethod()]
        public void TransposeTest()
        {
            var exp = VandermondeMatrix.CreateInstance(3, 3, 8);
            GFSquareMatrix mat = new GFSquareMatrix(3, 8);
            for (ulong i = 0; i < 3; i++)
            {
                GF gf = new GF(8, 1UL);
                GF alpha = new GF(8, i + 1);
                for (ulong j = 0; j < 3; j++)
                {
                    mat[j, i] = gf;
                    gf *= alpha;
                }
            }
            mat.Transpose();
            for (ulong i = 0; i < 3; i++)
            {
                for (ulong j = 0; j < 3; j++)
                {
                    Assert.AreEqual(exp[i, j], mat[i, j]);
                }
            }
        }

        [TestMethod()]
        public void GetDeterminantTest()
        {
            var mat = GFSquareMatrix.CreateIdentityMatrix(3, 8);
            mat[2, 1] = new GF(8, 5);
            var exp = GF.One(8);
            var rslt = mat.GetDeterminant();
            Assert.AreEqual(exp, rslt);
        }

        [TestMethod()]
        public void GetCofactorDeterminantTest()
        {
            var mat = GFSquareMatrix.CreateIdentityMatrix(4, 8);
            mat[2, 1] = new GF(8, 5);
            var exp = GF.One(8);
            var rslt = mat.GetCofactorDeterminant(2, 2);
            Assert.AreEqual(exp, rslt);
        }

        [TestMethod()]
        public void GetSubMatrixTest()
        {
            GFSquareMatrix smat = new GFSquareMatrix(4, 8);
            for (ulong i = 0; i < 4; i++)
                smat.SendRow(i, VandermondeMatrix.CreateRowInstance(i, 4, 8), 0);
            var rslt = smat.GetSubMatrix(3, 3);
            var exp = VandermondeMatrix.CreateInstance(3, 3, 8);
            Assert.IsTrue(exp == rslt);

        }

        [TestMethod()]
        public void GetAdjointMatrixTest()
        {
            GFSquareMatrix smat = new GFSquareMatrix(4, 8);
            for (ulong i = 0; i < 4; i++)
                smat.SendRow(i, VandermondeMatrix.CreateRowInstance(i, 4, 8), 0);
            var rslt = smat.GetAdjointMatrix();
            var exp = GFSquareMatrix.CreateIdentityMatrix(4, 8);
            exp.NumebrMultipy(smat.GetDeterminant());
            Assert.IsTrue(exp == rslt * smat);
        }

        [TestMethod()]
        public void GetInverseMatrixTest()
        {
            GFSquareMatrix smat = new GFSquareMatrix(4, 8);
            for (ulong i = 0; i < 4; i++)
                smat.SendRow(i, VandermondeMatrix.CreateRowInstance(i, 4, 8), 0);
            var rslt = smat.GetInverseMatrix();
            var exp = GFSquareMatrix.CreateIdentityMatrix(4, 8);
            Assert.IsTrue(exp == rslt * smat);
        }

        [TestMethod()]
        public void CreateIdentityMatrixTest()
        {
            GFSquareMatrix mat = GFSquareMatrix.CreateIdentityMatrix(3, 8);
            for (ulong i = 0; i < 3; i++)
            {
                for (ulong j = 0; j < 3; j++)
                {
                    if (i == j)
                        Assert.AreEqual(GF.One(8), mat[i, j]);
                    else
                        Assert.AreEqual(GF.Zero(8), mat[i, j]);
                }
            }
        }
    }
}