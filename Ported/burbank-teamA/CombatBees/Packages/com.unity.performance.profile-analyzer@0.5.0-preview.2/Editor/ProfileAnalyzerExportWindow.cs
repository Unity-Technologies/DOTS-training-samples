using UnityEngine;
using System.IO;
using System;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class ProfileAnalyzerExportWindow : EditorWindow
    {
        ProfileDataView m_ProfileDataView;
        ProfileDataView m_LeftDataView;
        ProfileDataView m_RightDataView;

        public void SetData(ProfileDataView profileDataView, ProfileDataView leftDataView, ProfileDataView rightDataView)
        {
            m_ProfileDataView = profileDataView;
            m_LeftDataView = leftDataView;
            m_RightDataView = rightDataView;
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Export as CSV:");

            if (m_ProfileDataView != null && m_ProfileDataView.analysis != null)
            {
                if (GUILayout.Button("Marker table"))
                    SaveMarkerTableCSV();
            }

            if (m_ProfileDataView != null && m_ProfileDataView.data != null)
            {
                if (GUILayout.Button("Single Frame Times"))
                    SaveFrameTimesCSV();
            }

            if (m_LeftDataView != null && m_LeftDataView.data != null && m_RightDataView != null && m_RightDataView.data != null)
            {
                if (GUILayout.Button("Comparison Frame Times"))
                    SaveComparisonFrameTimesCSV();
            }

            EditorGUILayout.EndVertical();
        }

        private void SaveMarkerTableCSV()
        {
            if (m_ProfileDataView.analysis == null)
                return;

            string path = EditorUtility.SaveFilePanel("Save marker table CSV data", "", "markerTable.csv", "csv");
            if (path.Length != 0)
            {
                var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.Write("Name, ");
                    file.Write("Median Time, Min Time, Max Time, ");
                    file.Write("Median Frame Index, Min Frame Index, Max Frame Index, ");
                    file.Write("Min Depth, Max Depth, ");
                    file.Write("Total Time, ");
                    file.Write("Mean Time, Time Lower Quartile, Time Upper Quartile, ");
                    file.Write("Count Total, Count Median, Count Min, Count Max, ");
                    file.Write("Number of frames containing Marker, ");
                    file.Write("First Frame Index, ");
                    file.Write("Time Min Individual, Time Max Individual, ");
                    file.Write("Min Individual Frame, Max Individual Frame, ");
                    file.WriteLine("Time at Median Frame");

                    foreach (MarkerData marker in m_ProfileDataView.analysis.GetMarkers())
                    {
                        file.Write("{0},", marker.name);
                        file.Write("{0},{1},{2},",
                            marker.msMedian, marker.msMin, marker.msMax);
                        file.Write("{0},{1},{2},",
                            marker.medianFrameIndex, marker.minFrameIndex, marker.maxFrameIndex);
                        file.Write("{0},{1},",
                            marker.minDepth, marker.maxDepth);
                        file.Write("{0},",
                            marker.msTotal);
                        file.Write("{0},{1},{2},",
                            marker.msMean, marker.msLowerQuartile, marker.msUpperQuartile);
                        file.Write("{0},{1},{2},{3},",
                            marker.count, marker.countMedian, marker.countMin, marker.countMax);
                        file.Write("{0},", marker.presentOnFrameCount);
                        file.Write("{0},", marker.firstFrameIndex);
                        file.Write("{0},{1},",
                            marker.msMinIndividual, marker.msMaxIndividual);
                        file.Write("{0},{1},",
                            marker.minIndividualFrameIndex, marker.maxIndividualFrameIndex);
                        file.WriteLine("{0}", marker.msAtMedian);
                    }
                }
                ProfileAnalyzerAnalytics.SendUIButtonEvent(ProfileAnalyzerAnalytics.UIButton.ExportSingleFrames, analytic);
            }
        }

        private void SaveFrameTimesCSV()
        {
            if (m_ProfileDataView == null)
                return;
            if (m_ProfileDataView.data == null)
                return;

            string path = EditorUtility.SaveFilePanel("Save frame time CSV data", "", "frameTime.csv", "csv");
            if (path.Length != 0)
            {
                var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.WriteLine("Frame Offset, Frame Index, Frame Time (ms), Time from first frame (ms)");
                    float maxFrames = m_ProfileDataView.data.GetFrameCount();

                    var frame = m_ProfileDataView.data.GetFrame(0);
                    // msStartTime isn't very accurate so we don't use it

                    double msTimePassed = 0.0;
                    for (int frameOffset = 0; frameOffset < maxFrames; frameOffset++)
                    {
                        frame = m_ProfileDataView.data.GetFrame(frameOffset);
                        int frameIndex = m_ProfileDataView.data.OffsetToDisplayFrame(frameOffset);
                        float msFrame = frame.msFrame;
                        file.WriteLine("{0},{1},{2},{3}",
                            frameOffset, frameIndex, msFrame, msTimePassed);

                        msTimePassed += msFrame;
                    }
                }
                ProfileAnalyzerAnalytics.SendUIButtonEvent(ProfileAnalyzerAnalytics.UIButton.ExportSingleFrames, analytic);
            }
        }

        private void SaveComparisonFrameTimesCSV()
        {
            if (m_LeftDataView == null || m_RightDataView == null)
                return;
            if (m_LeftDataView.data == null || m_RightDataView.data == null)
                return;

            string path = EditorUtility.SaveFilePanel("Save comparison frame time CSV data", "", "frameTimeComparison.csv", "csv");
            if (path.Length != 0)
            {
                var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.Write("Frame Offset, ");
                    file.Write("Left Frame Index, Right Frame Index, ");
                    file.Write("Left Frame Time (ms), Left time from first frame (ms), ");
                    file.Write("Right Frame Time (ms), Right time from first frame (ms), ");
                    file.WriteLine("Frame Time Diff (ms)");
                    float maxFrames = Math.Max(m_LeftDataView.data.GetFrameCount(), m_RightDataView.data.GetFrameCount());

                    var leftFrame = m_LeftDataView.data.GetFrame(0);
                    var rightFrame = m_RightDataView.data.GetFrame(0);

                    // msStartTime isn't very accurate so we don't use it

                    double msTimePassedLeft = 0.0;
                    double msTimePassedRight = 0.0;

                    for (int frameOffset = 0; frameOffset < maxFrames; frameOffset++)
                    {
                        leftFrame = m_LeftDataView.data.GetFrame(frameOffset);
                        rightFrame = m_RightDataView.data.GetFrame(frameOffset);
                        int leftFrameIndex = m_LeftDataView.data.OffsetToDisplayFrame(frameOffset);
                        int rightFrameIndex = m_LeftDataView.data.OffsetToDisplayFrame(frameOffset);
                        float msFrameLeft = leftFrame != null ? leftFrame.msFrame : 0;
                        float msFrameRight = rightFrame != null ? rightFrame.msFrame : 0;
                        float msFrameDiff = msFrameRight - msFrameLeft;
                        file.Write("{0},", frameOffset);
                        file.Write("{0},{1},",leftFrameIndex, rightFrameIndex);
                        file.Write("{0},{1},", msFrameLeft, msTimePassedLeft);
                        file.Write("{0},{1},", msFrameRight, msTimePassedRight);
                        file.WriteLine("{0}",msFrameDiff);

                        msTimePassedLeft += msFrameLeft;
                        msTimePassedRight += msFrameRight;
                    }
                }
                ProfileAnalyzerAnalytics.SendUIButtonEvent(ProfileAnalyzerAnalytics.UIButton.ExportComparisonFrames, analytic);
            }
        }
    }
}
