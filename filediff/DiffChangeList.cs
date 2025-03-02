namespace filediff
{
    // Helper function to keep track of the changed chunks. We only keep track of the indices
    internal class DiffChangeList
    {
        private List<(int, int)> linesIndices = new List<(int, int)>();

        private int currentChunkIndex = -1;

        // If the current chunk index is less the 0 it means that we are not keeping track of one yet so we assign the starting index.
        // Afterwards and also in the given chance that we are still tracking a chunk and the index drops below zero after the index is processed it means
        // we ran out file lines so we close the chunk
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

        // If theres a chunk pending we close it and reset the current chunk index
        public void TryCloseChunk(int startIndex)
        {
            if (currentChunkIndex > -1)
            {
                linesIndices.Add((startIndex, currentChunkIndex));
                currentChunkIndex = -1;
            }
        }

        // We reverse the resulting indices list since we were backtracking
        public List<(int, int)> GetResultingList()
        {
            linesIndices.Reverse();
            return linesIndices;
        }
    }
}
