using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("difftool.Tests")]
namespace filediff
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // If the program doesnt receive exactly 2 arguments it means that the call is invalid so we early exit
            if (args.Length != 2)
            {
                Console.WriteLine($"Invalid arguments.");
                return;
            }

            string baseFilePath = args[0];
            string targetFilePath = args[1];

            // If either of the files of the given paths dont exist we early exit
            if (!File.Exists(baseFilePath) || !File.Exists(targetFilePath))
            {
                Console.WriteLine($"Couldn't find one of the specified files to diff. Please check your file paths and try again.");
                return;
            }

            string[] baseFileLines = File.ReadAllLines(baseFilePath);
            string[] targetFileLines = File.ReadAllLines(targetFilePath);

            Console.WriteLine($"Comparing {baseFilePath} agains {targetFilePath}");
            var matrix = DiffTool.ComputeLCS(baseFileLines, targetFileLines);
            DiffTool.Backtrack(matrix, baseFileLines, targetFileLines);
        }
    }
}
