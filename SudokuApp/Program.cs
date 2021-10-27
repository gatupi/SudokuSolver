using System;
using System.Text;
using System.Collections.Generic;

namespace SudokuApp {
    class Program {
        static void Main(string[] args) {

            SudokuTable t = null;
            try {
                Console.WriteLine(t = new SudokuTable("170000006040106000000005208400078000006000005000001300020904500000000000810000649"));
                Console.WriteLine(t);
                for (int i = 0; i < 9; i++) {
                    t.GetRow(i).Print();
                }
                t.Solve();
            }
            catch (SudokuException se) {
                Console.WriteLine(se.Message);
            }
            Console.WriteLine(t == null);

            try {
                int[,,] matrix = new int[3, 5, 10];
                for (int i = 0; i < matrix.Rank; i++) {
                    Console.WriteLine($"matrix rank {i + 1} length: {matrix.GetLength(i)}");
                }
                Console.WriteLine(new int[1, 2].Rank);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            for (int i = 0; i < 9; i++) {
                t.GetRow(i).Print();
            }
            for (int i = 0; i < 10001; i++) {
                if (MyExtensions.IsPerfectSquare(i)) {
                    Console.WriteLine($"{i}, sqrt: {Math.Sqrt(i)}");
                }
            }
        }
    }

    class SudokuTable {

        private int[,] _grid;
        List<int> _emptyPositions;
        List<int>[] _possibilitiesByRow;
        List<int>[] _possibilitiesByColumn;
        List<int>[] _possibilitiesByRegion;
        List<int>[] _possibilitiesByPosition;
        bool _solved;

        public SudokuTable(string tableString) {

            if (!ValidateTableString(tableString))
                throw new SudokuException($"Invalid argument for {nameof(tableString)}!");

            _grid = ConvertToTable(tableString);
            _solved = false;

            // apagar as linhas abaixo depois

        }

        public int[] GetRow(int index) {

            return _grid.GetRow(index);
        }

        public int[] GetColumn(int index) {

            return _grid.GetColumn(index);
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
                    if (_grid[i, j] == 0)
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
                if (_grid[row, i] == number)
                    return true;

            return false;
        }

        private bool ColumnContains(int column, int number) {

            for (int i = 0; i < 9; i++)
                if (_grid[i, column] == number)
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
                if (_grid[startRow + i / 3, startCol + i % 3] == number)
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
                _grid[row, col] = 0;
                for (j = latest[i]; j < _possibilitiesByPosition[i].Count; j++) {
                    //Console.WriteLine($"[{row},{col}] => trying {latest[i]}: {_possibilitiesByPosition[i][j]}");
                    latest[i]++;
                    if (!RowContains(row, _possibilitiesByPosition[i][j]) &&
                        !ColumnContains(col, _possibilitiesByPosition[i][j]) &&
                        !RegionContains(reg, _possibilitiesByPosition[i][j])) {

                        _grid[row, col] = _possibilitiesByPosition[i][j];
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
                for (int j = 0; j < 9; j++) {
                    str.Append((_grid[i, j] > 0 ? _grid[i, j].ToString() : "-") + "  ");
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
