
using UnityEngine;
using System;
using System.Collections.Generic;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    [Serializable]
    public class FrameTimeGraphGlobalSettings
    {
        public bool showThreads = false;
        public bool showSelectedMarker = true;
        public bool showFrameLines = true;
        public bool showFrameLineText = true;
        public bool showOrderedByFrameDuration = false;
    }

    public class FrameTimeGraph
    {
        static FrameTimeGraphGlobalSettings m_GlobalSettings = new FrameTimeGraphGlobalSettings();

        static public void SetGlobalSettings(FrameTimeGraphGlobalSettings globalSettings)
        {
            m_GlobalSettings = globalSettings;
        }

        public struct Data
        {
            public readonly float ms;
            public readonly int frameOffset;

            public Data(float _ms, int _index)
            {
                ms = _ms;
                frameOffset = _index;
            }
        };

        public delegate void SetRange(List<int> selected, int clickCount, bool singleControlAction, FrameTimeGraph.State inputStatus);

        public enum State
        {
            None,
            Dragging,
            DragComplete
        };

        enum DragDirection
        {
            Start,
            Forward,
            Backward,
            None
        };

        enum AxisMode
        {
            One60HzFrame,
            Two60HzFrames,
            Four60HzFrames,
            Max,
            Custom
        };

        Draw2D m_2D;
        int m_DragBeginFirstOffset;
        int m_DragBeginLastOffset;

        bool m_Dragging;
        int m_DragFirstOffset;
        int m_DragLastOffset;
        bool m_Moving;
        int m_MoveHandleOffset;
        bool m_SingleControlAction;

        int m_ClickCount;
        double m_LastClickTime;
        bool m_MouseReleased;

        bool m_Zoomed;
        int m_ZoomStartOffset;
        int m_ZoomEndOffset;

        Color m_ColorBarBackground;
        Color m_ColorBarBackgroundSelected;
        Color m_ColorBar;
        Color m_ColorBarOutOfRange;
        Color m_ColorBarSelected;
        Color m_ColorBarThreads;
        Color m_ColorBarThreadsOutOfRange;
        Color m_ColorBarThreadsSelected;
        Color m_ColorBarMarker;
        Color m_ColorBarMarkerOutOfRange;
        Color m_ColorBarMarkerSelected;
        Color m_ColorGridLine;

        FrameTimeGraph m_PairedWithFrameTimeGraph;

        internal static class Styles
        {
            public static readonly GUIContent menuItemClearSelection = new GUIContent("Clear Selection");
            public static readonly GUIContent menuItemInvertSelection = new GUIContent("Invert Selection");
            public static readonly GUIContent menuItemZoomSelection = new GUIContent("Zoom Selection");
            public static readonly GUIContent menuItemZoomAll = new GUIContent("Zoom All");
            public static readonly GUIContent menuItemSelectMin = new GUIContent("Select Shortest Frame");
            public static readonly GUIContent menuItemSelectMax = new GUIContent("Select Longest Frame");
            public static readonly GUIContent menuItemSelectMedian = new GUIContent("Select Median Frame");

//            public static readonly GUIContent menuItemSelectPrevious = new GUIContent("Select previous frame (cursor left)");
//            public static readonly GUIContent menuItemSelectNext = new GUIContent("Select next frame (cursor right)");
            public static readonly GUIContent menuItemSelectGrow = new GUIContent("Grow selection (+), hold SHIFT for faster");
            public static readonly GUIContent menuItemSelectGrowLeft = new GUIContent("Grow selection left (<), hold ALT for reverse");
            public static readonly GUIContent menuItemSelectGrowRight = new GUIContent("Grow selection right (>)");
            public static readonly GUIContent menuItemSelectShrink = new GUIContent("Shrink selection (-)");

            public static readonly GUIContent menuItemShowSelectedMarker = new GUIContent("Show Selected Marker");
            public static readonly GUIContent menuItemShowThreads = new GUIContent("Show Filtered Threads");
            //    public static readonly GUIContent menuItemDetailedMode = new GUIContent("Detailed mode");
            public static readonly GUIContent menuItemShowFrameLines = new GUIContent("Show Frame Lines");
            public static readonly GUIContent menuItemShowFrameLineText = new GUIContent("Show Frame Line Text");
            public static readonly GUIContent menuItemShowOrderedByFrameDuration = new GUIContent("Order by Frame Duration");
        }

        const int kXAxisWidth = 60;
        const int kYAxisDetailThreshold = 40;
        const int kOverrunHeight = 3;

        static AxisMode s_YAxisMode;
        static float m_YAxisMs;

        bool m_IsOrderedByFrameDuration;

        List<Data> m_Values = new List<Data> { };
        int[] m_FrameOffsetToDataOffsetMapping = new int[] { };
        SetRange m_SetRange;

        List<int> m_CurrentSelection = new List<int>();
        int m_CurrentSelectionFirstDataOffset;
        int m_CurrentSelectionLastDataOffset;

        int m_GraphId;
        static int s_NextGraphId = 0;
        static int s_LastSelectedGraphId = -1;

        bool m_Enabled;

        struct BarData
        {
            public float x;
            public float y;
            public float w;
            public float h;

            public int startDataOffset;
            public int endDataOffset;
            public float yMin;
            public float yMax;

            public BarData(float _x, float _y, float _w, float _h, int _startDataOffset, int _endDataOffset, float _yMin, float _yMax)
            {
                x = _x;
                y = _y;
                w = _w;
                h = _h;
                startDataOffset = _startDataOffset;
                endDataOffset = _endDataOffset;
                yMin = _yMin;
                yMax = _yMax;
            }
        }

        List<BarData> m_Bars = new List<BarData>();

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

        public void Reset()
        {
            m_Zoomed = false;
            m_Dragging = false;
            ClearDragSelection();

            m_Moving = false;
            m_ClickCount = 0;
            m_MouseReleased = false;
        }

        private void Init()
        {
            Reset();

            m_PairedWithFrameTimeGraph = null;
            m_YAxisMs = 100f;
            s_YAxisMode = AxisMode.Max;
            m_IsOrderedByFrameDuration = false;

            m_Enabled = true;
        }

        int GetGraphIDAndIncrement()
        {
            int id = s_NextGraphId;

            s_NextGraphId++;
            if (s_NextGraphId < 0)
                s_NextGraphId = 0;

            return id;
        }

        public FrameTimeGraph(Draw2D draw2D, Units units, Color background, Color backgroundSelected, Color barColor, Color barSelected, Color barMarker, Color barMarkerSelected, Color barThreads, Color barThreadsSelected, Color colorGridlines)
        {
            m_GraphId = GetGraphIDAndIncrement();

            m_2D = draw2D;
            SetUnits(units);
            Init();

            float ratio = 0.75f;
            m_ColorBarBackground = background;
            m_ColorBarBackgroundSelected = backgroundSelected;

            m_ColorBar = barColor;
            m_ColorBarOutOfRange = new Color(barColor.r * ratio, barColor.g * ratio, barColor.b * ratio);
            m_ColorBarSelected = barSelected;

            m_ColorBarMarker = barMarker;
            m_ColorBarMarkerOutOfRange = new Color(barMarker.r * ratio, barMarker.g * ratio, barMarker.b * ratio);
            m_ColorBarMarkerSelected = barMarkerSelected; 

            m_ColorBarThreads = barThreads;
            m_ColorBarThreadsOutOfRange = new Color(barThreads.r * ratio, barThreads.g * ratio, barThreads.b * ratio);
            m_ColorBarThreadsSelected = barThreadsSelected;

            m_ColorGridLine = colorGridlines;
        }

        private int ClampToRange(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;

            return value;
        }

        private int GetDataOffsetForXUnclamped(int xPosition, int width, int totalDataSize)
        {
            int visibleDataSize;
            if (m_Zoomed)
                visibleDataSize = (m_ZoomEndOffset - m_ZoomStartOffset) + 1;
            else
                visibleDataSize = totalDataSize;

            int dataOffset = (int)(xPosition * visibleDataSize / width);

            if (m_Zoomed)
                dataOffset += m_ZoomStartOffset;

            return dataOffset;
        }


        private int GetDataOffsetForX(int xPosition, int width, int totalDataSize)
        {
            //xPosition = ClampToRange(xPosition, 0, width-1);
            int dataOffset = GetDataOffsetForXUnclamped(xPosition, width, totalDataSize);
            return ClampToRange(dataOffset, 0, totalDataSize - 1);
        }

        private int GetXForDataOffset(int dataOffset, int width, int totalDataSize)
        {
            //frameOffset = ClampToRange(frameOffset, 0, frames-1);

            int visibleDataSize;
            if (m_Zoomed)
            {
                dataOffset = ClampToRange(dataOffset, m_ZoomStartOffset, m_ZoomEndOffset+1);
                dataOffset -= m_ZoomStartOffset;
                visibleDataSize = (m_ZoomEndOffset - m_ZoomStartOffset) + 1;
            }
            else
                visibleDataSize = totalDataSize;

            int x = (int)(dataOffset * width / visibleDataSize);

            x = ClampToRange(x, 0, width-1);
            return x;
        }

        private void SetDragMovement(int startOffset, int endOffset, int currentSelectionFirstDataOffset, int currentSelectionLastDataOffset)
        {
            // Maintain length but clamp to range
            int frames = m_Values.Count;

            int currentSelectionRange = currentSelectionLastDataOffset - currentSelectionFirstDataOffset;
            endOffset = startOffset + currentSelectionRange;

            startOffset = ClampToRange(startOffset, 0, frames - (currentSelectionRange + 1));
            endOffset = ClampToRange(endOffset, 0, frames - 1);

            SetDragSelection(startOffset, endOffset);

            if (m_PairedWithFrameTimeGraph != null && !m_SingleControlAction)
            {
                m_PairedWithFrameTimeGraph.SetDragSelection(m_DragFirstOffset, m_DragLastOffset);
            }
        }

        private void SetDragSelection(int startOffset, int endOffset, DragDirection dragDirection)
        {
            // No need to clamp these as input is clamped.
            switch (dragDirection)
            {
                case DragDirection.Forward:
                    SetDragSelection(m_DragBeginFirstOffset, endOffset);
                    break;

                case DragDirection.Backward:
                    SetDragSelection(startOffset, m_DragBeginLastOffset);
                    break;

                case DragDirection.Start:
                    SetDragSelection(startOffset, endOffset);

                    // Record first selected bar range
                    m_DragBeginFirstOffset = m_DragFirstOffset;
                    m_DragBeginLastOffset = m_DragLastOffset;
                    break;
            }

            if (m_PairedWithFrameTimeGraph != null && !m_SingleControlAction)
            {
                m_PairedWithFrameTimeGraph.SetDragSelection(m_DragFirstOffset, m_DragLastOffset);
            }
        }

        public void SetDragSelection(int startOffset, int endOffset)
        {
            m_DragFirstOffset = startOffset;
            m_DragLastOffset = endOffset;
        }

        public void ClearDragSelection()
        {
            m_DragFirstOffset = -1;
            m_DragLastOffset = -1;
        }

        public bool HasDragRegion()
        {
            return (m_DragFirstOffset != -1);
        }

        public void GetSelectedRange(List<int> frameOffsets, out int firstDataOffset, out int lastDataOffset, out int firstFrameOffset, out int lastFrameOffset)
        {
            int frames = m_Values != null ? m_Values.Count : 0;

            firstDataOffset = 0;
            lastDataOffset = frames - 1;
            firstFrameOffset = 0;
            lastFrameOffset = frames - 1;

            if (m_FrameOffsetToDataOffsetMapping.Length > 0)
            {
                // By default data is ordered by index so first/last will be the selected visible range

                if (frameOffsets.Count >= 1)
                {
                    firstFrameOffset = frameOffsets[0];
                    lastFrameOffset = firstFrameOffset;

                    firstDataOffset = GetDataOffset(firstFrameOffset);
                    lastDataOffset = firstDataOffset;
                }
                if (frameOffsets.Count >= 2)
                {
                    lastFrameOffset = frameOffsets[frameOffsets.Count - 1];

                    lastDataOffset = GetDataOffset(lastFrameOffset);
                }

                if (m_GlobalSettings.showOrderedByFrameDuration)
                {
                    // Need to find the selected items with lowest and highest ms values
                    if (frameOffsets.Count > 0)
                    {
                        int dataOffset = GetDataOffset(firstFrameOffset);

                        firstDataOffset = dataOffset;
                        lastDataOffset = dataOffset;
                        float firstDataMS = m_Values[dataOffset].ms;
                        float lastDataMS = m_Values[dataOffset].ms;

                        foreach (int frameOffset in frameOffsets)
                        {
                            dataOffset = GetDataOffset(frameOffset);

                            float ms = m_Values[dataOffset].ms;
                            if (ms <= firstDataMS && dataOffset < firstDataOffset)
                            {
                                firstDataMS = ms;
                                firstDataOffset = dataOffset;
                            }
                            if (ms >= lastDataMS && dataOffset > lastDataOffset)
                            {
                                lastDataMS = ms;
                                lastDataOffset = dataOffset;
                            }
                        }
                    }
                }
            }
        }

        public State ProcessInput(Rect rect, List<int> selectedFrameOffsets, int maxFrames = 0)
        {
            if (!IsEnabled())
                return State.None;

            if (m_Values == null)
                return State.None;

            int dataLength = m_Values.Count;
            if (dataLength <= 0)
                return State.None;

            if (m_IsOrderedByFrameDuration != m_GlobalSettings.showOrderedByFrameDuration)
            {
                // Reorder if necessary
                SetData(m_Values);
            }

            int currentSelectionFirstDataOffset;
            int currentSelectionLastDataOffset;
            int currentSelectionFirstFrameOffset;
            int currentSelectionLastFrameOffset;

            GetSelectedRange(selectedFrameOffsets, out currentSelectionFirstDataOffset, out currentSelectionLastDataOffset, out currentSelectionFirstFrameOffset, out currentSelectionLastFrameOffset);

            m_CurrentSelection.Clear();
            m_CurrentSelection.AddRange(selectedFrameOffsets);
            m_CurrentSelectionFirstDataOffset = currentSelectionFirstDataOffset;
            m_CurrentSelectionLastDataOffset = currentSelectionLastDataOffset;

            if (Event.current.isKey && Event.current.type==EventType.KeyDown && s_LastSelectedGraphId == m_GraphId && !m_Dragging && !m_MouseReleased)
            {
                int step = Event.current.shift ? 10 : 1;
                switch (Event.current.keyCode)
                {
                    case KeyCode.LeftArrow:
                        SelectPrevious(step);
                        break;
                    case KeyCode.RightArrow:
                        SelectNext(step);
                        break;
                    case KeyCode.Less:
                    case KeyCode.Comma:
                        if (Event.current.alt)
                            SelectShrinkLeft(step);
                        else
                            SelectGrowLeft(step);
                        break;
                    case KeyCode.Greater:
                    case KeyCode.Period:
                        if (Event.current.alt)
                            SelectShrinkRight(step);
                        else
                            SelectGrowRight(step);
                        break;
                    case KeyCode.Plus:
                    case KeyCode.Equals:
                    case KeyCode.KeypadPlus:
                        if (Event.current.alt)
                            SelectShrink(step);
                        else
                            SelectGrow(step);
                        break;
                    case KeyCode.Underscore:
                    case KeyCode.Minus:
                    case KeyCode.KeypadMinus:
                        if (Event.current.alt)
                            SelectGrow(step);
                        else
                            SelectShrink(step);
                        break;
                }
            }

            float doubleClickTimeout = 0.25f;
            if (m_MouseReleased)
            {
                if ((EditorApplication.timeSinceStartup - m_LastClickTime) > doubleClickTimeout)
                {
                    // By this point we will know if its a single or double click
                    CallSetRange(m_DragFirstOffset, m_DragLastOffset, m_ClickCount, m_SingleControlAction, FrameTimeGraph.State.DragComplete);

                    ClearDragSelection();
                    if (m_PairedWithFrameTimeGraph != null && !m_SingleControlAction)
                        m_PairedWithFrameTimeGraph.ClearDragSelection();

                    m_MouseReleased = false;
                }
            }

            int width = (int)rect.width;
            int height = (int)rect.height;
            float xStart = rect.xMin;
            if (height > kYAxisDetailThreshold)
            {
                float h = GUI.skin.label.lineHeight;
                xStart += kXAxisWidth;
                width -= kXAxisWidth;
            }
            if (maxFrames > 0)
            {
                width = width * dataLength / maxFrames;
            }

            // Process input 
            Event e = Event.current;
            if (e.isMouse)
            {
                if (m_Dragging)
                {
                    if (e.type == EventType.MouseUp)
                    {
                        m_Dragging = false;
                        m_Moving = false;

                        // Delay the action as we are checking for double click
                        m_MouseReleased = true;
                        return State.Dragging;
                    }
                }

                int x = (int)(e.mousePosition.x - xStart);

                int dataOffset =  GetDataOffsetForXUnclamped(x, width, dataLength);
                if (m_Moving)
                    dataOffset -= m_MoveHandleOffset;
                dataOffset = ClampToRange(dataOffset, 0, dataLength - 1);
                int frameOffsetBeforeNext = Math.Max(dataOffset,  GetDataOffsetForX(x + 1, width, dataLength) - 1);

                if (m_Dragging)
                {
                    if (e.button == 0)
                    {
                        // Still dragging (doesn't have to be within the y bounds)
                        if (m_Moving)
                        {
                            // Forward drag from start point
                            SetDragMovement(dataOffset, frameOffsetBeforeNext, currentSelectionFirstDataOffset, currentSelectionLastDataOffset);
                        }
                        else
                        {
                            DragDirection dragDirection = (dataOffset < m_DragBeginFirstOffset) ? DragDirection.Backward : DragDirection.Forward;
                            SetDragSelection(dataOffset, frameOffsetBeforeNext, dragDirection);
                        }

                        CallSetRange(m_DragFirstOffset, m_DragLastOffset, m_ClickCount, m_SingleControlAction, FrameTimeGraph.State.Dragging);
                        return State.Dragging;
                    }
                }
                else
                {
                    if (e.mousePosition.x >= xStart && e.mousePosition.x <= (xStart + width) &&
                        e.mousePosition.y >= rect.y && e.mousePosition.y < rect.yMax)
                    {
                        s_LastSelectedGraphId = m_GraphId;

                        if (e.type == EventType.MouseDown && e.button == 0)
                        {
                            // Drag start (must be within the bounds of the control)
                            // Might be single or double click
                            m_LastClickTime = EditorApplication.timeSinceStartup;
                            m_ClickCount = e.clickCount;

                            m_Dragging = true;
                            m_Moving = false;

                            if (currentSelectionFirstDataOffset != 0 || currentSelectionLastDataOffset != dataLength - 1)
                            {
                                // Selection is valid
                                if (e.shift && dataOffset >= currentSelectionFirstDataOffset && frameOffsetBeforeNext <= currentSelectionLastDataOffset)
                                {
                                    // Moving if shift held and we are inside the current selection range
                                    m_Moving = true;
                                }
                            }

                            if (m_PairedWithFrameTimeGraph != null)
                                m_SingleControlAction = e.control || e.alt;  // Record if we are acting only on this control rather than the paired one too
                            else
                                m_SingleControlAction = true;

                            if (m_Moving)
                            {
                                m_MoveHandleOffset = dataOffset - currentSelectionFirstDataOffset;

                                SetDragMovement(currentSelectionFirstDataOffset, currentSelectionLastDataOffset, currentSelectionFirstDataOffset, currentSelectionLastDataOffset);
                            }
                            else
                            {
                                SetDragSelection(dataOffset, frameOffsetBeforeNext, DragDirection.Start);
                            }
                            CallSetRange(m_DragFirstOffset, m_DragLastOffset, m_ClickCount, m_SingleControlAction, FrameTimeGraph.State.Dragging);
                            return State.Dragging;
                        }
                    }
                }
            }

            if (m_MouseReleased)
            {
                // Not finished drag fully yet
                CallSetRange(m_DragFirstOffset, m_DragLastOffset, m_ClickCount, m_SingleControlAction, FrameTimeGraph.State.Dragging);
                return State.Dragging;
            }

            return State.None;
        }

        public float GetDataRange()
        {
            if (m_Values == null)
                return 0f;

            int frames = m_Values.Count;

            float min = 0f;
            float max = 0f;
            for (int frameOffset = 0; frameOffset < frames; frameOffset++)
            {
                float ms = m_Values[frameOffset].ms;
                if (ms > max)
                    max = ms;
            }
            float hRange = max - min;

            return hRange;
        }

        public void PairWith(FrameTimeGraph otherFrameTimeGraph)
        {
            if (m_PairedWithFrameTimeGraph != null)
            {
                // Clear existing pairing
                m_PairedWithFrameTimeGraph.m_PairedWithFrameTimeGraph = null;
            }

            m_PairedWithFrameTimeGraph = otherFrameTimeGraph;
            if (otherFrameTimeGraph!=null)
                otherFrameTimeGraph.m_PairedWithFrameTimeGraph = this;
        }

        public FrameTimeGraph GetPairedWith()
        {
            return m_PairedWithFrameTimeGraph;
        }

        public float GetYAxisRange(float yMax)
        {
            switch (s_YAxisMode)
            {
                case AxisMode.One60HzFrame:
                    return 1000f / 60f;
                case AxisMode.Two60HzFrames:
                    return 2000f / 60f;
                case AxisMode.Four60HzFrames:
                    return 4000f / 60f;
                case AxisMode.Max:
                    return yMax;
                case AxisMode.Custom:
                    return m_YAxisMs;
            }

            return yMax;
        }

        public void SetData(List<Data> values)
        {
            if (values==null)
                return;

            m_Values = values;

            if (m_GlobalSettings.showOrderedByFrameDuration)
                m_Values.Sort( (a, b) => { return a.ms.CompareTo(b.ms); } );
            else
                m_Values.Sort((a, b) => { return a.frameOffset.CompareTo(b.frameOffset); });

            m_FrameOffsetToDataOffsetMapping = new int[m_Values.Count];
            for (int dataOffset = 0; dataOffset < m_Values.Count; dataOffset++)
                m_FrameOffsetToDataOffsetMapping[ m_Values[dataOffset].frameOffset ] = dataOffset;

            m_CurrentSelection.Clear();
            for (int frameIndex = 0; frameIndex < m_Values.Count; frameIndex++)
            {
                m_CurrentSelection.Add(frameIndex);
            }
            m_CurrentSelectionFirstDataOffset = 0;
            m_CurrentSelectionLastDataOffset = m_Values.Count - 1;

            m_IsOrderedByFrameDuration = m_GlobalSettings.showOrderedByFrameDuration;
        }

        int GetDataOffset(int frameOffset)
        {
            if (frameOffset < 0 || frameOffset >= m_FrameOffsetToDataOffsetMapping.Length)
            {
                Debug.Log(string.Format("{0} out of range of frame offset to data offset mapping {1}", frameOffset, m_FrameOffsetToDataOffsetMapping.Length));
                return 0;
            }

           return m_FrameOffsetToDataOffsetMapping[frameOffset];
        }

        public bool HasData()
        {
            if (m_Values == null)
                return false;
            if (m_Values.Count == 0)
                return false;

            return true;
        }

        public void SetRangeCallback(SetRange setRange)
        {
            m_SetRange = setRange;
        }

        void CallSetRange(int startDataOffset, int endDataOffset, int clickCount, bool singleControlAction, FrameTimeGraph.State inputStatus, bool effectPaired = true)
        {
            if (m_SetRange == null)
                return;

            startDataOffset = Math.Max(0, startDataOffset);
            endDataOffset = Math.Min(endDataOffset, m_Values.Count-1);

            List<int> selected = new List<int>();
            for (int dataOffset = startDataOffset; dataOffset <= endDataOffset; dataOffset++)
            {
                if (dataOffset >= 0 && dataOffset < m_Values.Count)
                    selected.Add(m_Values[dataOffset].frameOffset);
            }
            // Sort selection in frame index order so start is lowest and end is highest
            selected.Sort();

            m_SetRange(selected, clickCount, singleControlAction, inputStatus);

            if (m_PairedWithFrameTimeGraph != null && m_PairedWithFrameTimeGraph.m_Values.Count>1 && effectPaired && !singleControlAction)
            {
                // Update selection on the other frame time graph
                int mainMaxFrame = m_Values.Count - 1;
                int otherMaxFrame = m_PairedWithFrameTimeGraph.m_Values.Count - 1;

                int startOffset = startDataOffset;
                int endOffset = endDataOffset;

                if (startOffset > otherMaxFrame)
                {
                    // Select all, if the main selection is outsize the range of the other
                    startOffset = 0;
                    endOffset = otherMaxFrame;
                }
                else
                {
                    if (startOffset == 0 && endOffset == mainMaxFrame)
                    {
                        // If clearing main selection then clear the other section fully too
                        endOffset = otherMaxFrame;
                    }

                    startOffset = ClampToRange(startOffset, 0, otherMaxFrame);
                    endOffset = ClampToRange(endOffset, 0, otherMaxFrame);
                }

                m_PairedWithFrameTimeGraph.CallSetRange(startOffset, endOffset, clickCount, singleControlAction, inputStatus, false);
            }
        }

        bool HasSubsetSelected()
        {
            if (m_Values == null)
                return false;

            int frames = m_Values.Count;

            bool subsetSelected = (m_CurrentSelectionFirstDataOffset != 0 || m_CurrentSelectionLastDataOffset != (frames - 1));

            return subsetSelected;
        }

        void RegenerateBars(float x, float y, float width, float height, float yRange)
        {
            int frames = m_Values.Count;
            if (frames <= 0)
                return;

            m_Bars.Clear();

            int nextDataOffset =  GetDataOffsetForX(0, (int)width, frames);
            for (int barX = 0; barX < width; barX++)
            {
                int startDataOffset = nextDataOffset;
                nextDataOffset =  GetDataOffsetForX(barX + 1, (int)width, frames);
                int endDataOffset = Math.Max(startDataOffset, nextDataOffset - 1);

                float min = m_Values[startDataOffset].ms;
                float max = min;
                for (int dataOffset = startDataOffset + 1; dataOffset <= endDataOffset; dataOffset++)
                {
                    float ms = m_Values[dataOffset].ms;
                    if (ms < min)
                        min = ms;
                    if (ms > max)
                        max = ms;
                }
                float maxClamped = Math.Min(max, yRange);
                float h = height * maxClamped / yRange;

                m_Bars.Add(new BarData(x + barX, y, 1, h, startDataOffset, endDataOffset, min, max));
            }
        }

        public void SetEnabled(bool enabled)
        {
            m_Enabled = enabled;
        }

        public bool IsEnabled()
        {
            return m_Enabled;
        }

        float GetTotalSelectionTime(List<int> selectedFrameOffsets)
        {
            float totalMs = 0;
            for (int i = 0; i < selectedFrameOffsets.Count; i++)
            {
                int frameOffset = selectedFrameOffsets[i];
                int dataOffset = m_FrameOffsetToDataOffsetMapping[frameOffset];
                totalMs += m_Values[dataOffset].ms;
            }

            return totalMs;
        }

        void ShowFrameLines(float x, float y, float yRange, float width, float height)
        {
            float msSegment = 1000f / 60f;
            int lines = (int)(yRange / msSegment);
            int step = 1;
            for (int line = 1; line <= lines; line += step, step *= 2)
            {
                float ms = line * msSegment;
                float h = height * ms / yRange;
                m_2D.DrawLine(x, y + h, x + width - 1, y + h, m_ColorGridLine);
            }
        }

        public void Draw(Rect rect, ProfileAnalysis analysis, List<int> selectedFrameOffsets, float yMax, int displayOffset, string selectedMarkerName, int maxFrames = 0, ProfileAnalysis fullAnalysis = null)
        {
            if (m_Values == null)
                return;

            int totalDataSize = m_Values.Count;
            if (totalDataSize <= 0)
                return;

            if (m_IsOrderedByFrameDuration != m_GlobalSettings.showOrderedByFrameDuration)
            {
                // Reorder if necessary
                SetData(m_Values);
            }

            // Get start and end selection span
            int currentSelectionFirstDataOffset;
            int currentSelectionLastDataOffset;
            int currentSelectionFirstFrameOffset;
            int currentSelectionLastFrameOffset;

            GetSelectedRange(selectedFrameOffsets, out currentSelectionFirstDataOffset, out currentSelectionLastDataOffset, out currentSelectionFirstFrameOffset, out currentSelectionLastFrameOffset);


            // Create mapping from offset to selection for faster selection detection
            Dictionary<int,int> frameOffsetToSelectionIndex = new Dictionary<int,int>();
            for (int i = 0; i < selectedFrameOffsets.Count; i++)
            {
                int frameOffset = selectedFrameOffsets[i];
                frameOffsetToSelectionIndex[frameOffset] = i;
            }

            Event current = Event.current;

            int selectedFirstOffset;
            int selectedLastOffset;
            int selectedCount;
            bool subsetSelected = false;
            if (HasDragRegion())
            {
                selectedFirstOffset = m_DragFirstOffset;
                selectedLastOffset = m_DragLastOffset;
                selectedCount = 1 + (m_DragLastOffset - m_DragFirstOffset);
                subsetSelected = true;
            }
            else
            {
                selectedFirstOffset = currentSelectionFirstDataOffset;
                selectedLastOffset = currentSelectionLastDataOffset;
                selectedCount = selectedFrameOffsets.Count;
                subsetSelected = (selectedCount > 0) && (selectedCount != totalDataSize);
            }

            // Draw frames and selection
            float width = rect.width;
            float height = rect.height;

            bool showAxis = false;
            float xStart = 0f;
            float yStart = 0f;
            if (height > kYAxisDetailThreshold)
            {
                showAxis = true;

                float h = GUI.skin.label.lineHeight;
                xStart += kXAxisWidth;
                width -= kXAxisWidth;

                yStart += h;
                height -= h;
            }
            if (maxFrames > 0)
            {
                width = width * totalDataSize / maxFrames;
            }

            // Start / End
            int startOffset = m_Zoomed ? m_ZoomStartOffset : 0;
            int endOffset = m_Zoomed ? m_ZoomEndOffset : totalDataSize - 1;

            // Get try index values
            int startIndex = displayOffset + startOffset;
            int endIndex = displayOffset + endOffset;
            int selectedFirstIndex = displayOffset + selectedFirstOffset;
            int selectedLastIndex = displayOffset + selectedLastOffset;

            string detailsString = "";

            if (!showAxis)
            {
                string frameRangeString;
                if (startIndex == endIndex)
                    frameRangeString = string.Format("Total Range {0}", startIndex);
                else
                    frameRangeString = string.Format("Total Range {0} - {1} [{2}]", startIndex, endIndex, 1 + (endIndex - startIndex));

                // Selection range
                string selectedTooltip = "";
                if (subsetSelected)
                {
                    if (selectedFirstIndex == selectedLastIndex)
                        selectedTooltip = string.Format("\nSelected {0}\n", selectedFirstIndex);
                    else
                        selectedTooltip = string.Format("\nSelected {0} - {1} [{2}]", selectedFirstIndex, selectedLastIndex, selectedCount);
                }

                detailsString = string.Format("\n\n{0}{1}", frameRangeString, selectedTooltip);
            }

            float yRange = GetYAxisRange(yMax);

            bool lastEnabled = GUI.enabled;
            bool enabled = IsEnabled();
            GUI.enabled = enabled;

            if (m_2D.DrawStart(rect, Draw2D.Origin.BottomLeft))
            {
                float x = xStart;
                float y = yStart;

                m_2D.DrawFilledBox(xStart, yStart, width, height, m_ColorBarBackground);

                RegenerateBars(xStart, yStart, width, height, yRange);

                foreach (BarData bar in m_Bars)
                {
                    bool containsSelection = false;
                    for (int dataOffset = bar.startDataOffset; dataOffset <= bar.endDataOffset; dataOffset++)
                    {
                        int frameOffset = m_Values[dataOffset].frameOffset;
                        if (frameOffsetToSelectionIndex.ContainsKey(frameOffset))
                        {
                            containsSelection = true;
                            break;
                        }
                    }

                    if (HasDragRegion() && bar.endDataOffset >= selectedFirstOffset && bar.startDataOffset <= selectedLastOffset)
                    {
                        m_2D.DrawFilledBox(bar.x, bar.y, bar.w, height, m_ColorBarBackgroundSelected);
                    }
                    else if (!HasDragRegion() && subsetSelected && containsSelection)
                    {
                        m_2D.DrawFilledBox(bar.x, bar.y, bar.w, height, m_ColorBarBackgroundSelected);
                    }
                }

                if (m_GlobalSettings.showFrameLines)
                {
                    ShowFrameLines(xStart, yStart, yRange, width, height);
                }

                ProfileAnalysis analysisData = analysis;
                bool full = false;
                if (fullAnalysis != null)
                {
                    analysisData = fullAnalysis;
                    full = true;
                }

                float totalMs = GetTotalSelectionTime(selectedFrameOffsets);
                string selectionAreaString = string.Format("\n\nTotal time for {0} selected frames {1}",selectedCount,ToDisplayUnits(totalMs, true));

                MarkerData selectedMarker = (m_GlobalSettings.showSelectedMarker && analysisData!=null) ? analysisData.GetMarkerByName(selectedMarkerName) : null;
                foreach (BarData bar in m_Bars)
                {
                    bool containsSelection = false;
                    for (int dataOffset = bar.startDataOffset; dataOffset <= bar.endDataOffset; dataOffset++)
                    {
                        int frameOffset = m_Values[dataOffset].frameOffset;
                        if (frameOffsetToSelectionIndex.ContainsKey(frameOffset))
                        {
                            containsSelection = true;
                            break;
                        }
                    }

                    bool inSelectionRegion = false;
                    if (HasDragRegion() && bar.endDataOffset >= selectedFirstOffset && bar.startDataOffset <= selectedLastOffset)
                    {
                        inSelectionRegion = true;
                    }
                    else if (!HasDragRegion() && subsetSelected && containsSelection)
                    {
                        inSelectionRegion = true;
                    }
                    
                    if (inSelectionRegion)
                    {
                        m_2D.DrawFilledBox(bar.x, bar.y, bar.w, bar.h, m_ColorBarSelected);
                    }
                    else
                    {
                        m_2D.DrawFilledBox(bar.x, bar.y, bar.w, bar.h, m_ColorBar);
                    }

                    // Show where its been clamped
                    if (bar.yMax > yRange)
                    {
                        m_2D.DrawFilledBox(bar.x, bar.y + height, 1, kOverrunHeight, m_ColorBarOutOfRange);
                    }


                    if (analysisData != null && (full || !m_Dragging))
                    {
                        // Analysis is just on the subset
                        if (m_GlobalSettings.showThreads)
                        {
                            ShowThreads(height, yRange, bar, full,
                                analysisData.GetThreads(), subsetSelected, selectedFirstOffset, selectedLastOffset,
                                displayOffset, frameOffsetToSelectionIndex);
                        }

                        if (m_GlobalSettings.showSelectedMarker)
                        {
                            // Analysis is just on the subset (unless we have full analysis data)
                            ShowSelectedMarker(height, yRange, bar, full, selectedMarker, subsetSelected, selectedFirstOffset, selectedLastOffset,
                                displayOffset, frameOffsetToSelectionIndex);
                        }
                    }

                    // Draw tooltip for bar (or 1 pixel segment of bar)
                    {
                        int barStartIndex = displayOffset + m_Values[bar.startDataOffset].frameOffset;
                        int barEndIndex = displayOffset + m_Values[bar.endDataOffset].frameOffset;
                        string tooltip;
                        if (barStartIndex == barEndIndex)
                            tooltip = string.Format("Frame {0}\n{1}{2}", barStartIndex, ToDisplayUnits(bar.yMax, true), detailsString);
                        else
                            tooltip = string.Format("Frame {0}-{1}\n{2} max\n{3} min{4}", barStartIndex, barEndIndex, ToDisplayUnits(bar.yMax, true), ToDisplayUnits(bar.yMin, true), detailsString);

                        if (inSelectionRegion)
                            tooltip += selectionAreaString;
                        GUI.Label(new Rect(rect.x + bar.x, rect.y + 5, bar.w, height), new GUIContent("", tooltip));
                    }
                }

                m_2D.DrawEnd();

                if (m_GlobalSettings.showFrameLines && m_GlobalSettings.showFrameLineText)
                {
                    ShowFrameLinesText(rect, xStart, yStart, yRange, width, height, subsetSelected, selectedFirstOffset, selectedLastOffset);
                }
            }
            GUI.enabled = lastEnabled;

            if (showAxis)
            {
                ShowAxis(rect, xStart, width, startOffset, endOffset, selectedFirstOffset, selectedLastOffset, selectedCount, yMax, totalDataSize, displayOffset); 
            }

            GUI.enabled = enabled;
            if (rect.Contains(current.mousePosition) && current.type == EventType.ContextClick)
            {
                var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                
                ShowContextMenu(subsetSelected);

                current.Use();

                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.FrameTimeContextMenu, analytic.GetDurationInSeconds(), true);
            }
            GUI.enabled = lastEnabled;
        }

        void ShowThreads(float height, float yRange, BarData bar, bool full, 
            List<ThreadData> threads, bool subsetSelected, int selectedFirstOffset, int selectedLastOffset, 
            int displayOffset,  Dictionary<int,int> frameOffsetToSelectionIndex)
        {
            float max = float.MinValue;
            bool selected = false;
            for (int dataOffset = bar.startDataOffset; dataOffset <= bar.endDataOffset; dataOffset++)
            {
                int frameOffset = m_Values[dataOffset].frameOffset;
                if (!full && !frameOffsetToSelectionIndex.ContainsKey(frameOffset))
                    continue;

                float threadMs = 0f;
                foreach (var thread in threads)
                {
                    int frameIndex = displayOffset + frameOffset;
                    var frame = thread.GetFrame(frameIndex);
                    if (frame==null)
                        continue;

                    float ms = frame.ms;
                    if (ms > threadMs)
                        threadMs = ms;
                }

                if (threadMs > max)
                    max = threadMs;

                if (m_Dragging)
                {
                    if (frameOffset >= selectedFirstOffset && frameOffset <= selectedLastOffset)
                        selected = true;
                }
                else if(subsetSelected)
                {
                    if (frameOffsetToSelectionIndex.ContainsKey(frameOffset))
                        selected = true;
                }
            }

            if (full || selected)
            {
                // Clamp to frame time (these values can be tiem summed over multiple threads)
                if (max > bar.yMax)
                    max = bar.yMax;

                float maxClamped = Math.Min(max, yRange);
                float h = height * maxClamped / yRange;

                m_2D.DrawFilledBox(bar.x, bar.y, bar.w, h, selected ? m_ColorBarThreadsSelected : m_ColorBarThreads);

                // Show where its been clamped
                if (max > yRange)
                {
                    m_2D.DrawFilledBox(bar.x, bar.y + height, bar.w, kOverrunHeight, m_ColorBarThreadsOutOfRange);
                }
            }
        }

        void ShowSelectedMarker(float height, float yRange, BarData bar, bool full, 
            MarkerData selectedMarker, bool subsetSelected, int selectedFirstOffset, int selectedLastOffset, 
            int displayOffset,  Dictionary<int,int> frameOffsetToSelectionIndex)
        {
            float max = 0f;
            bool selected = false;
            if (selectedMarker != null)
            {
                for (int dataOffset = bar.startDataOffset; dataOffset <= bar.endDataOffset; dataOffset++)
                {
                    int frameOffset = m_Values[dataOffset].frameOffset;
                    if (!full && !frameOffsetToSelectionIndex.ContainsKey(frameOffset))
                        continue;

                    float ms = selectedMarker.GetFrameMs(displayOffset + frameOffset);

                    if (ms > max)
                        max = ms;

                    if (m_Dragging)
                    {
                        if (frameOffset >= selectedFirstOffset && frameOffset <= selectedLastOffset)
                            selected = true;
                    }
                    else if (subsetSelected)
                    {
                        if (frameOffsetToSelectionIndex.ContainsKey(frameOffset))
                            selected = true;
                    }
                }
            }

            if (full || selected)
            {
                // Clamp to frame time (these values can be tiem summed over multiple threads)
                if (max > bar.yMax)
                    max = bar.yMax;

                float maxClamped = Math.Min(max, yRange);
                float h = height * maxClamped / yRange;

                m_2D.DrawFilledBox(bar.x, bar.y, bar.w, h, selected ? m_ColorBarMarkerSelected : m_ColorBarMarker);

                if (max > 0f)
                {
                    // we start the bar lower so that very small markers still show up.
                    m_2D.DrawFilledBox(bar.x, bar.y - kOverrunHeight, bar.w, kOverrunHeight, m_ColorBarMarkerOutOfRange);
                }

                // Show where its been clamped
                if (max > yRange)
                {
                    m_2D.DrawFilledBox(bar.x, bar.y + height, bar.w, kOverrunHeight, m_ColorBarMarkerOutOfRange);
                }
            }
        }

        void ShowFrameLinesText(Rect rect, float xStart, float yStart, float yRange, float width, float height, bool subsetSelected, int selectedFirstOffset, int selectedLastOffset)
        {
            int totalDataSize = m_Values.Count;
            float y = yStart;
            
            float msSegment = 1000f / 60f;

            int lines = (int)(yRange / msSegment);
            int step = 1;
            for (int line = 1; line <= lines; line += step, step *= 2)
            {
                float ms = line * msSegment;
                float h = height * ms / yRange;
                int edgePad = 3;
                if (h >= (height / 4) && h < (height - GUI.skin.label.lineHeight))
                {
                    GUIContent content = new GUIContent(ToDisplayUnits((float)Math.Floor(ms),true));
                    Vector2 size = EditorStyles.miniTextField.CalcSize(content);

                    bool left = true;
                    if (subsetSelected)
                    {
                        float x = GetXForDataOffset(selectedFirstOffset, (int)width, totalDataSize);
                        float x2 = GetXForDataOffset(selectedLastOffset + 1, (int)width, totalDataSize);

                        // text would overlap selection so move it if that prevents overlap
                        if (left)
                        {
                            if (x < (size.x + edgePad) && x2 < (width - (size.x + edgePad)))
                                left = false;
                        }
                        else
                        {
                            if (x > (size.x + edgePad) && x2 > (width - (size.x + edgePad)))
                                left = true;
                        }
                    }

                    Rect r;

                    if (left)
                        r = new Rect(rect.x + (xStart + edgePad), (rect.y - y) + (height - h), size.x, EditorStyles.miniTextField.lineHeight);
                    else
                        r = new Rect(rect.x + (xStart + width) - (size.x + edgePad), (rect.y - y) + (height - h), size.x, EditorStyles.miniTextField.lineHeight);
                    GUI.Label(r, content, EditorStyles.miniTextField);
                }
            }
        }

        void ShowContextMenu(bool subsetSelected)
        {
            GenericMenu menu = new GenericMenu();
            bool showselectionOptions = subsetSelected || ((m_PairedWithFrameTimeGraph != null) && m_PairedWithFrameTimeGraph.HasSubsetSelected());
            if (showselectionOptions)
            {
                menu.AddItem(Styles.menuItemClearSelection, false, () => ClearSelection());
                if (subsetSelected)
                    menu.AddItem(Styles.menuItemInvertSelection, false, () => InvertSelection());
                else
                    menu.AddDisabledItem(Styles.menuItemInvertSelection);
            }
            else
            {
                menu.AddDisabledItem(Styles.menuItemClearSelection);
                menu.AddDisabledItem(Styles.menuItemInvertSelection);
            }
            menu.AddItem(Styles.menuItemSelectMin, false, () => SelectMin());
            menu.AddItem(Styles.menuItemSelectMax, false, () => SelectMax());
            menu.AddItem(Styles.menuItemSelectMedian, false, () => SelectMedian());
            menu.AddSeparator("");
            //menu.AddItem(Styles.menuItemSelectPrevious, false, () => SelectPrevious(1));
            //menu.AddItem(Styles.menuItemSelectNext, false, () => SelectNext(1));
            menu.AddItem(Styles.menuItemSelectGrow, false, () => SelectGrow(1));
            menu.AddItem(Styles.menuItemSelectShrink, false, () => SelectShrink(1));
            menu.AddItem(Styles.menuItemSelectGrowLeft, false, () => SelectGrowLeft(1));
            menu.AddItem(Styles.menuItemSelectGrowRight, false, () => SelectGrowRight(1));
            //menu.AddItem(Styles.menuItemSelectShrinkLeft, false, () => SelectShrinkLeft(1));
            //menu.AddItem(Styles.menuItemSelectShrinkRight, false, () => SelectShrinkRight(1));
            menu.AddSeparator("");
            if (showselectionOptions)
            {
                menu.AddItem(Styles.menuItemZoomSelection, false, () => ZoomSelection());
                menu.AddItem(Styles.menuItemZoomAll, false, () => ZoomAll());
            }
            else
            {
                menu.AddDisabledItem(Styles.menuItemZoomSelection);
                menu.AddDisabledItem(Styles.menuItemZoomAll);
            }
            menu.AddSeparator("");
            menu.AddItem(Styles.menuItemShowSelectedMarker, m_GlobalSettings.showSelectedMarker, () => ToggleShowSelectedMarker());
            menu.AddItem(Styles.menuItemShowThreads, m_GlobalSettings.showThreads, () => ToggleShowThreads());
            menu.AddItem(Styles.menuItemShowFrameLines, m_GlobalSettings.showFrameLines, () => ToggleShowFrameLines());
            //menu.AddItem(Styles.menuItemShowFrameLineText, m_GlobalSettings.showFrameLineText, () => ToggleShowFrameLinesText());
            menu.AddSeparator("");
            menu.AddItem(Styles.menuItemShowOrderedByFrameDuration, m_GlobalSettings.showOrderedByFrameDuration, () => ToggleShowOrderedByFrameDuration());
            
            menu.ShowAsContext();
        }

        string GetYMaxText(float value)
        {
            if (value < 10)
               return ToDisplayUnits(value, true);

            // TODO: 1 dp for milliseconds ?
            return ToDisplayUnits(value, true);
        }

        void DrawYAxisRangeSelector(Rect rect, float yMax)
        {
            string yMaxText = GetYMaxText(yMax);

            List<GUIContent> yAxisOptions = new List<GUIContent>();
            yAxisOptions.Add(new GUIContent(ToDisplayUnits(1000f/60f,true), "Graph Scale : 1 60 Hz frame"));
            yAxisOptions.Add(new GUIContent(ToDisplayUnits(1000f/30f,true), "Graph Scale : 2 60 Hz frames"));
            yAxisOptions.Add(new GUIContent(ToDisplayUnits(1000f/15f,true), "Graph Scale : 4 60 Hz frames)"));
            yAxisOptions.Add(new GUIContent(yMaxText, "Graph Scale : Max frame time from data"));
            s_YAxisMode = (AxisMode)EditorGUI.Popup(rect, (int)s_YAxisMode, yAxisOptions.ToArray());
        }

        void ShowAxis(Rect rect, float xStart, float width, int startOffset, int endOffset, int selectedFirstOffset, int selectedLastOffset, int selectedCount, float yMax, int totalDataSize, int displayOffset)
        {
            GUIStyle leftAlignStyle = new GUIStyle(GUI.skin.label);
            leftAlignStyle.padding = new RectOffset(leftAlignStyle.padding.left, leftAlignStyle.padding.right, 0, 0);
            leftAlignStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle rightAlignStyle = new GUIStyle(GUI.skin.label);
            rightAlignStyle.padding = new RectOffset(rightAlignStyle.padding.left, rightAlignStyle.padding.right, 0, 0);
            rightAlignStyle.alignment = TextAnchor.MiddleRight;

            // y axis
            float h = GUI.skin.label.lineHeight;
            float y = rect.y + ((rect.height - 1) - h);


            DrawYAxisRangeSelector(new Rect(rect.x, rect.y, kXAxisWidth, h), yMax);

            string yMinText = ToDisplayUnits(0,true);
            GUI.Label(new Rect(rect.x, y - h, kXAxisWidth, h), yMinText, rightAlignStyle);


            // x axis
            rect.x += xStart;

            leftAlignStyle.padding = new RectOffset(0, 0, leftAlignStyle.padding.top, leftAlignStyle.padding.bottom);
            rightAlignStyle.padding = new RectOffset(0, 0, rightAlignStyle.padding.top, rightAlignStyle.padding.bottom);

            int startIndex = displayOffset + startOffset;
            string startIndexText = string.Format("{0}", startIndex);
            GUIContent startIndexContent = new GUIContent(startIndexText);
            Vector2 startIndexSize = GUI.skin.label.CalcSize(startIndexContent);
            bool drawStart = !m_GlobalSettings.showOrderedByFrameDuration;

            int endIndex = displayOffset + endOffset;
            string endIndexText = string.Format("{0}", endIndex);
            GUIContent endIndexContent = new GUIContent(endIndexText);
            Vector2 endIndexSize = GUI.skin.label.CalcSize(endIndexContent);
            bool drawEnd = !m_GlobalSettings.showOrderedByFrameDuration;



            // Show selection frame values (if space for them)
            int selectedFirstX = GetXForDataOffset(selectedFirstOffset, (int)width, totalDataSize);
            int selectedLastX = GetXForDataOffset(selectedLastOffset+1, (int)width, totalDataSize);   // last + 1 so right hand side of the bbar
            int selectedRangeWidth = 1 + (selectedLastX - selectedFirstX);

            int selectedFirstIndex = displayOffset + selectedFirstOffset;
            int selectedLastIndex = displayOffset + selectedLastOffset;
            string selectionRangeText;
            if (selectedCount > 1)
            {
                if (m_GlobalSettings.showOrderedByFrameDuration)
                    selectionRangeText = string.Format("[{0}]", selectedCount);
                else
                    selectionRangeText = string.Format("{0} [{1}] {2}", selectedFirstIndex, selectedCount, selectedLastIndex);
            }
            else
                selectionRangeText = string.Format("{0} [{1}]", selectedFirstIndex, selectedCount);
            GUIContent selectionRangeTextContent = new GUIContent(selectionRangeText, string.Format("{0} frames in selection", selectedCount));
            Vector2 selectionRangeTextSize = GUI.skin.label.CalcSize(selectionRangeTextContent);
            if (selectedRangeWidth > selectionRangeTextSize.x)
            {
                // Selection width is larger than the text so we can split the text 
                string selectedFirstIndexText = string.Format("{0}", selectedFirstIndex);
                GUIContent selectedFirstIndexContent = new GUIContent(selectedFirstIndexText);
                Vector2 selectedFirstIndexSize = GUI.skin.label.CalcSize(selectedFirstIndexContent);
                if (m_GlobalSettings.showOrderedByFrameDuration)
                    selectedFirstIndexSize.x = 0;

                string selectedLastIndexText = string.Format("{0}", selectedLastIndex);
                GUIContent selectedLastIndexContent = new GUIContent(selectedLastIndexText);
                Vector2 selectedLastIndexSize = GUI.skin.label.CalcSize(selectedLastIndexContent);
                if (m_GlobalSettings.showOrderedByFrameDuration)
                    selectedLastIndexSize.x = 0;

                string selectedCountText = string.Format("[{0}]", selectedCount);
                GUIContent selectedCountContent = new GUIContent(selectedCountText, string.Format("{0} frames in selection", selectedCount));
                Vector2 selectedCountSize = GUI.skin.label.CalcSize(selectedCountContent);

                Rect rFirst = new Rect(rect.x + selectedFirstX, y, selectedFirstIndexSize.x, selectedFirstIndexSize.y);
                GUI.Label(rFirst, selectedFirstIndexContent); 

                Rect rLast = new Rect(rect.x + selectedLastX - selectedLastIndexSize.x, y, selectedLastIndexSize.x, selectedLastIndexSize.y);
                GUI.Label(rLast, selectedLastIndexContent);

                float mid = selectedFirstX + ((selectedLastX - selectedFirstX) / 2);
                Rect rCount = new Rect(rect.x + mid - (selectedCountSize.x / 2), y, selectedCountSize.x, selectedCountSize.y);
                GUI.Label(rCount, selectedCountContent);

                if (selectedFirstX < startIndexSize.x)
                {
                    // would overlap with start text
                    drawStart = false;
                }
                if (selectedLastX > ((width - 1) - endIndexSize.x))
                {
                    // would overlap with end text
                    drawEnd = false;
                }
            }
            else
            {
                int mid = (selectedFirstX + (selectedRangeWidth / 2));
                int selectionTextX = mid - (int)(selectionRangeTextSize.x / 2);
                selectionTextX = ClampToRange(selectionTextX, 0, (int)((width - 1) - selectionRangeTextSize.x));

                Rect rangeRect = new Rect(rect.x + selectionTextX, y, selectionRangeTextSize.x, selectionRangeTextSize.y);
                GUI.Label(rangeRect, selectionRangeTextContent);

                if (selectionTextX < startIndexSize.x)
                {
                    // would overlap with start text
                    drawStart = false;
                }
                if ((selectionTextX + selectionRangeTextSize.x) > ((width - 1) - endIndexSize.x))
                {
                    // would overlap with end text
                    drawEnd = false;
                }
            }


            // Show start and end values
            if (drawStart)
            {
                Rect leftRect = new Rect(rect.x, y, startIndexSize.x, startIndexSize.y);
                GUI.Label(leftRect, startIndexContent, leftAlignStyle);
            }

            if (drawEnd)
            {
                Rect rightRect = new Rect(rect.x + ((width - 1) - endIndexSize.x), y, endIndexSize.x, endIndexSize.y);
                GUI.Label(rightRect, endIndexContent, rightAlignStyle);
            }
        }

        void ClearSelection(bool effectPaired = true)
        {
            int dataLength = m_Values.Count;

            bool singleControlAction = true;    // As we need the frame range to be unique to each data set
            CallSetRange(0, dataLength - 1, 0, singleControlAction, FrameTimeGraph.State.DragComplete);

            // Disable zoom 
            m_Zoomed = false;

            if (m_PairedWithFrameTimeGraph != null && effectPaired)
                m_PairedWithFrameTimeGraph.ClearSelection(false);
        }

        void InvertSelection(bool effectPaired = true)
        {
            int dataLength = m_Values.Count;

            Dictionary<int, int> frameOffsetToSelectionIndex = new Dictionary<int, int>();
            for (int i = 0; i < m_CurrentSelection.Count; i++)
            {
                int frameOffset = m_CurrentSelection[i];
                frameOffsetToSelectionIndex[frameOffset] = i;
            }

            m_CurrentSelection.Clear();
            for (int frameIndex = 0; frameIndex < dataLength; frameIndex++)
            {
                if (!frameOffsetToSelectionIndex.ContainsKey(frameIndex))
                    m_CurrentSelection.Add(frameIndex);
            }

            bool singleControlAction = true;    // As we need the frame range to be unique to each data set
            m_SetRange(m_CurrentSelection, 1, singleControlAction, FrameTimeGraph.State.DragComplete);

            // Disable zoom 
            m_Zoomed = false;

            if (m_PairedWithFrameTimeGraph != null && effectPaired)
                m_PairedWithFrameTimeGraph.InvertSelection(false);
        }

        void ZoomSelection(bool effectPaired = true)
        {
            m_Zoomed = true;
            m_ZoomStartOffset = m_CurrentSelectionFirstDataOffset;
            m_ZoomEndOffset = m_CurrentSelectionLastDataOffset;

            if (m_PairedWithFrameTimeGraph != null && effectPaired)
                m_PairedWithFrameTimeGraph.ZoomSelection(false);
        }

        void ZoomAll(bool effectPaired = true)
        {
            m_Zoomed = false;
            int frames = m_Values.Count;

            m_ZoomStartOffset = 0;
            m_ZoomEndOffset = frames - 1;


            if (m_PairedWithFrameTimeGraph != null && effectPaired)
                m_PairedWithFrameTimeGraph.ZoomAll(false);
        }

        void SelectMin(bool effectPaired = true)
        {
            int dataLength = m_Values.Count;
            if (dataLength <= 0)
                return;


            int minDataOffset = 0;
            float msMin = m_Values[0].ms;
            for (int dataOffset = 0; dataOffset < dataLength; dataOffset++)
            {
                float ms = m_Values[dataOffset].ms;
                if (ms < msMin)
                {
                    msMin = ms;
                    minDataOffset = dataOffset;
                }
            }

            bool singleControlAction = true;
            CallSetRange(minDataOffset, minDataOffset, 1, singleControlAction, State.DragComplete); // act like single click, so we select frame
            //CallSetRange(minDataOffset, minDataOffset, 2, singleControlAction, State.DragComplete); // act like double click, so we jump to the frame
            m_Zoomed = false;


            if (m_PairedWithFrameTimeGraph != null && effectPaired)
                m_PairedWithFrameTimeGraph.SelectMin(false);
        }

        void SelectMax(bool effectPaired = true)
        {
            int dataLength = m_Values.Count;
            if (dataLength <= 0)
                return;


            int maxDataOffset = 0;
            float msMax = m_Values[0].ms;
            for (int dataOffset = 0; dataOffset < dataLength; dataOffset++)
            {
                float ms = m_Values[dataOffset].ms;
                if (ms > msMax)
                {
                    msMax = ms;
                    maxDataOffset = dataOffset;
                }
            }

            bool singleControlAction = true;
            CallSetRange(maxDataOffset, maxDataOffset, 1, singleControlAction, State.DragComplete); // act like single click, so we select frame
            //CallSetRange(maxDataOffset, maxDataOffset, 2, singleControlAction, State.DragComplete); // act like double click, so we jump to the frame
            m_Zoomed = false;


            if (m_PairedWithFrameTimeGraph != null && effectPaired)
                m_PairedWithFrameTimeGraph.SelectMax(false);
        }

        private float GetPercentageOffset(List<Data> data, float percent, out int outputFrameOffset)
        {
            int index = (int)((data.Count - 1) * percent / 100);
            outputFrameOffset = data[index].frameOffset;

            // True median is half of the sum of the middle 2 frames for an even count. However this would be a value never recorded so we avoid that.
            return data[index].ms;
        }

        void SelectMedian(bool effectPaired = true)
        {
            int dataLength = m_Values.Count;
            if (dataLength <= 0)
                return;

            List<Data> sortedValues = new List<Data>();
            foreach (var value in m_Values)
            {
                Data data = new Data(value.ms, value.frameOffset);
                sortedValues.Add(data);
            }
            // If ms value is the same then order by frame offset
            sortedValues.Sort((a, b) => { int compare = a.ms.CompareTo(b.ms); return compare==0 ? a.frameOffset.CompareTo(b.frameOffset) : compare; });
            int medianFrameOffset = 0;
            GetPercentageOffset(sortedValues, 50, out medianFrameOffset);
            int medianDataOffset = m_FrameOffsetToDataOffsetMapping[medianFrameOffset];


            bool singleControlAction = true;
            CallSetRange(medianDataOffset, medianDataOffset, 1, singleControlAction, State.DragComplete); // act like single click, so we select frame
            //CallSetRange(medianDataOffset, medianDataOffset, 2, singleControlAction, State.DragComplete); // act like double click, so we jump to the frame
            m_Zoomed = false;


            if (m_PairedWithFrameTimeGraph != null && effectPaired)
                m_PairedWithFrameTimeGraph.SelectMedian(false);
        }

        public void SelectPrevious(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;

            if (step > m_CurrentSelectionFirstDataOffset)
            {
                // clamp end of range
                step = m_CurrentSelectionFirstDataOffset;
            }

            CallSetRange(m_CurrentSelectionFirstDataOffset - step, m_CurrentSelectionLastDataOffset - step, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
        }

        public void SelectNext(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;
            int dataLength = m_Values.Count;

            if (m_CurrentSelectionLastDataOffset + step >= (dataLength - 1))
            {
                // clamp end of range
                step = (dataLength - 1) - m_CurrentSelectionLastDataOffset;
            }

            CallSetRange(m_CurrentSelectionFirstDataOffset + step, m_CurrentSelectionLastDataOffset + step, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
        }


        public void SelectGrow(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;
            int dataLength = m_Values.Count;

            // Auto clamps
            CallSetRange(m_CurrentSelectionFirstDataOffset - step, m_CurrentSelectionLastDataOffset + step, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
        }

        public void SelectGrowLeft(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;

            // Auto clamps
            CallSetRange(m_CurrentSelectionFirstDataOffset - step, m_CurrentSelectionLastDataOffset, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
        }

        public void SelectGrowRight(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;
            int dataLength = m_Values.Count;

            // Auto clamps
            CallSetRange(m_CurrentSelectionFirstDataOffset, m_CurrentSelectionLastDataOffset + step, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
        }

        public void SelectShrink(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;

            if ((m_CurrentSelectionLastDataOffset - m_CurrentSelectionFirstDataOffset) >= (2 * step))
            {
                // shrink both sides
                CallSetRange(m_CurrentSelectionFirstDataOffset + step, m_CurrentSelectionLastDataOffset - step, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
            }
            else
            {
                // Choose mid point
                int mid = m_CurrentSelectionFirstDataOffset + (m_CurrentSelectionLastDataOffset - m_CurrentSelectionFirstDataOffset) / 2;
                CallSetRange(mid, mid, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
            }
        }

        public void SelectShrinkLeft(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;

            if ((m_CurrentSelectionLastDataOffset - m_CurrentSelectionFirstDataOffset) >= step)
            {
                CallSetRange(m_CurrentSelectionFirstDataOffset + step, m_CurrentSelectionLastDataOffset, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
            }
            else
            {
                // Just right remains
                CallSetRange(m_CurrentSelectionLastDataOffset, m_CurrentSelectionLastDataOffset, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
            }
        }

        public void SelectShrinkRight(int step)
        {
            int clicks = 1;
            bool singleClickAction = false;

            if ((m_CurrentSelectionLastDataOffset - m_CurrentSelectionFirstDataOffset) >= step)
            {
                CallSetRange(m_CurrentSelectionFirstDataOffset, m_CurrentSelectionLastDataOffset - step, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
            }
            else
            {
                // Just left remains
                CallSetRange(m_CurrentSelectionFirstDataOffset, m_CurrentSelectionFirstDataOffset, clicks, singleClickAction, FrameTimeGraph.State.DragComplete);
            }
        }

        public void ToggleShowThreads()
        {
            m_GlobalSettings.showThreads ^= true;
        }

        public void ToggleShowSelectedMarker()
        {
            m_GlobalSettings.showSelectedMarker ^= true;
        }

        public void ToggleShowFrameLines()
        {
            m_GlobalSettings.showFrameLines ^= true;
        }

        public void ToggleShowFrameLinesText()
        {
            m_GlobalSettings.showFrameLineText ^= true;
        }

        public void ToggleShowOrderedByFrameDuration()
        {
            m_GlobalSettings.showOrderedByFrameDuration ^= true;
            SetData(m_Values);  // Update order

            if (m_PairedWithFrameTimeGraph != null)
            {
                m_PairedWithFrameTimeGraph.SetData(m_PairedWithFrameTimeGraph.m_Values);  // Update order
            }
        }
    }
}