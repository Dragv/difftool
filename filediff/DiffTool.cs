namespace filediff
{
    internal class DiffTool
    {
        // Function to compute the matrix that determines the longest common sequences if the given strings
        public static int[,] ComputeLCS(string[] baseFileLines, string[] targetFileLines)
        {
            int[,] matrix = new int[baseFileLines.Length + 1, targetFileLines.Length + 1];

            // Iteration to create the matrix with one dimension being the length of the base file and the other the length of the target file
            for (int rowIndex = 1; rowIndex <= baseFileLines.Length; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= targetFileLines.Length; columnIndex++)
                {
                    // In the case that the current element on the base file and the target file are the same we add 1 to the latest
                    // matrix value since that element is a common element between the two files.
                    // Otherwise we get the max value from either the previous matrix value in a column orientation or the previous
                    // matrix value in the row orientation
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

        // Given a matrix we backtrack and fetch all the deleted and inserted indices range
        public static void Backtrack(int[,] matrix, string[] baseFileLines, string[] targetFileLines, out List<(int, int)> deletedLines, out List<(int, int)> insertedLines)
        {
            int baseFileLineIndex = baseFileLines.Length;
            int targetFileLineIndex = targetFileLines.Length;

            // Create helper functions to keep track when we are reading a deleted range or an inserted range
            DiffChangeList deletedChunkList = new DiffChangeList();
            DiffChangeList insertedChunkList = new DiffChangeList();

            while (baseFileLineIndex > 0 || targetFileLineIndex > 0)
            {
                // We first validate that we dont have an index out of bounds
                // If the base file line and the target file line are the same it means that the program is back to a common string range
                // therefore we attempt to clouse out any deleted or inserted range if there was any pending after that we keep iterating
                if (baseFileLineIndex - 1 >= 0 && targetFileLineIndex - 1 >= 0 && baseFileLines[baseFileLineIndex - 1] == targetFileLines[targetFileLineIndex - 1])
                {
                    deletedChunkList.TryCloseChunk(baseFileLineIndex);
                    insertedChunkList.TryCloseChunk(targetFileLineIndex);

                    baseFileLineIndex--;
                    targetFileLineIndex--;
                    continue;
                }

                // Given the case that we have have run out of target file indices that means that the rest of the file
                // will be a deleted chunk so we try to start a deleted chunk if we dont have one already.
                // Also if the previous base file line index value in the matrix is bigger than the previous target file line index value it means that we are in a deleted chunk
                // Otherwise we the chunk is an insertion chunk
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

        // Prints the formatted deleted and inserted chunks
        public static void PrintDiff(List<(int, int)> deletedLines, List<(int, int)> insertedLines, string[] targetFileLines)
        {
            int maxLength = deletedLines.Count + insertedLines.Count;

            int deletedIndex = 0;
            int insertedIndex = 0;

            // We keep a line offset since we will be inserting and deleting lines sequencially so we want to keep track of change on lines in the working file
            int lineOffset = 0;

            for (int index = 0; index < maxLength; index++)
            {
                // We print the first possible change in the lists of changed chunks
                if ((deletedIndex < deletedLines.Count && insertedIndex >= insertedLines.Count) || (deletedIndex < deletedLines.Count && deletedLines[deletedIndex].Item1 + lineOffset <= insertedLines[insertedIndex].Item1))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"<<<< remove {deletedLines[deletedIndex].Item2 - deletedLines[deletedIndex].Item1} lines from line {deletedLines[deletedIndex].Item1 + 1 + lineOffset}");
                    lineOffset -= deletedLines[deletedIndex].Item2 - deletedLines[deletedIndex].Item1;
                    deletedIndex++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($">>>> insert {insertedLines[insertedIndex].Item2 - insertedLines[insertedIndex].Item1} lines from line {insertedLines[insertedIndex].Item1 + 1}");
                    lineOffset += insertedLines[insertedIndex].Item2 - insertedLines[insertedIndex].Item1;
                    for (int linesIndex = insertedLines[insertedIndex].Item1; linesIndex < insertedLines[insertedIndex].Item2; linesIndex++)
                    {
                        Console.WriteLine(targetFileLines[linesIndex]);
                    }
                    insertedIndex++;
                }
            }

            Console.ResetColor();
        }
    }
}
