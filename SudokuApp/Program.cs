using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using MyLibrary1;

namespace SudokuApp {
    class Program {
        static void Main(string[] args) {

            Prog();
        }

        static void Prog() {
            SudokuTable t = null;
            try {
                Console.WriteLine(t = new SudokuTable("170000006040106000000005208400078000006000005000001300020904500000000000810000649"));
                for (int i = 0; i < 9; i++) {
                    MyConsole.PrintArray(t.GetRow(i));
                }
                t.Solve();
                Console.WriteLine(t);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }

    class SudokuTable {

        private int[,] _matrix;
        private List<int> _emptyPositions;
        private List<int>[] _possibilitiesByPosition;

        public SudokuTable(string tableString) {

            if (!ValidateTableString(tableString))
                throw new SudokuException($"Invalid argument for {nameof(tableString)}!");

            _matrix = ConvertToTable(tableString);

            // apagar as linhas abaixo depois

        }

        public int[] GetRow(int index) {

            return _matrix.GetRow(index);
        }

        public int[] GetColumn(int index) {

            return _matrix.GetColumn(index);
        }

        public int[] GetSquare(int index) {

            return _matrix.GetSquare(index);
        }

        private bool ValidateTableString(string tableString) {

            int length = tableString.Length;

            if (length == 81) {
                for (int i = 0; i < length; i++) {
                    char n = tableString[i];
                    if (n < '0' || n > '9')
                        return false;
                }
                return true;
            }

            return false;
        }

        private int[,] ConvertToTable(string tableString) {

            int[,] table = new int[9, 9];

            for (int i = 0; i < tableString.Length; i++)
                table[i / 9, i % 9] = tableString[i] - 48;

            return table;
        }

        private void MapEmptyPositions() {

            _emptyPositions = new List<int>();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (_matrix[i, j] == 0)
                        _emptyPositions.Add(9 * i + j);
        }

        private void MapPossibilitiesByPosition() {

            _possibilitiesByPosition = new List<int>[_emptyPositions.Count];

            for (int i = 0; i < _emptyPositions.Count; i++) {
                _possibilitiesByPosition[i] = new List<int>(MyArray.BuildOrdered(9, 1));
                int position = _emptyPositions[i];
                int rowIndex = position / 9;
                int colIndex = position % 9;
                int sqrIndex = rowIndex / 3 * 3 + colIndex / 3;
                int[] row = _matrix.GetRow(rowIndex);
                int[] col = _matrix.GetColumn(colIndex);
                int[] sqr = _matrix.GetSquare(sqrIndex);
                int count = 0;
                while (count < _possibilitiesByPosition[i].Count) {
                    int value = _possibilitiesByPosition[i][count];
                    if (row.Contains(value) || col.Contains(value) || sqr.Contains(value))
                        _possibilitiesByPosition[i].RemoveAt(count);
                    else
                        count++;
                }
            }
        }

        public void Solve() {
            MapEmptyPositions();
            MapPossibilitiesByPosition();

            int[] next = new int[_emptyPositions.Count];

            for (int i = 0; i < _emptyPositions.Count; i++) {
                int position = _emptyPositions[i];
                int rowIndex = position / 9;
                int colIndex = position % 9;
                int sqrIndex = rowIndex / 3 * 3 + colIndex / 3;
                int j;
                int[] row = _matrix.GetRow(rowIndex);
                int[] col = _matrix.GetColumn(colIndex);
                int[] sqr = _matrix.GetSquare(sqrIndex);
                for (j = next[i]; j < _possibilitiesByPosition[i].Count; j++) {
                    //Console.WriteLine($"[{row},{col}] => trying {latest[i]}: {_possibilitiesByPosition[i][j]}");
                    next[i]++;
                    int value = _possibilitiesByPosition[i][j];
                    if (!row.Contains(value) && !col.Contains(value) && !sqr.Contains(value)) {

                        _matrix[rowIndex, colIndex] = _possibilitiesByPosition[i][j];
                        //Console.WriteLine("  ok!");
                        break;
                    }
                }
                if (j == _possibilitiesByPosition[i].Count) {
                    if (i > 0) {
                        _matrix[rowIndex, colIndex] = 0;
                        next[i] = 0;
                        i -= 2;
                        //Console.WriteLine("  No more possibilities...");
                    }
                    else
                        Console.WriteLine("Nao solucionavel!");
                }
                //Console.WriteLine();
            }
        }

        public override string ToString() {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    str.Append((_matrix[i, j] > 0 ? _matrix[i, j].ToString() : "-") + "  ");
                    if ((j + 1) % 3 == 0 && j < 8)
                        str.Append("| ");
                }
                str.Append("\n");
                if ((i + 1) % 3 == 0 && i < 8)
                    str.Append("\n");
            }

            return $"{str}";
        }
    }

    class SudokuException : ApplicationException {

        public SudokuException() : base() {

        }

        public SudokuException(string message) : base(message) {

        }
    }
}
