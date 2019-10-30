using UnityEngine;
using System;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class BoxAndWhiskerPlot
    {
        Draw2D m_2D;
        Color m_ColorBackground;
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

        public BoxAndWhiskerPlot(Draw2D draw2D, Units units, Color colorBackground)
        {
            m_2D = draw2D;
            SetUnits(units);
            m_ColorBackground = colorBackground;
        }

        public BoxAndWhiskerPlot(Draw2D draw2D, Units units)
        {
            m_2D = draw2D;
            SetUnits(units);
            m_ColorBackground = new Color(0.4f, 0.4f, 0.4f);
        }

        private float ClampToRange(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public void Draw(float width, float height, float min, float lowerQuartile, float median, float upperQuartile, float max, float yAxisStart, float yAxisEnd, Color color, Color colorFilled)
        {
            if (m_2D.DrawStart(width, height, Draw2D.Origin.BottomLeft))
            {
                Rect rect = GUILayoutUtility.GetLastRect();

                float x = 0;
                float y = 0;
                float w = width;
                float h = height;

                Draw(rect, x, y, w, h, min, lowerQuartile, median, upperQuartile, max, yAxisStart, yAxisEnd, color, colorFilled);
                m_2D.DrawEnd();
            }
        }

        private string GetTooltip(float min, float lowerQuartile, float median, float upperQuartile, float max)
        {
            string tooltip = string.Format(
                "Max :\t\t{0}\n\nUpper Quartile :\t{1}\nMedian :\t\t{2}\nLower Quartile :\t{3}\nInterquartile range : \t{4}\n\nMin :\t\t{5}\nUnits :\t\t{6}",
                ToDisplayUnits(max),
                ToDisplayUnits(upperQuartile),
                ToDisplayUnits(median),
                ToDisplayUnits(lowerQuartile),
                ToDisplayUnits(upperQuartile - lowerQuartile),
                ToDisplayUnits(min),
                m_Units.Postfix()
                );

            return tooltip;
        }

        public void Draw(Rect rect, float x, float y, float w, float h, float min, float lowerQuartile, float median, float upperQuartile, float max, float yAxisStart, float yAxisEnd, Color color, Color colorFilled, bool clearFirst = true)
        {
            string tooltip = GetTooltip(min, lowerQuartile, median, upperQuartile, max);
            GUI.Label(rect, new GUIContent("", tooltip));

            if (clearFirst)
                m_2D.DrawFilledBox(x, y, w, h, m_ColorBackground);

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

            float hMax = h - 1;
            float yMin = hMax * (min - first) / range;
            float yLowerQuartile = hMax * (lowerQuartile - first) / range;
            float yMedian = hMax * (median - first) / range;
            float yUpperQuartile = hMax * (upperQuartile - first) / range;
            float yMax = hMax * (max - first) / range;

            // Min to max line
            float xCentre = x + (w / 2);
            m_2D.DrawLine(xCentre, y + yMin, xCentre, y + yLowerQuartile, color);
            m_2D.DrawLine(xCentre, y + yUpperQuartile, xCentre, y + yMax, color);

            // Quartile boxes
            float xMargin = (2 * w / 8);
            float x1 = x + xMargin;
            float x2 = x + (w - xMargin);
            float wBox = x2 - x1;
            if (colorFilled != color)
                m_2D.DrawFilledBox(x1, y + yLowerQuartile, wBox, (yMedian - yLowerQuartile), colorFilled);
            m_2D.DrawBox(x1, y + yLowerQuartile, wBox, (yMedian - yLowerQuartile), color);
            if (colorFilled != color)
                m_2D.DrawFilledBox(x1, y + yMedian, wBox, (yUpperQuartile - yMedian), colorFilled);
            m_2D.DrawBox(x1, y + yMedian, wBox, (yUpperQuartile - yMedian), color);

            // Median line
            //xMargin = (1 * w / 8);
            //x1 = x + xMargin;
            //x2 = x + (w - xMargin);
            m_2D.DrawLine(x1, y + yMedian, x2, y + yMedian, color);
            m_2D.DrawLine(x1, y + yMedian + 1, x2, y + yMedian + 1, color);

            // Line caps
            xMargin = (3 * w / 8);
            x1 = x + xMargin;
            x2 = x + (w - xMargin);
            if (startCap)
                m_2D.DrawLine(x1, y + yMin, x2, y + yMin, color);

            if (endCap)
                m_2D.DrawLine(x1, y + yMax, x2, y + yMax, color);
        }

        public void DrawHorizontal(float width, float height, float min, float lowerQuartile, float median, float upperQuartile, float max, float xAxisStart, float xAxisEnd, Color color, Color colorFilled, GUIStyle style = null)
        {
            if (m_2D.DrawStart(width, height, Draw2D.Origin.BottomLeft, style))
            {
                Rect rect = GUILayoutUtility.GetLastRect();

                float x = 0;
                float y = 0;
                float w = width;
                float h = height;

                DrawHorizontal(rect, x, y, w, h, min, lowerQuartile, median, upperQuartile, max, xAxisStart, xAxisEnd, color, colorFilled);
                m_2D.DrawEnd();
            }
        }

        public void DrawHorizontal(Rect rect, float x, float y, float w, float h, float min, float lowerQuartile, float median, float upperQuartile, float max, float xAxisStart, float xAxisEnd, Color color, Color colorFilled, bool clearFirst = true)
        {
            string tooltip = GetTooltip(min, lowerQuartile, median, upperQuartile, max);
            GUI.Label(rect, new GUIContent("", tooltip));

            if (clearFirst)
                m_2D.DrawFilledBox(x, y, w, h, m_ColorBackground);

            float first = xAxisStart;
            float last = xAxisEnd;
            float range = last - first;

            bool startCap = (min >= first) ? true : false;
            bool endCap = (max <= last) ? true : false;

            // Range clamping
            min = ClampToRange(min, first, last);
            lowerQuartile = ClampToRange(lowerQuartile, first, last);
            median = ClampToRange(median, first, last);
            upperQuartile = ClampToRange(upperQuartile, first, last);
            max = ClampToRange(max, first, last);

            float xMin = w * (min - first) / range;
            float xLowerQuartile = w * (lowerQuartile - first) / range;
            float xMedian = w * (median - first) / range;
            float xUpperQuartile = w * (upperQuartile - first) / range;
            float xMax = w * (max - first) / range;

            // Min to max line
            m_2D.DrawLine(x + xMin, y + (h / 2), x + xMax, y + (h / 2), color);

            // Quartile boxes
            float yMargin = (2 * h / 8);
            float y1 = y + yMargin;
            float y2 = y + (h - yMargin);
            float hBox = y2 - y1;
            if (colorFilled != color)
                m_2D.DrawFilledBox(x + xLowerQuartile, y1, xMedian - xLowerQuartile, hBox, colorFilled);
            m_2D.DrawBox(x + xLowerQuartile, y1, xMedian - xLowerQuartile, hBox, color);
            if (colorFilled != color)
                m_2D.DrawFilledBox(x + xMedian, y1, xUpperQuartile - xMedian, hBox, colorFilled);
            m_2D.DrawBox(x + xMedian, y1, xUpperQuartile - xMedian, hBox, color);

            // Median line
            //yMargin = (1 * h / 8);
            //y1 = y + yMargin;
            //y2 = y + (h - yMargin);
            m_2D.DrawLine(x + xMedian, y1, x + xMedian, y2, color);
            m_2D.DrawLine(x + xMedian + 1, y1, x + xMedian + 1, y2, color);

            // Line caps
            yMargin = (3 * h / 8);
            y1 = y + yMargin;
            y2 = y + (h - yMargin);
            if (startCap)
                m_2D.DrawLine(x + xMin, y1, x + xMin, y2, color);

            if (endCap)
                m_2D.DrawLine(x + xMax, y1, x + xMax, y2, color);
        }

        public void DrawText(float width, float plotHeight, float min, float max, string minTooltip, string maxTooltip)
        {
            GUIStyle shiftUpStyle = new GUIStyle(GUI.skin.label);
            shiftUpStyle.contentOffset = new Vector2(0, -5);
            EditorGUILayout.BeginVertical(GUILayout.Height(plotHeight));
            EditorGUILayout.LabelField(new GUIContent(ToDisplayUnits(max), maxTooltip), shiftUpStyle, GUILayout.Width(width));
            GUILayout.FlexibleSpace();
            GUIStyle shiftDownStyle = new GUIStyle(GUI.skin.label);
            shiftDownStyle.contentOffset = new Vector2(0, 2);
            EditorGUILayout.LabelField(new GUIContent(ToDisplayUnits(min), minTooltip), shiftDownStyle, GUILayout.Width(width));
            EditorGUILayout.EndVertical();
        }
    }
}