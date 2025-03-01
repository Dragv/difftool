using filediff;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace difftool.Tests
{
    [TestClass]
    public class DiffToolTests
    {
        private static readonly string[] list1 = { "X", "M", "J", "Y", "A", "U", "Z" };
        private static readonly string[] list2 = { "M", "Z", "J", "A", "W", "X", "U" };
        private static readonly int[,] expectedMatrixValues =
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
        private static readonly (int, int)[] expectedDeletedRangeValues = { (0, 1), (3, 4), (6, 7) };
        private static readonly (int, int)[] expectedInsertedRangeValues = { (1, 2), (4, 6) };

        [TestMethod]
        public void DiffToolCreateLCSTest()
        {
            int[,] matrix = DiffTool.ComputeLCS(list1, list2);

            Assert.IsTrue(matrix != null);

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Assert.AreEqual(expectedMatrixValues[i, j], matrix[i, j]);
                }
            }
        }

        [TestMethod]
        public void DiffToolBacktrackTest()
        {
            DiffTool.Backtrack(expectedMatrixValues, list1, list2, out List<(int, int)> deletedLines, out List<(int, int)> insertedLines);

            Assert.IsTrue(deletedLines != null);
            Assert.IsTrue(insertedLines != null);

            for (int index = 0; index < deletedLines.Count; index++)
            {
                Assert.AreEqual(deletedLines[index].Item1, expectedDeletedRangeValues[index].Item1);
                Assert.AreEqual(deletedLines[index].Item2, expectedDeletedRangeValues[index].Item2);
            }

            for (int index = 0; index < insertedLines.Count; index++)
            {
                Assert.AreEqual(insertedLines[index].Item1, expectedInsertedRangeValues[index].Item1);
                Assert.AreEqual(insertedLines[index].Item2, expectedInsertedRangeValues[index].Item2);
            }
        }
    }
}
