// See https://aka.ms/new-console-template for more information
// TODO Add args validation

//string baseFilePath = args[0];
//string targetFilePath = args[1];

string[] num1 = new string[] { "G", "A", "C" };
string[] num2 = new string[] { "A", "G", "C", "A", "T" };

DiffTool.ComputeLCS(num1, num2);

public class DiffTool
{
    public static void ComputeLCS(string[] baseFileLines, string[] targetFileLines)
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

        for (int rowIndex = 0; rowIndex < matrix.GetLength(0); rowIndex++)
        {
            Console.WriteLine("");
            for (int columnIndex = 0; columnIndex < matrix.GetLength(1); columnIndex++)
            {
                Console.Write($"{matrix[rowIndex, columnIndex]},");
            }
        }
    }
}
