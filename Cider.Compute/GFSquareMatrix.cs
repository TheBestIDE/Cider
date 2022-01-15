using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math
{
    /// <summary>
    /// 有限域上的方阵
    /// </summary>
    public class GFSquareMatrix : GFMatrix
    {
        #region Property

        /// <summary>
        /// 矩阵维数
        /// </summary>
        public ulong Dimension { get => Row; }

        #endregion

        #region Constructor

        public GFSquareMatrix(ulong dimension, long bitLength) : base(dimension, dimension, bitLength)
        {
        }

        public GFSquareMatrix(GFSquareMatrix mat) : base(mat)
        {

        }

        #endregion

        #region Method

        /// <summary>
        /// 转置该矩阵
        /// </summary>
        public void Transpose()
        {
            for (ulong row = 0; row < Dimension; row++)
            {
                for (ulong col = row + 1; col < Dimension; col++)
                {
                    GF tmp = matrix[row, col];
                    matrix[row, col] = matrix[col, row];
                    matrix[col, row] = tmp;
                }
            }
        }

        /// <summary>
        /// 计算行列式
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public GF GetDeterminant()
        {
            if (Dimension == 0)
                throw new IndexOutOfRangeException();
            if (Dimension == 1)
                return matrix[0, 0];
            if (Dimension == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            GF rslt = new GF(BitLength);
            for (ulong col = 0; col < Dimension; col++)
            {
                // 有限域上加减法一样
                // 故无需区分余子式与代数余子式
                rslt += matrix[0, col] * GetCofactorDeterminant(0, col);
            }

            return rslt;
        }

        /// <summary>
        /// 计算余子式
        /// </summary>
        /// <returns>新构造的有限域上的数</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public GF GetCofactorDeterminant(ulong x, ulong y)
        {
            if (Dimension < 2)
                throw new IndexOutOfRangeException();
            if (Dimension == 2)
                return new (matrix[2 - x - 1, 2 - y - 1]);
            return GetSubMatrix(x, y).GetDeterminant();
        }

        /// <summary>
        /// 得到子矩阵
        /// </summary>
        /// <param name="x">行数</param>
        /// <param name="y">列数</param>
        /// <returns>
        /// 去掉x行y列以后的矩阵
        /// </returns>
        /// <remarks>
        /// 注意：返回的矩阵中的元素是原矩阵中元素的浅拷贝
        /// </remarks>
        public GFSquareMatrix GetSubMatrix(ulong x, ulong y)
        {
            // 优化小矩阵 不需要进入循环
            if (Dimension <= 1)
                return this;
            if (Dimension == 2)
            {
                GFSquareMatrix tmp = new (1, BitLength);
                tmp.matrix[0, 0] = matrix[2 - x - 1, 2 - y - 1];
                return tmp;
            }

            GFSquareMatrix mat = new GFSquareMatrix(Dimension - 1, BitLength);
            for (ulong row = 0; row < Dimension; row++)
            {
                if (row == x)
                    continue;
                for (ulong col = 0; col < Dimension; col++)
                {
                    if (col == y)
                        continue;
                    mat[row < x ? row : row - 1, col < y ? col : col - 1] = matrix[row, col];
                }
            }

            return mat;
        }

        /// <summary>
        /// 计算伴随矩阵
        /// </summary>
        /// <returns></returns>
        public GFSquareMatrix GetAdjointMatrix()
        {
            GFSquareMatrix mat = new (Dimension, BitLength);
            for (ulong row = 0; row < Dimension; row++)
            {
                for (ulong col = 0; col < Dimension; col++)
                {
                    mat[col, row] = GetCofactorDeterminant(row, col);
                }
            }
            return mat;
        }

        /// <summary>
        /// 求逆矩阵
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">矩阵行列式为0</exception>
        public GFSquareMatrix GetInverseMatrix()
        {
            GF dt = GetDeterminant();   // 行列式
            if (dt == GF.Zero(BitLength))
                throw new InvalidOperationException("Matrix Determinant is 0");

            var invs = GetAdjointMatrix();   // 求伴随矩阵
            invs.NumebrMultipy(dt);      // 乘上矩阵的行列式得到逆矩阵
            return invs;
        }

        /// <summary>
        /// 创建单位矩阵
        /// </summary>
        /// <param name="dimension">矩阵维度</param>
        /// <param name="bitLength">有限域比特长度</param>
        public static GFSquareMatrix CreateIdentityMatrix(ulong dimension, long bitLength)
        {
            GFSquareMatrix mat = new (dimension, bitLength);
            for (ulong i = 0; i < dimension; i++)
            {
                mat.matrix[i, i] = 1;
            }
            return mat;
        }

        #endregion
    }
}
