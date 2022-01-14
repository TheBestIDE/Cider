using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math
{
    public class GFMatrix
    {
        #region Field

        protected GF[,] matrix;

        #endregion

        #region Construtor

        public GFMatrix(ulong row, ulong col, long bitLength)
        {
            Row = row;
            Column = col;
            BitLength = bitLength;
            matrix = new GF[Row, Column];
            InitializationMatrix();
        }

        #endregion

        #region Property

        public ulong Row { get; }

        public ulong Column { get; }

        public long BitLength { get; }

        #endregion

        #region Method

        protected void InitializationMatrix()
        {
            for (ulong i = 0; i < Row; i++)
            {
                for (ulong j = 0; j < Column; j++)
                {
                    matrix[i, j] = new GF(BitLength);
                }
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="row">行数</param>
        /// <param name="col">列数</param>
        /// <returns></returns>
        public GF this[ulong row, ulong col]
        {
            get { return matrix[row, col]; }
            set 
            {
                if (BitLength == value.BitLength)
                    matrix[row, col] = value;
            }
        }

        public GF Locate(ulong row, ulong col)
        {
            return matrix[row, col];
        }

        public void Send(ulong row, ulong col, GF gf)
        {
            if (BitLength == gf.BitLength)
                matrix[row, col] = gf;
        }

        /// <summary>
        /// 将两个矩阵相加
        /// </summary>
        /// <param name="left">左矩阵</param>
        /// <param name="right">右矩阵</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">比特长不相等或者矩阵行数或者列数不相等</exception>
        public static GFMatrix Add(GFMatrix left, GFMatrix right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("左右矩阵不在同一个伽罗华域内");
            if (left.Row == right.Row && left.Column == right.Column)
            {
                GFMatrix mat = new(left.Row, left.Column, left.BitLength);

                for (ulong i = 0; i < left.Row; i++)
                {
                    for (ulong j = 0; j < left.Column; j++)
                    {
                        mat.matrix[i, j] = left.matrix[i, j] + right.matrix[i, j];
                    }
                }

                return mat;
            }
            else
            {
                throw new ArgumentException("两个矩阵行数列数不相等");
            }
        }

        public static GFMatrix operator+(GFMatrix left, GFMatrix right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="left">左矩阵</param>
        /// <param name="right">右矩阵</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">比特长不相等或左矩阵列数不等于右矩阵行数</exception>
        public static GFMatrix Multipy(GFMatrix left, GFMatrix right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("左右矩阵不在同一个伽罗华域内");
            if (left.Column == right.Row)
            {
                var mat = new GFMatrix(left.Row, right.Column, left.BitLength);
                for (ulong i = 0; i < left.Row; i++)
                {
                    for (ulong j = 0; j < right.Column; j++)
                    {
                        for (ulong k = 0; k < left.Column; k++)
                        {
                            mat.matrix[i, j] += left.matrix[i, k] * right.matrix[k, j];
                        }
                    }
                }
                return mat;
            }
            else
            {
                throw new ArgumentException("左矩阵列数需要等于右矩阵行数");
            }
        }

        public static GFMatrix operator*(GFMatrix left, GFMatrix right)
        {
            return Multipy(left, right);
        }

        #endregion

    }
}
