using UnityEngine;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class Histogram
    {
        Draw2D m_2D;
        Color m_ColorBarBackground;
        DisplayUnits m_Units;

        string DisplayUnits()
        {
            return m_Units.Postfix();
        }

        string ToDisplayUnits(float ms, bool showUnits = false)
        {
            return m_Units.ToString(ms, showUnits, 5);
        }

        public void SetUnits(Units units)
        {
            m_Units = new DisplayUnits(units);
        }

        public Histogram(Draw2D draw2D, Units units)
        {
            m_2D = draw2D;
            SetUnits(units);
            m_ColorBarBackground = new Color(0.5f, 0.5f, 0.5f);
        }

        public Histogram(Draw2D draw2D, Units units, Color barBackground)
        {
            m_2D = draw2D;
            SetUnits(units);
            m_ColorBarBackground = barBackground;
        }

        public void DrawStart(float width)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(width + 10));

            EditorGUILayout.BeginVertical();
        }

        public void DrawEnd(float width, float min, float max, float spacing)
        {
            EditorGUILayout.BeginHorizontal();
            float halfWidth = width / 2;
            GUIStyle leftAlignStyle = new GUIStyle(GUI.skin.label);
            leftAlignStyle.contentOffset = new Vector2(-5, 0);
            leftAlignStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle rightAlignStyle = new GUIStyle(GUI.skin.label);
            rightAlignStyle.contentOffset = new Vector2(-5, 0);
            rightAlignStyle.alignment = TextAnchor.MiddleRight;
            EditorGUILayout.LabelField(ToDisplayUnits(min), leftAlignStyle, GUILayout.Width(halfWidth));
            EditorGUILayout.LabelField(ToDisplayUnits(max), rightAlignStyle, GUILayout.Width(halfWidth));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        public void DrawBackground(float width, float height, int bucketCount, float spacing)
        {
            float x = (spacing / 2);
            float y = 0;
            float w = ((width + spacing) / bucketCount) - spacing;
            float h = height;

            for (int i = 0; i < bucketCount; i++)
            {
                m_2D.DrawFilledBox(x, y, w, h, m_ColorBarBackground);
                x += w;
                x += spacing;
            }
        }

        public void DrawData(float width, float height, int[] buckets, int totalFrameCount, float min, float max, Color barColor, float spacing)
        {
            float x = (spacing / 2);
            float y = 0;
            float w = ((width + spacing) / buckets.Length) - spacing;
            float h = height;

            int bucketCount = buckets.Length;
            float bucketWidth = ((max - min) / bucketCount);
            Rect rect = GUILayoutUtility.GetLastRect();
            for (int bucketAt = 0; bucketAt < bucketCount; bucketAt++)
            {
                var count = buckets[bucketAt];

                float barHeight = (h * count) / totalFrameCount;
                m_2D.DrawFilledBox(x, y, w, barHeight, barColor);

                float bucketStart = min + (bucketAt * bucketWidth);
                float bucketEnd = bucketStart + bucketWidth;
                GUI.Label(new Rect(rect.x + x, rect.y + y, w, h), new GUIContent("", string.Format("{0}-{1}\n{2} frames", ToDisplayUnits(bucketStart), ToDisplayUnits(bucketEnd,true), count)));

                x += w;
                x += spacing;
            }
        }

        public void Draw(float width, float height, int[] buckets, int totalFrameCount, float min, float max, Color barColor)
        {
            DrawStart(width);

            float spacing = 2;

            if (m_2D.DrawStart(width, height, Draw2D.Origin.BottomLeft))
            {
                DrawBackground(width, height, buckets.Length, spacing);
                DrawData(width, height, buckets, totalFrameCount, min, max, barColor, spacing);
                m_2D.DrawEnd();
            }

            DrawEnd(width, min, max, spacing);
        }
    }
}