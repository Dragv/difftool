using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("difftool.Tests")]
namespace filediff
{
    internal class Program
    {
        private const string invalidArgumentsMsg = "Invalid arguments.";
        private const string fileNotFoundMsg = "Couldn't find one of the specified files to diff. Please check your file paths and try again.";
        private const string usageMsg = "usage: filediff.exe <path for base file> <path for target file>";

        private static void Main(string[] args)
        {
            // If the program doesnt receive exactly 2 arguments it means that the call is invalid so we early exit
            if (args.Length != 2)
            {
                Console.WriteLine(invalidArgumentsMsg);
                Console.WriteLine(usageMsg);
                return;
            }

            string baseFilePath = args[0];
            string targetFilePath = args[1];

            // If either of the files of the given paths dont exist we early exit
            if (!File.Exists(baseFilePath) || !File.Exists(targetFilePath))
            {
                Console.WriteLine(fileNotFoundMsg);
                Console.WriteLine(usageMsg);
                return;
            }

            string[] baseFileLines = File.ReadAllLines(baseFilePath);
            string[] targetFileLines = File.ReadAllLines(targetFilePath);

            Console.WriteLine($"Comparing {baseFilePath} against {targetFilePath}");
            int[,] matrix = DiffTool.ComputeLCS(baseFileLines, targetFileLines);
            DiffTool.Backtrack(matrix, baseFileLines, targetFileLines, out List<(int, int)> deletedLines, out List<(int, int)> insertedLines);
            DiffTool.PrintDiff(deletedLines, insertedLines, targetFileLines);
        }
    }
}
