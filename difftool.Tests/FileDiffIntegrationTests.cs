using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace difftool.Tests
{
    [TestClass]
    public class FileDiffIntegrationTests
    {
        // Integration Tests

        private const string appExePath = "..\\..\\..\\..\\filediff\\bin\\Debug\\net6.0\\filediff.exe";
        private const string filediff1Path = "..\\..\\..\\testFiles\\filediff1.cpp";
        private const string filediff2Path = "..\\..\\..\\testFiles\\filediff2.cpp";
        private const string emptyFilePath = "..\\..\\..\\testFiles\\empty.cpp";
        private const string missingFilePath = "..\\..\\..\\testFiles\\missing.cpp";

        private const string expectedMissingFileOutput = "Couldn't find one of the specified files to diff. Please check your file paths and try again.";
        private const string expectedInvalidArgsOutput = "Invalid arguments.";
        private const string usageMsg = "usage: filediff.exe <path for base file> <path for target file>";

        public string RunProgram(string args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = appExePath;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.Arguments = args;

            Process? process = Process.Start(processStartInfo);
            process?.WaitForExit();

            return process?.StandardOutput.ReadToEnd() ?? "";
        }

        public void BasicTest(string baseFilePath, string targetFilePath)
        {
            string output = RunProgram($"{baseFilePath} {targetFilePath}");

            List<string> baseFileLines = File.ReadAllLines(baseFilePath).ToList();
            string[] targetFileLines = File.ReadAllLines(targetFilePath);
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

            for (int index = 0; index < targetFileLines.Length; index++)
            {
                Assert.AreEqual(targetFileLines[index].Trim().Length, baseFileLines[index].Trim().Length);
                Assert.IsTrue(string.Compare(targetFileLines[index].Trim(), baseFileLines[index].Trim()) == 0);
            }
        }

        public string CleanConsoleOutput(string output)
        {
            string trimmedOutput = "";
            foreach (var line in output.Split("\n"))
            {
                trimmedOutput += $"{line.Trim()}\n";
            }

            return trimmedOutput;
        }

        [TestMethod]
        public void MissingOneArg()
        {
            string output = RunProgram($"{filediff1Path}");
            output = CleanConsoleOutput(output);

            Assert.AreEqual(output.Trim(), $"{expectedInvalidArgsOutput}\n{usageMsg}");
        }

        [TestMethod]
        public void MissingAllArgs()
        {
            string output = RunProgram("");
            output = CleanConsoleOutput(output);

            Assert.AreEqual(output.Trim(), $"{expectedInvalidArgsOutput}\n{usageMsg}");

        }

        [TestMethod]
        public void TooManyArgs()
        {
            string output = RunProgram($"{filediff1Path} {filediff2Path} {emptyFilePath}");
            output = CleanConsoleOutput(output);

            Assert.AreEqual(output.Trim(), $"{expectedInvalidArgsOutput}\n{usageMsg}");
        }

        [TestMethod]
        public void MissingFirstFile()
        {
            string output = RunProgram($"{missingFilePath} {filediff1Path}");
            output = CleanConsoleOutput(output);

            Assert.AreEqual(output.Trim(), $"{expectedMissingFileOutput}\n{usageMsg}");
        }

        [TestMethod]
        public void MissingSecondFile()
        {
            string output = RunProgram($"{filediff1Path} {missingFilePath}");
            output = CleanConsoleOutput(output);

            Assert.AreEqual(output.Trim(), $"{expectedMissingFileOutput}\n{usageMsg}");
        }

        [TestMethod]
        public void MissingBothFiles()
        {
            string output = RunProgram($"{missingFilePath} {missingFilePath}");
            output = CleanConsoleOutput(output);

            Assert.AreEqual(output.Trim(), $"{expectedMissingFileOutput}\n{usageMsg}");
        }

        [TestMethod]
        public void SharedDiff1ToDiff2Case()
        {
            BasicTest(filediff1Path, filediff2Path);
        }

        [TestMethod]
        public void SharedDiff2ToDiff1Case()
        {
            BasicTest(filediff2Path, filediff1Path);
        }

        [TestMethod]
        public void Diff1ToEmptyCase()
        {
            BasicTest(filediff1Path, emptyFilePath);
        }

        [TestMethod]
        public void EmptyToDiff1Case()
        {
            BasicTest(emptyFilePath, filediff1Path);
        }

        [TestMethod]
        public void Diff2ToEmptyCase()
        {
            BasicTest(filediff2Path, emptyFilePath);
        }

        [TestMethod]
        public void EmptyToDiff2Case()
        {
            BasicTest(emptyFilePath, filediff2Path);
        }
    }
}
