
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
            GFMatrix vmatrix = new GFMatrix(row, column, bitlength);
            for (ulong i = 0; i < row; i++)
            {
                GF gf = new GF(bitlength, 1);   // 指向当前元素
                GF alphai = new GF(bitlength, i + 1);   // 当前行的生成元素alpha
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
            GFMatrix vmatrix = new GFMatrix(1, column, bitlength);
            GF gf = new GF(bitlength, 1);
            GF alpha = new GF(bitlength, row_num + 1);
            for (ulong j = 0; j < column; j++)
            {
                vmatrix[0, j] = gf;
                gf *= alpha;
            }

            return vmatrix;
        }
    }
}