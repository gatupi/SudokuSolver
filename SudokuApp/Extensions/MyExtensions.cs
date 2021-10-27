using System.Text;

namespace System {
    public static class MyExtensions {

        public static int[] GetRow(this int[,] matrix, int index) {

            int[] row = null;

            if (matrix != null) {
                int columns = matrix.GetLength(1);
                row = new int[columns];
                for (int i = 0; i < columns; i++)
                    row[i] = matrix[index, i];
            }

            return row;
        }
        public static int[] GetColumn(this int[,] matrix, int index) {

            int[] column = null;

            if (matrix != null) {
                int rows = matrix.GetLength(0);
                column = new int[rows];
                for (int i = 0; i < rows; i++)
                    column[i] = matrix[i, index];
            }

            return column;
        }

        public static void Print(this int[] array) {

            Console.Write("{");

            if (array != null) {
                int lastIndex = array.Length - 1;
                for (int i = 0; i < lastIndex; i++)
                    Console.Write($"{array.GetValue(i)}, ");
                Console.Write(array.GetValue(lastIndex));
            }

            Console.WriteLine("}");
        }

        public static bool IsPerfectSquare(long n) {

            if (n < 0)
                return false;

            switch (n & 0xF) {
                case 0:
                case 1:
                case 4:
                case 9:
                    long tst = (long)Math.Sqrt(n);
                    return tst * tst == n;

                default:
                    return false;
            }
        }

    }
}
