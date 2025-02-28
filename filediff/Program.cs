// TODO Add args validation

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("difftool.Tests")]
namespace filediff
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string baseFilePath = args[0];
            string targetFilePath = args[1];

            string[] baseFileLines = File.ReadAllLines(baseFilePath);
            string[] targetFileLines = File.ReadAllLines(targetFilePath);

            Console.WriteLine($"Comparing {baseFilePath} agains {targetFilePath}");
            var matrix = DiffTool.ComputeLCS(baseFileLines, targetFileLines);
            DiffTool.Backtrack(matrix, baseFileLines, targetFileLines);
        }
    }
}
