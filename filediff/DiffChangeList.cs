namespace filediff
{
    internal class DiffChangeList
    {
        private List<(int, int)> linesIndices = new List<(int, int)>();

        private int currentChunkIndex = -1;

        public void StartChunk(int endIndex)
        {
            if (currentChunkIndex < 0)
            {
                currentChunkIndex = endIndex;
            }
            endIndex--;
            if (endIndex <= 0)
            {
                TryCloseChunk(endIndex);
            }
        }

        public void TryCloseChunk(int startIndex)
        {
            if (currentChunkIndex > -1)
            {
                linesIndices.Add((startIndex, currentChunkIndex));
                currentChunkIndex = -1;
            }
        }

        public List<(int, int)> GetResultingList()
        {
            linesIndices.Reverse();
            return linesIndices;
        }
    }
}
