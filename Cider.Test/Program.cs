using Cider.Global;
using Cider.Global.Core;
using Cider.Math;

namespace Cider.Test
{
    public class Program
    {
        public static int Main(string[] args)
        {
            TestSubString();
            return 0;
        }

        public static void TestSubString()
        {
            string path = "123\\456\\789";
            int i = path.LastIndexOf(RuntimeArgs.Separator);
            string substring = path.Substring(i + 1, path.Length - i - 1);
            Console.WriteLine(substring);
        }
        
        public static void TestMatrix()
        {
            GFMatrix matrix1 = new(2, 2, 4);
            GFMatrix matrix2 = new(2, 2, 4);
            for (ulong i = 0; i < 4; i++)
            {
                matrix1[i >> 1, i & 1] = new GF(4, (i << 1) & 0x0F);
                matrix2[i >> 1, i & 1] = new GF(4, (i << 2) & 0x0F);
            }

            GFMatrix result = matrix1 * matrix2;
            for (ulong i = 0; i < 2; i++)
            {
                Console.WriteLine("{0} {1}\t{2} {3}\t{4} {5}", matrix1[i, 0], matrix1[i, 1], matrix2[i, 0], matrix2[i, 1], result[i, 0], result[i, 1]);
            }
        }
    }
}