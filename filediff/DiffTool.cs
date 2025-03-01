﻿namespace filediff
{
    internal class DiffTool
    {
        public static int[,] ComputeLCS(string[] baseFileLines, string[] targetFileLines)
        {
            int[,] matrix = new int[baseFileLines.Length + 1, targetFileLines.Length + 1];

            for (int rowIndex = 1; rowIndex <= baseFileLines.Length; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= targetFileLines.Length; columnIndex++)
                {
                    if (string.Compare(baseFileLines[rowIndex - 1], targetFileLines[columnIndex - 1]) == 0)
                    {
                        matrix[rowIndex, columnIndex] = matrix[rowIndex - 1, columnIndex - 1] + 1;
                    }
                    else
                    {
                        matrix[rowIndex, columnIndex] = Math.Max(matrix[rowIndex, columnIndex - 1], matrix[rowIndex - 1, columnIndex]);
                    }
                }
            }

            return matrix;
        }

        public static void Backtrack(int[,] matrix, string[] baseFileLines, string[] targetFileLines, out List<(int, int)> deletedLines, out List<(int, int)> insertedLines)
        {
            int baseFileLineIndex = baseFileLines.Length;
            int targetFileLineIndex = targetFileLines.Length;

            DiffChangeList deletedChunkList = new DiffChangeList();
            DiffChangeList insertedChunkList = new DiffChangeList();

            while (baseFileLineIndex > 0 || targetFileLineIndex > 0)
            {
                if (baseFileLineIndex - 1 >= 0 && targetFileLineIndex - 1 >= 0 && baseFileLines[baseFileLineIndex - 1] == targetFileLines[targetFileLineIndex - 1])
                {
                    deletedChunkList.TryCloseChunk(baseFileLineIndex);
                    insertedChunkList.TryCloseChunk(targetFileLineIndex);

                    baseFileLineIndex--;
                    targetFileLineIndex--;
                    continue;
                }

                if (targetFileLineIndex - 1 < 0 || (baseFileLineIndex - 1 >= 0 && matrix[baseFileLineIndex - 1, targetFileLineIndex] >= matrix[baseFileLineIndex, targetFileLineIndex - 1]))
                {
                    deletedChunkList.StartChunk(baseFileLineIndex);
                    baseFileLineIndex--;
                }
                else
                {
                    insertedChunkList.StartChunk(targetFileLineIndex);
                    targetFileLineIndex--;
                }
            }

            deletedLines = deletedChunkList.GetResultingList();
            insertedLines = insertedChunkList.GetResultingList();
        }

        public static void PrintDiff(List<(int, int)> deletedLines, List<(int, int)> insertedLines, string[] targetFileLines)
        {
            int maxLength = deletedLines.Count + insertedLines.Count;

            int deletedIndex = 0;
            int insertedIndex = 0;

            int lineOffset = 0;

            for (int index = 0; index < maxLength; index++)
            {
                if ((deletedIndex < deletedLines.Count && insertedIndex >= insertedLines.Count) || (deletedIndex < deletedLines.Count && deletedLines[deletedIndex].Item1 + lineOffset <= insertedLines[insertedIndex].Item1))
                {
                    Console.WriteLine($"<<<< remove {deletedLines[deletedIndex].Item2 - deletedLines[deletedIndex].Item1} lines from line {deletedLines[deletedIndex].Item1 + 1 + lineOffset}");
                    lineOffset -= deletedLines[deletedIndex].Item2 - deletedLines[deletedIndex].Item1;
                    deletedIndex++;
                }
                else
                {
                    Console.WriteLine($">>>> insert {insertedLines[insertedIndex].Item2 - insertedLines[insertedIndex].Item1} lines from line {insertedLines[insertedIndex].Item1 + 1}");
                    lineOffset += insertedLines[insertedIndex].Item2 - insertedLines[insertedIndex].Item1;
                    for (int linesIndex = insertedLines[insertedIndex].Item1; linesIndex < insertedLines[insertedIndex].Item2; linesIndex++)
                    {
                        Console.WriteLine(targetFileLines[linesIndex]);
                    }
                    insertedIndex++;
                }
            }
        }
    }
}
