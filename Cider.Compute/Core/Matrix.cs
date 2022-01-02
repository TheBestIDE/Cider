using Cider.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Core
{
    public class Matrix<T> : IAddable<Matrix<T>>, IMultifyable<Matrix<T>>
        where T : IAddable<T>,
                  IMultifyable<T>,
                  IDividable<T>,
                  new()
    {
        #region Field

        /// <summary>
        /// Save data
        /// </summary>
        protected T[,] _mat;

        #endregion

        #region Constructor

        public Matrix(ulong row, ulong colum)
        {
            Row = row;
            Column = colum;
            _mat = new T[row, colum];
        }

        #endregion

        #region Property

        /// <summary>
        /// 行数
        /// </summary>
        public ulong Row { get; protected set; } = 0;

        /// <summary>
        /// 列数
        /// </summary>
        public ulong Column { get; protected set; } = 0;

        #endregion

        #region Method

        /// <summary>
        /// 返回对应行列数的元素
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public T Locate(ulong i, ulong j)
        {
            return _mat[i, j];
        }

        /// <summary>
        /// 替换矩阵对应位置的元素
        /// </summary>
        /// <param name="row">行数</param>
        /// <param name="column">列数</param>
        /// <param name="value">替换的值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Send(ulong row, ulong column, T value)
        {
            if (row < Row)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }
            if (column < Column)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }
            _mat[row, column] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public object Add(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value is Matrix<T> matrix)
            {
                return Add(matrix);
            }
            else
            {
                throw new ArgumentException("Object must be Matrix Class");
            }
        }

        /// <summary>
        /// 两个行列数相等的矩阵加法
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">行列数不相等抛出异常</exception>
        public Matrix<T> Add(Matrix<T> matrix)
        {
            return Add(this, matrix);
        }

        public static Matrix<T> Add(Matrix<T>left, Matrix<T> right)
        {
            if (left.Row == right.Row && left.Column == right.Column)
            {
                Matrix<T> mat = new(left.Row, left.Column);

                for (ulong i = 0; i < left.Row; i++)
                {
                    for (ulong j = 0; j < left.Column; j++)
                    {
                        mat.Send(i, j, right.Locate(i, j).Add(left._mat[i, j]));
                    }
                }

                return mat;
            }
            else
            {
                throw new ArgumentException("两个矩阵行数列数不相等");
            }
        }

        public static Matrix<T> operator+(Matrix<T> left, Matrix<T> right)
        {
            return Add(left, right);
        }

        public object Multify(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException (nameof(value));
            }
            if (value is Matrix<T> matrix)
            {
                return Multify(this, matrix);
            }
            else
            {
                throw new ArgumentException("Object must be Matrix Class");
            }
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="left">乘法左元素</param>
        /// <param name="right">乘法右元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Matrix<T> Multify(Matrix<T> matrix)
        {
            return Multify(this, matrix);
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="left">乘法左元素</param>
        /// <param name="right">乘法右元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Matrix<T> Multify(Matrix<T> left, Matrix<T> right)
        {
            if (left.Column == right.Row)
            {
                var mat = new Matrix<T>(left.Row, right.Column);
                for (ulong i = 0; i < left.Row; i++)
                {
                    for (ulong j = 0; j < right.Column; j++)
                    {
                        T element = new();
                        for (ulong k = 0; k < left.Column; k++)
                        {
                            var mid = left.Locate(i, k).Add(right.Locate(k, j));
                            element.Add(mid);
                        }
                        mat._mat[i, j] = element;
                    }
                }
                return mat;
            }
            else
            {
                throw new ArgumentException("左矩阵列数需要等于右矩阵行数");
            }
        }

        public static Matrix<T> operator*(Matrix<T>left, Matrix<T> right)
        {
            return Multify(left, right);
        }

        #endregion
    }
}
