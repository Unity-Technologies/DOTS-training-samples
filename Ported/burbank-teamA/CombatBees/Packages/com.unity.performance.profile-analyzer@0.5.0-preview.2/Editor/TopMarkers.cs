using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class TopMarkers
    {
        internal static class Styles
        {
            public static readonly GUIContent menuItemSelectFramesInAll = new GUIContent("Select Frames that contain this marker (within whole data set)", "");
            public static readonly GUIContent menuItemSelectFramesInCurrent = new GUIContent("Select Frames that contain this marker (within current selection)", "");
            public static readonly GUIContent menuItemSelectFramesAll = new GUIContent("Clear Selection", "");
            public static readonly GUIContent menuItemAddToIncludeFilter = new GUIContent("Add to Include Filter", "");
            public static readonly GUIContent menuItemAddToExcludeFilter = new GUIContent("Add to Exclude Filter", "");
            public static readonly GUIContent menuItemRemoveFromIncludeFilter = new GUIContent("Remove from Include Filter", "");
            public static readonly GUIContent menuItemRemoveFromExcludeFilter = new GUIContent("Remove from Exclude Filter", "");
            public static readonly GUIContent menuItemSetAsParentMarkerFilter = new GUIContent("Set as Parent Marker Filter", "");
            public static readonly GUIContent menuItemClearParentMarkerFilter = new GUIContent("Clear Parent Marker Filter", "");
            public static readonly GUIContent menuItemCopyToClipboard = new GUIContent("Copy to Clipboard", "");
        }

        ProfileAnalyzerWindow m_ProfileAnalyzerWindow;
        Draw2D m_2D;
        Color m_BackgroundColor;
        Color m_TextColor;

        public TopMarkers(ProfileAnalyzerWindow profileAnalyzerWindow, Draw2D draw2D, Color backgroundColor, Color textColor)
        {
            m_ProfileAnalyzerWindow = profileAnalyzerWindow;
            m_2D = draw2D;
            m_BackgroundColor = backgroundColor;
            m_TextColor = textColor;
        }
        
        string ToDisplayUnits(float ms, bool showUnits = false, int limitToDigits = 5)
        {
            return m_ProfileAnalyzerWindow.ToDisplayUnits(ms, showUnits, limitToDigits);
        }

        public float GetTopMarkerTimeRange(ProfileAnalysis analysis, int count, int depthFilter)
        {
            if (analysis == null)
                return 0.0f;

            var frameSummary = analysis.GetFrameSummary();
            if (frameSummary == null)
                return 0.0f;

            var markers = analysis.GetMarkers();

            List<string> nameFilters = m_ProfileAnalyzerWindow.GetNameFilters();
            List<string> nameExcludes = m_ProfileAnalyzerWindow.GetNameExcludes();

            float range = 0;
            foreach (var marker in markers)
            {
                if (depthFilter >= 0 && marker.minDepth != depthFilter)
                {
                    continue;
                }

                if (nameFilters.Count > 0)
                {
                    if (!m_ProfileAnalyzerWindow.NameInFilterList(marker.name, nameFilters))
                        continue;
                }
                if (nameExcludes.Count > 0)
                {
                    if (m_ProfileAnalyzerWindow.NameInExcludeList(marker.name, nameExcludes))
                        continue;
                }

                range += marker.msAtMedian;
            }

            // Minimum is the frame time range
            // As we can have unaccounted markers 
            if (range < frameSummary.msMedian)
                range = frameSummary.msMedian;

            return range;
        }

        public void Draw(ProfileAnalysis analysis, Rect rect, Color barColor, int barCount, float timeRange, int depthFilter, Color selectedBackground, Color selectedBorder, Color selectedText, bool includeOthers, bool includeUnaccounted)
        {
            if (analysis == null)
                return;

            FrameSummary frameSummary = analysis.GetFrameSummary();
            if (frameSummary==null)
                return;
            
            var markers = analysis.GetMarkers();
            if (markers==null)
                return;

            // Start by adding frame link button for median frame 
            int buttonWidth = 50;
            int buttonWidthWithoutMargins = buttonWidth - 4;
            Rect buttonRect = new Rect(rect.x, rect.y, buttonWidthWithoutMargins, rect.height);
            m_ProfileAnalyzerWindow.DrawFrameIndexButton(buttonRect, frameSummary.medianFrameIndex);

            // After the marker graph we want an indication of the time range
            int rangeLabelWidth = 60;
            Rect rangeLabelRect = new Rect(rect.x + rect.width - rangeLabelWidth, rect.y, rangeLabelWidth, rect.height);
            string timeRangeString = ToDisplayUnits(timeRange, true);
            string frameTimeString = ToDisplayUnits(frameSummary.msMedian, true, 0);
            string timeRangeTooltip = string.Format("{0} median frame time", frameTimeString);
            GUI.Label(rangeLabelRect, new GUIContent(timeRangeString, timeRangeTooltip) );

            // Reduce the size of the marker graph for the button/label we just added
            rect.x += buttonWidth;
            rect.width -= (buttonWidth + rangeLabelWidth);

            // Show marker graph
            float x = 0;
            float y = 0;
            float width = rect.width;
            float height = rect.height;

            int max = barCount;
            int at = 0;

            var selectedPairingMarkerName = m_ProfileAnalyzerWindow.GetSelectedMarkerName();
            float spacing = 2;

            float other = 0.0f;

            List<string> nameFilters = m_ProfileAnalyzerWindow.GetNameFilters();
            List<string> nameExcludes = m_ProfileAnalyzerWindow.GetNameExcludes();

            if (timeRange <= 0.0f)
                timeRange = frameSummary.msMedian;

            float msToWidth = (width - spacing) / timeRange;

            float totalMarkerTime = 0;
            if (m_2D.DrawStart(rect, Draw2D.Origin.BottomLeft))
            {
                m_2D.DrawFilledBox(x, y, width, height, m_BackgroundColor);

                foreach (var marker in markers)
                {
                    float ms = MarkerData.GetMsAtMedian(marker);
                    totalMarkerTime += ms;

                    if (depthFilter >= 0 && marker.minDepth != depthFilter)
                    {
                        continue;
                    }

                    if (nameFilters.Count > 0)
                    {
                        if (!m_ProfileAnalyzerWindow.NameInFilterList(marker.name, nameFilters))
                            continue;
                    }
                    if (nameExcludes.Count > 0)
                    {
                        if (m_ProfileAnalyzerWindow.NameInExcludeList(marker.name, nameExcludes))
                            continue;
                    }

                    if (at < max)
                    {
                        float w = ms * msToWidth;
                        if (x + w > width)
                            w = width - x;
                        if (marker.name == selectedPairingMarkerName)
                        {
                            m_2D.DrawFilledBox(x + 1, y + 1, w, height - 2, selectedBorder);
                            m_2D.DrawFilledBox(x + 2, y + 2, w - 2, height - 4, selectedBackground);
                        }
                        else
                        {
                            m_2D.DrawFilledBox(x + 2, y + 2, w - 2, height - 4, barColor);
                        }

                        x += w;
                    }
                    else
                    {
                        other += ms;
                        if (!includeOthers)
                            break;
                    }

                    at++;
                }

                if (includeOthers && other > 0.0f)
                {
                    x += DrawBar(x, y, other, msToWidth, width, height, barColor);
                }
                if (includeUnaccounted && totalMarkerTime < frameSummary.msMedian)
                {
                    float unaccounted = frameSummary.msMedian - totalMarkerTime;
                    Color color = new Color(barColor.r * 0.5f, barColor.g * 0.5f, barColor.b * 0.5f, barColor.a);
                    x += DrawBar(x, y, unaccounted, msToWidth, width, height, color);
                }

                m_2D.DrawEnd();
            }
            else if (includeOthers)
            {
                // Need to calculate the size of the others for the input phase if not drawing at this time
                at = 0;
                foreach (var marker in markers)
                {
                    float ms = MarkerData.GetMsAtMedian(marker);
                    totalMarkerTime += ms;

                    if (depthFilter >= 0 && marker.minDepth != depthFilter)
                    {
                        continue;
                    }

                    if (nameFilters.Count > 0)
                    {
                        if (!m_ProfileAnalyzerWindow.NameInFilterList(marker.name, nameFilters))
                            continue;
                    }
                    if (nameExcludes.Count > 0)
                    {
                        if (m_ProfileAnalyzerWindow.NameInExcludeList(marker.name, nameExcludes))
                            continue;
                    }

                    if (at >= max)
                    {
                        other += ms;
                        if (!includeOthers)
                            break;
                    }

                    at++;
                }
            }

            at = 0;
            x = 0.0f;
            GUIStyle centreAlignStyle = new GUIStyle(GUI.skin.label);
            centreAlignStyle.alignment = TextAnchor.MiddleCenter;
            centreAlignStyle.normal.textColor = m_TextColor;
            GUIStyle leftAlignStyle = new GUIStyle(GUI.skin.label);
            leftAlignStyle.alignment = TextAnchor.MiddleLeft;
            leftAlignStyle.normal.textColor = m_TextColor;
            Color contentColor = GUI.contentColor;

            for (int index = 0; index < markers.Count; index++)
            {
                var marker = markers[index];
                if (depthFilter >= 0 && marker.minDepth != depthFilter)
                {
                    continue;
                }

                if (nameFilters.Count > 0)
                {
                    if (!m_ProfileAnalyzerWindow.NameInFilterList(marker.name, nameFilters))
                        continue;
                }
                if (nameExcludes.Count > 0)
                {
                    if (m_ProfileAnalyzerWindow.NameInExcludeList(marker.name, nameExcludes))
                        continue;
                }

                if (at < max)
                {
                    float w = MarkerData.GetMsAtMedian(marker) * msToWidth;
                    if (x + w > width)
                        w = width - x;

                    Rect labelRect = new Rect(rect.x + x, rect.y, w, rect.height);
                    GUIStyle style = centreAlignStyle;
                    String displayName = "";
                    if (w >= 20)
                    {
                        displayName = marker.name;
                        Vector2 size = centreAlignStyle.CalcSize(new GUIContent(marker.name));
                        if (size.x > w)
                        {
                            var words = marker.name.Split('.');
                            displayName = words[words.Length - 1];
                            style = leftAlignStyle;
                        }
                    }
                    float percentAtMedian = MarkerData.GetMsAtMedian(marker) * 100 / timeRange;
                    string tooltip = string.Format("{0}\n{1:f2}% ({2} on median frame {3})\n\nMedian marker time (in currently selected frames)\n{4} on frame {5}",
                        marker.name,
                        percentAtMedian, ToDisplayUnits(marker.msAtMedian, true, 0), frameSummary.medianFrameIndex,
                        ToDisplayUnits(marker.msMedian, true, 0), marker.medianFrameIndex);
                    if (marker.name == selectedPairingMarkerName)
                        style.normal.textColor = selectedText;
                    else
                        style.normal.textColor = m_TextColor;
                    GUI.Label(labelRect, new GUIContent(displayName, tooltip), style);

                    Event current = Event.current;
                    if (labelRect.Contains(current.mousePosition))
                    {
                        if (current.type == EventType.ContextClick)
                        {
                            GenericMenu menu = new GenericMenu();

                            menu.AddItem(Styles.menuItemSelectFramesInAll, false, () => m_ProfileAnalyzerWindow.SelectFramesContainingMarker(marker.name, false));
                            menu.AddItem(Styles.menuItemSelectFramesInCurrent, false, () => m_ProfileAnalyzerWindow.SelectFramesContainingMarker(marker.name, true));
                            menu.AddItem(Styles.menuItemSelectFramesAll, false, m_ProfileAnalyzerWindow.SelectAllFrames);
                            menu.AddSeparator("");
                            if (!m_ProfileAnalyzerWindow.GetNameFilters().Contains(marker.name))
                                menu.AddItem(Styles.menuItemAddToIncludeFilter, false, () => m_ProfileAnalyzerWindow.AddToIncludeFilter(marker.name));
                            else
                                menu.AddItem(Styles.menuItemRemoveFromIncludeFilter, false, () => m_ProfileAnalyzerWindow.RemoveFromIncludeFilter(marker.name));
                            if (!m_ProfileAnalyzerWindow.GetNameExcludes().Contains(marker.name))
                                menu.AddItem(Styles.menuItemAddToExcludeFilter, false, () => m_ProfileAnalyzerWindow.AddToExcludeFilter(marker.name));
                            else
                                menu.AddItem(Styles.menuItemRemoveFromExcludeFilter, false, () => m_ProfileAnalyzerWindow.RemoveFromExcludeFilter(marker.name));
                            menu.AddSeparator("");
                            menu.AddItem(Styles.menuItemSetAsParentMarkerFilter, false, () => m_ProfileAnalyzerWindow.SetAsParentMarkerFilter(marker.name));
                            menu.AddItem(Styles.menuItemClearParentMarkerFilter, false, () => m_ProfileAnalyzerWindow.SetAsParentMarkerFilter(""));
                            menu.AddSeparator("");
                            menu.AddItem(Styles.menuItemCopyToClipboard, false, () => CopyToClipboard(current, marker.name));

                            menu.ShowAsContext();

                            current.Use();
                        }
                        if (current.type == EventType.MouseDown)
                        {
                            m_ProfileAnalyzerWindow.SelectMarker(marker.name);
                            m_ProfileAnalyzerWindow.RequestRepaint();
                        }
                    }

                    x += w;
                }
                else
                {
                    break;
                }

                at++;
            }

            if (includeOthers)
            {
                x += DrawBarText(rect, x, other, "Others", msToWidth, timeRange, leftAlignStyle, frameSummary.medianFrameIndex);
            }
            if (includeUnaccounted && totalMarkerTime < frameSummary.msMedian)
            {
                float unaccounted = frameSummary.msMedian - totalMarkerTime;
                x += DrawBarText(rect, x, unaccounted, "Unaccounted", msToWidth, timeRange, leftAlignStyle, frameSummary.medianFrameIndex);
            }
        }

        float DrawBar(float x, float y, float msTime, float msToWidth, float width, float height, Color barColor)
        {
            float w = msTime * msToWidth;
            if (x + w > width)
                w = width - x;
            m_2D.DrawFilledBox(x + 2, y + 2, w - 2, height - 4, barColor);

            return w;
        }

        float DrawBarText(Rect rect, float x, float msTime, string name, float msToWidth, float timeRange, GUIStyle leftAlignStyle, int medianFrameIndex)
        {
            float width = rect.width;
            float w = msTime * msToWidth;
            if (x + w > width)
                w = width - x;
            Rect labelRect = new Rect(rect.x + x, rect.y, w, rect.height);
            float percent = msTime / timeRange * 100;
            GUIStyle style = leftAlignStyle;
            string tooltip = string.Format("{0}\n{1:f2}% ({2} on median frame {3})",
                name,
                percent, 
                ToDisplayUnits(msTime, true, 0), 
                medianFrameIndex);
            GUI.Label(labelRect, new GUIContent("", tooltip), style);

            Event current = Event.current;
            if (labelRect.Contains(current.mousePosition))
            {
                if (current.type == EventType.ContextClick)
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(Styles.menuItemSelectFramesAll, false, m_ProfileAnalyzerWindow.SelectAllFrames);
                    menu.ShowAsContext();

                    current.Use();
                }
                if (current.type == EventType.MouseDown)
                {
                    m_ProfileAnalyzerWindow.SelectMarker(null);
                    m_ProfileAnalyzerWindow.RequestRepaint();
                }
            }

            return w;
        }
        
        void CopyToClipboard(Event current, string text)
        {
            EditorGUIUtility.systemCopyBuffer = text;
        }
    }
}
