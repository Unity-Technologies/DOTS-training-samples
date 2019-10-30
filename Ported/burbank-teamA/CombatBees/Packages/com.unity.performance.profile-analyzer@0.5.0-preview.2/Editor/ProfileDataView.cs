using System;
using System.Collections.Generic;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    [Serializable]
    public class ProfileDataView
    {
        public string path;
        public ProfileData data;
        public ProfileAnalysis analysisFullNew;
        public ProfileAnalysis analysisFull;
        public ProfileAnalysis analysisNew;
        public ProfileAnalysis analysis;
        public List<int> selectedIndices = new List<int> { 0, 0 };

        public bool HasValidSelection()
        {
            if (selectedIndices.Count == 2 && selectedIndices[0] == 0 && selectedIndices[1] == 0)
                return false;

            return true;
        }

        private bool SelectAllFramesContainingMarker(string markerName, ProfileAnalysis inAnalysis)
        {
            if (inAnalysis == null)
                return false;

            MarkerData markerData = inAnalysis.GetMarkerByName(markerName);
            if (markerData == null)
                return false;

            selectedIndices.Clear();
            foreach (var frameTime in markerData.frames)
            {
                selectedIndices.Add(frameTime.frameIndex);
            }

            return true;
        }

        public bool SelectAllFramesContainingMarker(string markerName, bool inSelection)
        {
            return SelectAllFramesContainingMarker(markerName, inSelection ? analysis : analysisFull);
        }

        private int ClampToRange(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;

            return value;
        }

        public void SelectFullRange()
        {
            selectedIndices.Clear();

            if (data == null)
                return;

            for (int offset = 0; offset < data.GetFrameCount(); offset++)
            {
                selectedIndices.Add(data.OffsetToDisplayFrame(offset));
            }
        }

        private void SelectIndexRangeFromOffsets(int startOffset, int endOffset)
        {
            selectedIndices.Clear();

            if (data == null)
                return;

            startOffset = ClampToRange(startOffset, 0, data.GetFrameCount() - 1);
            endOffset = ClampToRange(endOffset, 0, data.GetFrameCount() - 1);

            for (int offset = startOffset; offset <= endOffset; offset++)
            {
                selectedIndices.Add(data.OffsetToDisplayFrame(offset));
            }
        }
    }
}
