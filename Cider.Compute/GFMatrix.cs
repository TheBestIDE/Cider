using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math
{
    /// <summary>
    /// 有限域上的矩阵
    /// </summary>
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

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <remarks>
        /// 使用深拷贝复制每一个元素
        /// </remarks>
        public GFMatrix(GFMatrix mat)
        {
            Row = mat.Row;
            Column = mat.Column;
            BitLength = mat.BitLength;
            matrix = new GF[Row, Column];
            for (ulong i = 0; i < Row; i++)
            {
                for (ulong j = 0; j < Column; j++)
                {
                    matrix[i, j] = new (mat.matrix[i, j]);
                }
            }
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

        /// <summary>
        /// 添加指定元素的浅拷贝
        /// </summary>
        /// <param name="row">行标</param>
        /// <param name="col">列标</param>
        /// <param name="gf"></param>
        public void Send(ulong row, ulong col, GF gf)
        {
            if (BitLength == gf.BitLength)
                matrix[row, col] = gf;
        }

        /// <summary>
        /// 添加一行的浅拷贝
        /// </summary>
        /// <param name="row">添加到的行标</param>
        /// <param name="mat">源矩阵</param>
        /// <param name="mat_row">来自源矩阵的行标</param>
        public void SendRow(ulong row, GFMatrix mat, ulong mat_row)
        {
            // 该矩阵列数小于源矩阵列数
            // 所添加到的行标超过该矩阵行数
            // 来源矩阵行标超过源矩阵行数
            // 超出索引
            if (Column < mat.Column || row >= Row || mat_row >= mat.Row)
                throw new IndexOutOfRangeException();
            for (ulong col = 0; col < Column; col++)
            {
                matrix[row, col] = mat[mat_row, col];
            }
        }

        /// <summary>
        /// 得到该矩阵的转置矩阵
        /// </summary>
        /// <remarks>
        /// 转置后的矩阵中的元素为原矩阵元素的深拷贝
        /// </remarks>
        /// <returns>转置矩阵</returns>
        public GFMatrix GetTransposeMatrix()
        {
            GFMatrix mat = new GFMatrix(Column, Row, BitLength);
            for (ulong i = 0; i < Column; i++)
            {
                for (ulong j = 0; j < Row; j++)
                {
                    // 深拷贝矩阵元素
                    // 使其元素值与原矩阵中元素引用不一样
                    mat[i, j] = new (matrix[j, i]);
                }
            }

            return mat;
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        public void NumebrMultipy(GF gf)
        {
            for (ulong i = 0; i < Row; i++)
            {
                for (ulong j = 0; j < Column; j++)
                {
                    matrix[i, j] *= gf;
                }
            }
        }

        #region Override

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            return this == (GFMatrix)obj;
        }

        public override int GetHashCode()
        {
            int code = Row.GetHashCode() ^ Column.GetHashCode() ^ BitLength.GetHashCode();
            foreach (var gf in matrix)
            {
                code ^= gf.GetHashCode();
            }
            return code;
        }

        #endregion

        #region Static Method

        /// <summary>
        /// 创建一个单位行向量
        /// </summary>
        /// <param name="column">指定为1的列标</param>
        /// <param name="col_num">指定列数</param>
        /// <param name="bitLength">有限域比特长度</param>
        public static GFMatrix CreateIdentityRowVector(ulong column, ulong col_num, long bitLength)
        {
            GFMatrix mat = new GFMatrix(1, col_num, bitLength);
            mat.matrix[0, column] = GF.One(bitLength);
            return mat;
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

        #region Override Operator

        public static GFMatrix operator +(GFMatrix left, GFMatrix right)
        {
            return Add(left, right);
        }

        public static GFMatrix operator*(GFMatrix left, GFMatrix right)
        {
            return Multipy(left, right);
        }

        public static bool operator ==(GFMatrix left, GFMatrix right)
        {
            if (left.Row != right.Row
             || left.Column != right.Column
             || left.BitLength != right.BitLength)
                return false;

            for (ulong i = 0; i < left.Row; i++)
            {
                for (ulong j = 0; j < left.Column; j++)
                {
                    if (left[i, j] != right[i, j])
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(GFMatrix left, GFMatrix right)
        {
            return !(left == right);
        }

        #endregion

        #endregion

        #endregion

    }
}
