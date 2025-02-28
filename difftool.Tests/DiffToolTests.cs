using filediff;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace difftool.Tests
{
    [TestClass]
    public class DiffToolTests
    {
        [TestMethod]
        public void CreateLCS()
        {
            string[] list1 = new string[] { "X", "M", "J", "Y", "A", "U", "Z" };
            string[] list2 = new string[] { "M", "Z", "J", "A", "W", "X", "U" };
            int[,] matrix = DiffTool.ComputeLCS(list1, list2);

            Assert.IsTrue(matrix != null);

            int[,] expectedMatrixValues =
            {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1 },
                { 0, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 1, 2, 2, 2, 2, 2 },
                { 0, 1, 1, 2, 2, 2, 2, 2 },
                { 0, 1, 1, 2, 3, 3, 3, 3 },
                { 0, 1, 1, 2, 3, 3, 3, 4 },
                { 0, 1, 2, 2, 3, 3, 3, 4 }
            };

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Assert.AreEqual(expectedMatrixValues[i, j], matrix[i, j]);
                }
            }
        }

        private const string appExePath = "C:\\Workspace\\filediff\\filediff\\bin\\Debug\\net6.0\\filediff.exe";
        private const string baseFilePath = "C:\\Workspace\\filediff\\filediff\\bin\\Debug\\net6.0\\filediff1.cpp";
        private const string targetFilePath = "C:\\Workspace\\filediff\\filediff\\bin\\Debug\\net6.0\\filediff2.cpp";

        [TestMethod]
        public void TestRoundTrip()
        {
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = appExePath;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.Arguments = $"{baseFilePath} {targetFilePath}";

            List<string> baseFileLines = File.ReadAllLines(baseFilePath).ToList();
            string[] targetFileLines = File.ReadAllLines(targetFilePath);

            var process = Process.Start(processStartInfo);

            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string[] diffLines = Regex.Split(output, @"(?<=[\n])");

            for (int index = 0; index < diffLines.Length; index++)
            {
                string diff = diffLines[index];

                if (diff.Contains("<<<< remove"))
                {
                    int[] amountAndLinePair = Regex.Matches(diff, @"\d+").Select(x => Int32.Parse(x.Value)).ToArray();
                    baseFileLines.RemoveRange(amountAndLinePair[1] - 1, amountAndLinePair[0]);
                }

                if (diff.Contains(">>>> insert"))
                {
                    int[] amountAndLinePair = Regex.Matches(diff, @"\d+").Select(x => Int32.Parse(x.Value)).ToArray();

                    List<string> insertBlock = new List<string>();
                    for (int appendBlockIndex = 0; appendBlockIndex < amountAndLinePair[0]; appendBlockIndex++)
                    {
                        index++;
                        insertBlock.Add(diffLines[index]);
                    }
                    baseFileLines.InsertRange(amountAndLinePair[1] - 1, insertBlock);
                }
            }

            Assert.IsTrue(process.ExitCode == 0);
            for (int index = 0; index < targetFileLines.Length; index++)
            {
                Assert.AreEqual(targetFileLines[index].Trim().Length, baseFileLines[index].Trim().Length);
                Assert.IsTrue(string.Compare(targetFileLines[index].Trim(), baseFileLines[index].Trim()) == 0);
            }
        }
    }
}