
using System.Collections;

namespace Cider.Math
{
    public class VandermondeMatrix
    {
        /// <summary>范德蒙德矩阵</summary>
        /// <param name="row_num">行数</param>
        /// <param name="column">列数</param>
        /// <param name="bitlength">有限域比特数</param>
        public static GFMatrix CreateInstance(ulong row, ulong column, uint bitlength)
        {
            GFMatrix vmatrix = new (row, column, bitlength);
            for (ulong i = 0; i < row; i++)
            {
                GF gf = new (bitlength, 1);     // 指向当前元素
                GF alphai = new (bitlength, i + 1);     // 当前行的生成元素alpha
                for (ulong j = 0; j < column; j++)
                {
                    vmatrix[i, j] = gf;
                    gf *= alphai;
                }
            }

            return vmatrix;
        }

        /// <summary>范德蒙德行向量</summary>
        /// <param name="row_num">行号</param>
        /// <param name="column">列数 即行向量维度</param>
        /// <param name="bitlength">有限域比特数</param>
        public static GFMatrix CreateRowInstance(ulong row_num, ulong column, uint bitlength)
        {
            GFMatrix vmatrix = new (1, column, bitlength);
            GF gf = new (bitlength, 1);
            GF alpha = new (bitlength, row_num + 1);
            for (ulong j = 0; j < column; j++)
            {
                vmatrix[0, j] = gf;
                gf *= alpha;
            }

            return vmatrix;
        }

    }

    /// <summary>
    /// 范德蒙德矩阵迭代器
    /// </summary>
    public class VandermondeEnumerator : IEnumerable<GF>, IEnumerator<GF>
    {
        #region Field

        /// <summary>
        /// 当前行迭代基数
        /// </summary>
        protected GF alpha;

        #endregion

        #region Property

        /// <summary>
        /// 行数
        /// </summary>
        public long RowCount { get; }

        /// <summary>
        /// 列数
        /// </summary>
        public long ColumnCount { get; }

        /// <summary>
        /// 行号
        /// </summary>
        public long RowNumber { get; set; } = 0;

        /// <summary>
        /// 列号
        /// </summary>
        public long ColumnNumber { get; set; } = -1;

        /// <summary>
        /// 比特长度
        /// </summary>
        public uint BitLength { get; }

        public GF Current { get; private set; }

        object IEnumerator.Current => (object)Current;

        #endregion

        #region Constructor

        public VandermondeEnumerator(long rowCount, long columnCount, uint bitLength)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            BitLength = bitLength;
            Current = new GF(bitLength, 1);
            alpha = new GF(bitLength, 1);
        }

        #endregion

        #region Method

        /// <summary>
        /// 重置范德蒙德矩阵元素迭代
        /// </summary>
        public void Reset()
        {
            RowNumber = 0;
            ColumnNumber = -1;
            alpha = new GF(BitLength, 1);
        }

        /// <summary>
        /// 按行迭代
        /// </summary>
        /// <returns>是否有下一个元素</returns>
        public bool MoveNext()
        {
            if (RowNumber == RowCount - 1 && ColumnNumber == ColumnCount - 1)
                return false;

            // 行未迭代完
            if (ColumnNumber < ColumnCount - 1)
            {
                ColumnNumber++;
                Current *= alpha;
            }
            // 行迭代完 转入下一行
            else
            {
                RowNumber++;
                ColumnNumber = 0;
                alpha = new GF(BitLength, (ulong)RowNumber + 1);    // 下一行的迭代基数alpha
                Current = new GF(BitLength, 1ul);   // 首项元素为1
            }
            return true;
        }

        public void Dispose()
        {
            
        }

        public IEnumerator<GF> GetEnumerator()
        {
            Reset();
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
    }
}