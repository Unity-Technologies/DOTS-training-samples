using System;
using UnityEngine;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class TopMarkerList
    {
        internal static class Styles
        {
            public static readonly GUIContent frameCosts = new GUIContent(" by frame costs", "Contains accumulated marker cost within the frame");
            public static readonly GUIContent frameCounts = new GUIContent(" by frame counts", "Contains marker count within the frame");
        }

        public delegate void DrawFrameIndexButton(int index);

        DrawFrameIndexButton m_DrawFrameIndexButton;
        Draw2D m_2D;
        DisplayUnits m_Units;
        int m_WidthColumn0;
        int m_WidthColumn1;
        int m_WidthColumn2;
        Color colorBar;
        Color colorBarBackground;
        
        public TopMarkerList(Draw2D draw2D, Units units, 
            int widthColumn0, int widthColumn1, int widthColumn2,
            Color colorBar, Color colorBarBackground, DrawFrameIndexButton drawFrameIndexButton)
        {
            m_2D = draw2D;
            SetUnits(units);
            m_WidthColumn0 = widthColumn0;
            m_WidthColumn1 = widthColumn1;
            m_WidthColumn2 = widthColumn2;
            this.colorBar= colorBar;
            this.colorBarBackground = colorBarBackground;
            m_DrawFrameIndexButton = drawFrameIndexButton;
        }
        
        void SetUnits(Units units)
        {
            m_Units = new DisplayUnits(units);
        }
        
        string ToDisplayUnits(float ms, bool showUnits = false, int limitToDigits = 5)
        {
            return m_Units.ToString(ms, showUnits, limitToDigits);
        }
        
        GUIContent ToDisplayUnitsWithTooltips(float ms, bool showUnits = false, int frameIndex = -1)
        {
            if (frameIndex>=0)
                return new GUIContent(ToDisplayUnits(ms, showUnits), string.Format("{0} on frame {1}", ToDisplayUnits(ms, true, 0), frameIndex));

            return new GUIContent(ToDisplayUnits(ms, showUnits), ToDisplayUnits(ms, true, 0));
        }
        
        public int DrawTopNumber(int topNumber, string[] topStrings, int[] topValues)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Top ", GUILayout.Width(30));
            topNumber = EditorGUILayout.IntPopup(topNumber, topStrings, topValues, GUILayout.Width(30));
            EditorGUILayout.LabelField(m_Units.Units==Units.Count ? Styles.frameCounts : Styles.frameCosts, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            return topNumber;
        }

        public int Draw(MarkerData marker, int topNumber, string[] topStrings, int[] topValues)
        {
            GUIStyle style = GUI.skin.label;
            float w = m_WidthColumn0;
            float h = style.lineHeight;
            float ySpacing = 2;
            float barHeight = h - ySpacing;

            EditorGUILayout.BeginVertical(GUILayout.Width(w + m_WidthColumn1 + m_WidthColumn2));

            topNumber = DrawTopNumber(topNumber, topStrings, topValues);

            /*
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(w));
            EditorGUILayout.LabelField("Value", GUILayout.Width(LayoutSize.WidthColumn1));
            EditorGUILayout.LabelField("Frame", GUILayout.Width(LayoutSize.WidthColumn2));
            EditorGUILayout.EndHorizontal();
            */

            // var frameSummary = m_ProfileSingleView.analysis.GetFrameSummary();
            float barMax = marker.msMax; // frameSummary.msMax
            if (m_Units.Units == Units.Count)
            {
                barMax = marker.countMax;
            }
            FrameTime zeroTime = new FrameTime(0,0.0f,0);

            int index = marker.frames.Count - 1;
            for (int i = 0; i < topNumber; i++)
            {
                FrameTime frameTime = new FrameTime((index >= 0 ) ? marker.frames[index] : zeroTime);
                float barValue = (m_Units.Units == Units.Count) ? frameTime.count : frameTime.ms;
                float barLength = Math.Min((w * barValue) / barMax, w);

                EditorGUILayout.BeginHorizontal();
                if (m_2D.DrawStart(w, h, Draw2D.Origin.TopLeft, style))
                {
                    if (i < marker.frames.Count)
                    {
                        m_2D.DrawFilledBox(0, ySpacing, barLength, barHeight, colorBar);
                        m_2D.DrawFilledBox(barLength, ySpacing, w - barLength, barHeight, colorBarBackground);
                    }
                    m_2D.DrawEnd();

                    Rect rect = GUILayoutUtility.GetLastRect();
                    GUI.Label(rect, new GUIContent("", m_Units.ToString(barValue, true, 5)));
                }
                if (i < marker.frames.Count)
                { 
                    EditorGUILayout.LabelField(ToDisplayUnitsWithTooltips(barValue,true), GUILayout.Width(m_WidthColumn2));
                    if (m_DrawFrameIndexButton!=null)
                        m_DrawFrameIndexButton(marker.frames[index].frameIndex);
                }
                EditorGUILayout.EndHorizontal();

                index--;
            }

            EditorGUILayout.EndVertical();

            return topNumber;
        }
    }
}
