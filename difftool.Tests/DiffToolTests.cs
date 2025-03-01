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
    }
}
