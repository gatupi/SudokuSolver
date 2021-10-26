using System;
using System.Text;
using System.Collections.Generic;

namespace SudokuApp {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            SudokuTable t = null, j = null;
            try {
                //Console.WriteLine(j = new SudokuTable("123456789123456789123456789123456789123456789123456789123456789123456789123456789"));
                // Console.WriteLine(t = new SudokuTable("a"));
                Console.WriteLine(t = new SudokuTable("170000006040106000000005208400078000006000005000001300020904500000000000810000649"));
                t.Solve();
                Console.WriteLine(t);
            }
            catch (SudokuException se) {
                Console.WriteLine(se.Message);
            }
            Console.WriteLine(t == null);
        }
    }

    class SudokuTable {

        private int[,] _table;
        List<int> _emptyPositions;
        List<int>[] _possibilitiesByRow;
        List<int>[] _possibilitiesByColumn;
        List<int>[] _possibilitiesByRegion;
        List<int>[] _possibilitiesByPosition;
        bool _solved;

        public SudokuTable(string tableString) {

            if (!ValidateTableString(tableString))
                throw new SudokuException($"Invalid argument for {nameof(tableString)}!");

            _table = ConvertToTable(tableString);
            _solved = false;

            // apagar as linhas abaixo depois

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
                    if (_table[i, j] == 0)
                        _emptyPositions.Add(9 * i + j);
        }

        private void MapPossibilitiesByRow() {

            _possibilitiesByRow = new List<int>[9];

            for (int i = 0; i < 9; i++) {
                _possibilitiesByRow[i] = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                int count = 0;
                while (count < _possibilitiesByRow[i].Count) {
                    if (RowContains(i, _possibilitiesByRow[i][count]))
                        _possibilitiesByRow[i].RemoveAt(count);
                    else
                        count++;
                }
            }
        }

        private void MapPossibilitiesByColumn() {

            _possibilitiesByColumn = new List<int>[9];

            for (int i = 0; i < 9; i++) {
                _possibilitiesByColumn[i] = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                int count = 0;
                while (count < _possibilitiesByColumn[i].Count) {
                    if (ColumnContains(i, _possibilitiesByColumn[i][count]))
                        _possibilitiesByColumn[i].RemoveAt(count);
                    else
                        count++;
                }
            }
        }

        private void MapPossibilitiesByRegion() {

            _possibilitiesByRegion = new List<int>[9];

            for (int i = 0; i < 9; i++) {
                TestRegion(i);
                _possibilitiesByRegion[i] = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                int count = 0;
                while (count < _possibilitiesByRegion[i].Count) {
                    if (RegionContains(i, _possibilitiesByRegion[i][count]))
                        _possibilitiesByRegion[i].RemoveAt(count);
                    else
                        count++;
                }
            }
        }

        private void MapPossibilitiesByPosition() {

            int empty = _emptyPositions.Count;
            _possibilitiesByPosition = new List<int>[empty];

            int row;
            int col;
            int reg;

            for (int i = 0; i < empty; i++) {
                _possibilitiesByPosition[i] = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                int position = _emptyPositions[i];
                row = position / 9;
                col = position % 9;
                reg = row / 3 * 3 + col / 3;
                int count = 0;
                while (count < _possibilitiesByPosition[i].Count) {
                    int value = _possibilitiesByPosition[i][count];
                    if (_possibilitiesByRow[row].Contains(value) &&
                        _possibilitiesByColumn[col].Contains(value) &&
                        _possibilitiesByRegion[reg].Contains(value)) {

                        count++;
                    }
                    else
                        _possibilitiesByPosition[i].RemoveAt(count);
                }
            }
        }

        private bool RowContains(int row, int number) {

            for (int i = 0; i < 9; i++)
                if (_table[row, i] == number)
                    return true;

            return false;
        }

        private bool ColumnContains(int column, int number) {

            for (int i = 0; i < 9; i++)
                if (_table[i, column] == number)
                    return true;

            return false;
        }

        private void TestRegion(int region) {

            int startRow = region / 3 * 3;
            int startCol = region % 3 * 3;

            Console.WriteLine($"Region {region}:\n");

            for (int i = 0; i < 9; i++) {
                Console.Write($"  [{startRow + i / 3},{startCol + i % 3}] ");
                if ((i + 1) % 3 == 0)
                    Console.WriteLine();
            }
        }

        private bool RegionContains(int region, int number) {

            int startRow = region / 3 * 3;
            int startCol = region % 3 * 3;

            for (int i = 0; i < 9; i++) {
                if (_table[startRow + i / 3, startCol + i % 3] == number)
                    return true;
            }

            return false;
        }

        public void Solve() {
            MapEmptyPositions();
            MapPossibilitiesByRow();
            MapPossibilitiesByColumn();
            MapPossibilitiesByRegion();
            MapPossibilitiesByPosition();

            int row;
            int col;
            int reg;
            int position;
            int j;

            int[] latest = new int[_emptyPositions.Count];

            for (int i = 0; i < _emptyPositions.Count; i++) {
                position = _emptyPositions[i];
                row = position / 9;
                col = position % 9;
                reg = row / 3 * 3 + col / 3;
                _table[row, col] = 0;
                for (j = latest[i]; j < _possibilitiesByPosition[i].Count; j++) {
                    //Console.WriteLine($"[{row},{col}] => trying {latest[i]}: {_possibilitiesByPosition[i][j]}");
                    latest[i]++;
                    if (!RowContains(row, _possibilitiesByPosition[i][j]) &&
                        !ColumnContains(col, _possibilitiesByPosition[i][j]) &&
                        !RegionContains(reg, _possibilitiesByPosition[i][j])) {

                        _table[row, col] = _possibilitiesByPosition[i][j];
                        //Console.WriteLine("  ok!");
                        break;
                    }
                }
                if (j == _possibilitiesByPosition[i].Count) {
                    if (i > 0) {
                        latest[i] = 0;
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
                for (int j = 0; j < 8; j++)
                    str.Append(_table[i, j] + ",");
                str.Append(_table[i, 8] + "\n");
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
