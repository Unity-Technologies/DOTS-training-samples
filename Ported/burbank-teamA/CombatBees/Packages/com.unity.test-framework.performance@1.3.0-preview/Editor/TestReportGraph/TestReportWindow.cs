using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using UnityEditor.IMGUI.Controls;
using System.IO;
using System.Linq;
using Unity.PerformanceTesting.Runtime;

namespace Unity.PerformanceTesting
{
    public class TestReportWindow : EditorWindow
    {
        static int s_windowWidth = 800;
        static int s_windowHeight = 600;
        private GUIStyle m_glStyle = null;
        private GUIStyle m_boldStyle = null;
        private Material m_material;
        public Color m_colorWhite = new Color(1.0f, 1.0f, 1.0f);
        public Color m_colorBarBackground = new Color(0.5f, 0.5f, 0.5f);
        public Color m_colorBoxAndWhiskerBackground = new Color(0.4f, 0.4f, 0.4f);
        public Color m_colorBar = new Color(0.95f, 0.95f, 0.95f);
        public Color m_colorStandardLine = new Color(1.0f, 1.0f, 1.0f);
        public Color m_colorMedianLine = new Color(0.2f, 0.5f, 1.0f, 0.5f);
        public Color m_colorMedianText = new Color(0.4f, 0.7f, 1.0f, 1.0f);
        public Color m_colorWarningText = Color.red;
        private PerformanceTestRun m_resultsData;
        private string m_selectedTest;

        private List<string> m_sampleGroups = new List<string>();

        //private int m_selectedSampleGroupIndex;
        private bool m_showTests = true;
        private bool m_showSamples = true;
        private int[] m_columnWidth = new int[4];

        [SerializeField] TreeViewState m_testListTreeViewState;
        [SerializeField] MultiColumnHeaderState m_testListMulticolumnHeaderState;
        TestListTable m_testListTable;

        Vector2 m_sampleGroupScroll = new Vector2(0, 0);
        List<SampleGroupAdditionalData> m_sampleGroupAdditionalData = new List<SampleGroupAdditionalData>();

        private void CreateTestListTable()
        {
            if (m_testListTreeViewState == null)
                m_testListTreeViewState = new TreeViewState();

            //if (m_profileMulticolumnHeaderState==null)
            m_testListMulticolumnHeaderState = TestListTable.CreateDefaultMultiColumnHeaderState(700);

            var multiColumnHeader = new MultiColumnHeader(m_testListMulticolumnHeaderState);
            multiColumnHeader.SetSorting((int) TestListTable.MyColumns.Name, false);
            multiColumnHeader.ResizeToFit();
            m_testListTable = new TestListTable(m_testListTreeViewState, multiColumnHeader, this);
        }

        public PerformanceTestRun GetResults()
        {
            return m_resultsData;
        }

        public void SelectTest(int index)
        {
            if (index < 0 || index >= m_resultsData.Results.Count)
                return;

            var result = m_resultsData.Results[index];
            SelectTest(result);
        }

        public void SelectTest(string name)
        {
            foreach (var result in m_resultsData.Results)
            {
                if (result.TestName == name)
                {
                    SelectTest(result);
                    return;
                }
            }
        }

        public void SelectTest(PerformanceTest result)
        {
            m_selectedTest = result.TestName;

            m_sampleGroups.Clear();
            foreach (var sampleGroup in result.SampleGroups)
            {
                m_sampleGroups.Add(sampleGroup.Definition.Name);
            }
        }

        [MenuItem("Window/Analysis/Performance Test Report")]
        private static void Init()
        {
            var window = GetWindow<TestReportWindow>("Test Report");
            window.minSize = new Vector2(640, 480);
            window.position.size.Set(s_windowWidth, s_windowHeight);
            window.Show();
        }

        public bool CheckAndSetupMaterial()
        {
            if (m_material == null)
                m_material = new Material(Shader.Find("Unlit/TestReportShader"));

            if (m_material == null)
                return false;

            return true;
        }

        private void LoadData()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "PerformanceTestResults.json");
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            m_resultsData = JsonUtility.FromJson<PerformanceTestRun>(json);

            List<SamplePoint> samplePoints = new List<SamplePoint>();

            m_sampleGroupAdditionalData.Clear();
            foreach (var result in m_resultsData.Results)
            {
                foreach (var sampleGroup in result.SampleGroups)
                {
                    samplePoints.Clear();
                    for (int index = 0; index < sampleGroup.Samples.Count; index++)
                    {
                        var sample = sampleGroup.Samples[index];
                        samplePoints.Add(new SamplePoint(sample, index));
                    }

                    samplePoints.Sort();

                    int discard;
                    SampleGroupAdditionalData data = new SampleGroupAdditionalData();
                    data.min = (float) GetPercentageOffset(samplePoints, 0, out discard);
                    data.lowerQuartile = (float) GetPercentageOffset(samplePoints, 25, out discard);
                    data.median = (float) GetPercentageOffset(samplePoints, 50, out discard);
                    data.upperQuartile = (float) GetPercentageOffset(samplePoints, 75, out discard);
                    data.max = (float) GetPercentageOffset(samplePoints, 100, out discard);

                    m_sampleGroupAdditionalData.Add(data);
                }
            }

            CreateTestListTable();
        }

        private void OnEnable()
        {
            CheckAndSetupMaterial();

            LoadData();
        }

        private void OnGUI()
        {
            if (m_glStyle == null)
            {
                m_glStyle = new GUIStyle(GUI.skin.box);
                m_glStyle.padding = new RectOffset(0, 0, 0, 0);
                m_glStyle.margin = new RectOffset(0, 0, 0, 0);
            }

            if (m_boldStyle == null)
            {
                m_boldStyle = new GUIStyle(GUI.skin.label);
                m_boldStyle.fontStyle = FontStyle.Bold;
            }

            Draw();
        }

        private double GetPercentageOffset(List<SamplePoint> samplePoint, float percent, out int outputFrameIndex)
        {
            int index = (int) ((samplePoint.Count - 1) * percent / 100);
            outputFrameIndex = samplePoint[index].index;

            // True median is half of the sum of the middle 2 frames for an even count. However this would be a value never recorded so we avoid that.
            return samplePoint[index].sample;
        }

        private bool BoldFoldout(bool toggle, string text)
        {
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            return EditorGUILayout.Foldout(toggle, text, foldoutStyle);

            /*
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel, GUILayout.Width(100));
            return true;
            */
        }

        private void SetColumnSizes(int a, int b, int c, int d)
        {
            m_columnWidth[0] = a;
            m_columnWidth[1] = b;
            m_columnWidth[2] = c;
            m_columnWidth[3] = d;
        }

        private void DrawColumn(int n, string col)
        {
            if (m_columnWidth[n] > 0)
                EditorGUILayout.LabelField(col, GUILayout.Width(m_columnWidth[n]));
            else
                EditorGUILayout.LabelField(col);
        }

        private void DrawColumn(int n, float value)
        {
            DrawColumn(n, string.Format("{0:f2}", value));
        }

        private void Draw2Column(string label, float value)
        {
            EditorGUILayout.BeginHorizontal();
            DrawColumn(0, label);
            DrawColumn(1, value);
            EditorGUILayout.EndHorizontal();
        }

        private void Draw()
        {
            GUIStyle selectedStyle = new GUIStyle(GUI.skin.label);
            selectedStyle.fontStyle = FontStyle.Bold;

            if (GUILayout.Button("Refresh"))
            {
                LoadData();
                CreateTestListTable();
                if (m_resultsData == null) return;
                if (m_resultsData.Results == null) return;
                if (m_resultsData.Results.Any(result => result.TestName == m_selectedTest))
                    SelectTest(m_selectedTest);
                else
                    SelectTest(0);
            }

            if (m_resultsData == null)
                return;

            if (m_resultsData.Results.Count <= 0)
                GUILayout.Label("No performance test data found.\nNote this is supported only on 2018.3 or newer.");
            else
            {
                m_showTests = BoldFoldout(m_showTests, "Test View");
                if (m_showTests)
                {
                    if (m_testListTable != null)
                    {
                        Rect r = GUILayoutUtility.GetRect(position.width, s_windowHeight / 4, GUI.skin.box,
                            GUILayout.ExpandWidth(true));
                        m_testListTable.OnGUI(r);
                    }
                }

                if (!string.IsNullOrEmpty(m_selectedTest))
                {
                    var profileDataFile = Path.Combine(Application.persistentDataPath,
                        Utils.RemoveIllegalCharacters(m_selectedTest) + ".raw");
                    if (File.Exists(profileDataFile))
                    {
                        if (GUILayout.Button(string.Format("Load profiler data for test: {0}", m_selectedTest)))
                        {
                            ProfilerDriver.LoadProfile(profileDataFile, false);
                        }
                    }
                }

                m_showSamples = BoldFoldout(m_showSamples, "Sample Group View");
                if (m_showSamples)
                {
                    SetColumnSizes(50, 50, 50, 50);

                    EditorGUILayout.BeginVertical();
                    m_sampleGroupScroll =
                        EditorGUILayout.BeginScrollView(m_sampleGroupScroll, false,
                            true); //, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.window);
                    int dataIndex = 0;
                    foreach (var result in m_resultsData.Results)
                    {
                        if (result.TestName != m_selectedTest)
                        {
                            dataIndex += result.SampleGroups.Count;
                            continue;
                        }

                        foreach (var sampleGroup in result.SampleGroups)
                        {
                            var data = m_sampleGroupAdditionalData[dataIndex];
                            dataIndex++;

                            float min = data.min;
                            float lowerQuartile = data.lowerQuartile;
                            float median = data.median;
                            float upperQuartile = data.upperQuartile;
                            float max = data.max;

                            float graphMin = min > 0.0f ? 0.0f : min;

                            EditorGUILayout.BeginVertical(GUI.skin.box,
                                GUILayout.Width(position.width - GUI.skin.verticalScrollbar.fixedWidth -
                                                (GUI.skin.box.padding.horizontal + GUI.skin.box.margin.horizontal)),
                                GUILayout.ExpandHeight(false));
                            EditorGUILayout.LabelField(sampleGroup.Definition.Name, m_boldStyle);
                            EditorGUILayout.LabelField("Sample Unit: " + sampleGroup.Definition.SampleUnit.ToString());

                            EditorGUILayout.BeginHorizontal(GUILayout.Height(100), GUILayout.ExpandHeight(false));

                            EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.ExpandHeight(true));
                            Draw2Column("Max", max);
                            GUILayout.FlexibleSpace();
                            Color oldColor = GUI.contentColor;
                            if (median < 0.01f)
                                GUI.contentColor = m_colorWarningText;
                            else
                                GUI.contentColor = m_colorMedianText;
                            Draw2Column("Median", median);
                            GUI.contentColor = oldColor;
                            //Draw2Column("SD", (float)sampleGroup.StandardDeviation);
                            //Draw2Column("P", (float)sampleGroup.PercentileValue);
                            GUILayout.FlexibleSpace();
                            Draw2Column("Min", min);
                            EditorGUILayout.EndVertical();
                            DrawBarGraph(position.width - 200, 100, sampleGroup.Samples, graphMin, max, median);
                            DrawBoxAndWhiskerPlot(50, 100, min, lowerQuartile, median, upperQuartile, max, min, max,
                                (float) sampleGroup.StandardDeviation, m_colorWhite, m_colorBoxAndWhiskerBackground);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndVertical();
                        }
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void DrawBarGraph(float width, float height, List<double> samples, float min, float max, float median)
        {
            if (DrawStart(width, height))
            {
                Rect rect = GUILayoutUtility.GetLastRect();

                float clipH = Math.Max(m_sampleGroupScroll.y - rect.y, 0);
                float maxH = Math.Max(height - clipH, 0);

                int xAxisDivisions = samples.Count;
                float xAxisInc = width / xAxisDivisions;

                float yRange = max - min;
                float x = 0;
                float y = 0;
                float spacing = 2;
                float w = xAxisInc - spacing;
                if (w < 1) spacing = 0;

                float h = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    float sample = (float) samples[i];

                    w = xAxisInc - spacing;
                    h = ((sample - min) * height) / yRange;
                    if (h > maxH)
                        h = maxH;
                    DrawBar(x, y + (height - h), w, h, m_colorBar);

                    x += xAxisInc;
                }

                h = ((median - min) * height) / yRange;
                if (h <= maxH)
                {
                    //DrawLine(0, (height - h), width, (height - h), m_colorMedianLine);
                    DrawBar(0, (height - h), width, 3, m_colorMedianLine);
                }

                x = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    float sample = (float) samples[i];

                    string tooltip = string.Format("{0} (at sample {1} of {2})", sample, i, samples.Count);

                    GUI.Label(new Rect(rect.x + x, rect.y + y, xAxisInc, height), new GUIContent("", tooltip));

                    x += xAxisInc;
                }

                DrawEnd();
            }
        }

        public bool DrawStart(Rect r)
        {
            if (Event.current.type != EventType.Repaint)
                return false;

            if (!CheckAndSetupMaterial())
                return false;

            GL.PushMatrix();
            CheckAndSetupMaterial();
            m_material.SetPass(0);

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(new Vector3(r.x, r.y, 0), Quaternion.identity, Vector3.one);
            GL.MultMatrix(matrix);
            return true;
        }

        public bool DrawStart(float w, float h, GUIStyle style = null)
        {
            Rect r = GUILayoutUtility.GetRect(w, h, style == null ? m_glStyle : style);
            return DrawStart(r);
        }

        public void DrawEnd()
        {
            GL.PopMatrix();
        }

        public void DrawBar(float x, float y, float w, float h, Color col)
        {
            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(col);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(x + w, y, 0);
            GL.Vertex3(x, y + h, 0);
            GL.Vertex3(x + w, y + h, 0);
            GL.End();
        }

        void DrawBar(float x, float y, float w, float h, float r, float g, float b)
        {
            DrawBar(x, y, w, h, new Color(r, g, b));
        }

        void DrawLine(float x, float y, float x2, float y2, Color col)
        {
            GL.Begin(GL.LINES);
            GL.Color(col);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(x2, y2, 0);
            GL.End();
        }

        void DrawLine(float x, float y, float x2, float y2, float r, float g, float b)
        {
            DrawLine(x, y, x2, y2, new Color(r, g, b));
        }

        void DrawBox(float x, float y, float w, float h, Color col)
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(col);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(x + w, y, 0);
            GL.Vertex3(x + w, y + h, 0);
            GL.Vertex3(x, y + h, 0);
            GL.Vertex3(x, y, 0);
            GL.End();
        }

        void DrawBox(float x, float y, float w, float h, float r, float g, float b)
        {
            DrawBox(x, y, w, h, new Color(r, g, b));
        }

        private float ClampToRange(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        private void DrawHistogramStart(float width)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(width + 10));

            //GUILayoutUtility.GetRect(GUI.skin.box.margin.left, 1);

            EditorGUILayout.BeginVertical();
        }

        private void DrawHistogramEnd(float width, float min, float max, float spacing)
        {
            EditorGUILayout.BeginHorizontal();
            float lastBar = width - 50;
            GUIStyle rightAlignStyle = new GUIStyle(GUI.skin.label);
            rightAlignStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(string.Format("{0:f2}", min), GUILayout.Width(lastBar));
            EditorGUILayout.LabelField(string.Format("{0:f2}", max), rightAlignStyle, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHistogramBackground(float width, float height, int bucketCount, float spacing)
        {
            float x = (spacing / 2);
            float y = 0;
            float w = ((width + spacing) / bucketCount) - spacing;
            float h = height;

            for (int i = 0; i < bucketCount; i++)
            {
                DrawBar(x, y, w, h, m_colorBarBackground);
                x += w;
                x += spacing;
            }
        }

        private void DrawHistogramData(float width, float height, int[] buckets, int totalFrameCount, float min,
            float max, Color barColor, float spacing)
        {
            float x = (spacing / 2);
            float y = 0;
            float w = ((width + spacing) / buckets.Length) - spacing;
            float h = height;

            Rect rect = GUILayoutUtility.GetLastRect();

            int bucketCount = buckets.Length;
            float bucketWidth = ((max - min) / bucketCount);
            for (int bucketAt = 0; bucketAt < bucketCount; bucketAt++)
            {
                var count = buckets[bucketAt];

                float barHeight = (h * count) / totalFrameCount;
                if (barHeight > rect.height)
                    barHeight = rect.height;
                DrawBar(x, y + (h - barHeight), w, barHeight, barColor);

                float bucketStart = min + (bucketAt * bucketWidth);
                float bucketEnd = bucketStart + bucketWidth;
                GUI.Label(new Rect(rect.x + x, rect.y + y, w, h),
                    new GUIContent("", string.Format("{0:f2}-{1:f2}ms\n{2} frames", bucketStart, bucketEnd, count)));

                x += w;
                x += spacing;
            }
        }

        private void DrawHistogram(float width, float height, int[] buckets, int totalFrameCount, float min, float max,
            Color barColor)
        {
            DrawHistogramStart(width);

            float spacing = 2;

            if (DrawStart(width, height))
            {
                Rect rect = GUILayoutUtility.GetLastRect();

                float clipH = Math.Max(m_sampleGroupScroll.y - rect.y, 0);
                float maxH = Math.Max(height - clipH, 0);

                DrawHistogramBackground(width, maxH, buckets.Length, spacing);
                DrawHistogramData(width, height, buckets, totalFrameCount, min, max, barColor, spacing);
                DrawEnd();
            }

            DrawHistogramEnd(width, min, max, spacing);
        }

        private void DrawBoxAndWhiskerPlot(float width, float height, float min, float lowerQuartile, float median,
            float upperQuartile, float max, float yAxisStart, float yAxisEnd, float standardDeviation, Color color,
            Color colorBackground)
        {
            if (DrawStart(width, height))
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                float clipH = Math.Max(m_sampleGroupScroll.y - rect.y, 0);
                float maxH = Math.Max(height - clipH, 0);
                if (maxH > 0)
                {
                    float x = 0;
                    float y = 0;
                    float w = width;
                    float h = height;

                    DrawBoxAndWhiskerPlot(rect, x, y, w, h, min, lowerQuartile, median, upperQuartile, max, yAxisStart,
                        yAxisEnd, standardDeviation, color, colorBackground);
                }

                DrawEnd();
            }
        }

        private void DrawBoxAndWhiskerPlot(Rect rect, float x, float y, float w, float h, float min,
            float lowerQuartile, float median, float upperQuartile, float max, float yAxisStart, float yAxisEnd,
            float standardDeviation, Color color, Color colorBackground)
        {
            string tooltip = string.Format(
                "Max :\t\t{0:f2}\n\nUpper Quartile :\t{1:f2}\nMedian :\t\t{2:f2}\nLower Quartile :\t{3:f2}\n\nMin :\t\t{4:f2}\n\nStandard Deviation:\t{5:f2}",
                max, upperQuartile, median, lowerQuartile, min,
                standardDeviation);
            GUI.Label(rect, new GUIContent("", tooltip));

            float clipH = Math.Max(m_sampleGroupScroll.y - rect.y, 0);
            float maxH = Math.Max(h - clipH, 0);

            DrawBar(x, y + (h - maxH), w, maxH, m_colorBoxAndWhiskerBackground);

            float first = yAxisStart;
            float last = yAxisEnd;
            float range = last - first;

            bool startCap = (min >= first) ? true : false;
            bool endCap = (max <= last) ? true : false;

            // Range clamping
            min = ClampToRange(min, first, last);
            lowerQuartile = ClampToRange(lowerQuartile, first, last);
            median = ClampToRange(median, first, last);
            upperQuartile = ClampToRange(upperQuartile, first, last);
            max = ClampToRange(max, first, last);

            float yMin = h * (min - first) / range;
            float yLowerQuartile = h * (lowerQuartile - first) / range;
            float yMedian = h * (median - first) / range;
            float yUpperQuartile = h * (upperQuartile - first) / range;
            float yMax = h * (max - first) / range;

            // Clamp to scroll area
            yMin = Math.Min(yMin, maxH);
            yLowerQuartile = Math.Min(yLowerQuartile, maxH);
            yMedian = Math.Min(yMedian, maxH);
            yUpperQuartile = Math.Min(yUpperQuartile, maxH);
            yMax = Math.Min(yMax, maxH);

            // Min to max line
            //DrawLine(x + (w / 2), y + (h - yMin), x + (w / 2), y + (h - yMax), color);
            DrawLine(x + (w / 2), y + (h - yMin), x + (w / 2), y + (h - yLowerQuartile), color);
            DrawLine(x + (w / 2), y + (h - yUpperQuartile), x + (w / 2), y + (h - yMax), color);

            // Quartile boxes
            float xMargin = (2 * w / 8);
            if (colorBackground != color)
                DrawBar(x + xMargin, y + (h - yMedian), w - (2 * xMargin), (yMedian - yLowerQuartile), colorBackground);
            DrawBox(x + xMargin, y + (h - yMedian), w - (2 * xMargin), (yMedian - yLowerQuartile), color);
            if (colorBackground != color)
                DrawBar(x + xMargin, y + (h - yUpperQuartile), w - (2 * xMargin), (yUpperQuartile - yMedian),
                    colorBackground);
            DrawBox(x + xMargin, y + (h - yUpperQuartile), w - (2 * xMargin), (yUpperQuartile - yMedian), color);

            // Median line
            DrawLine(x + xMargin, y + (h - yMedian), x + (w - xMargin), y + (h - yMedian), color);

            // Line caps
            xMargin = (3 * w / 8);
            if (startCap)
                DrawLine(x + xMargin, y + (h - yMin), x + (w - xMargin), y + (h - yMin), color);

            if (endCap)
                DrawLine(x + xMargin, y + (h - yMax), x + (w - xMargin), y + (h - yMax), color);
        }
    }
}