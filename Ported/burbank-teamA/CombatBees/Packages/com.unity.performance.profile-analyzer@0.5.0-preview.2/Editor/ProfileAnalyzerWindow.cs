using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using System.Reflection;
using UnityEngine.Profiling;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    enum ThreadRange
    {
        Median,
        UpperQuartile,
        Max
    };

    enum ActiveTab
    {
        Summary,
        Compare,
    };

    enum ThreadActivity
    {
        None,
        Analyze,
        AnalyzeDone,
        Compare,
        CompareDone,
    };

    public enum TopTenDisplay
    {
        Normalised,
        LongestTime,
    };

    public enum NameFilterOperation
    {
        All,    // AND
        Any,    // OR
    };

    public class ProfileAnalyzerWindow : EditorWindow
    {
        internal static class Styles
        {
            public static readonly GUIContent emptyString = new GUIContent("", "");
            public static readonly GUIContent dash = new GUIContent("-", "");
            public static readonly GUIContent thread = new GUIContent("Thread", "");

            public static readonly GUIContent max = new GUIContent("Max", "The peak value in the data set");
            public static readonly GUIContent upperQuartile = new GUIContent("Upper Quartile", "The middle value between the median and the highest value of the data set. I.e. at 75% of the ordered data.");
            public static readonly GUIContent mean = new GUIContent("Mean", "The average value in the data set");
            public static readonly GUIContent median = new GUIContent("Median", "The central value in the data set");
            public static readonly GUIContent lowerQuartile = new GUIContent("Lower Quartile", "The middle number between the smallest number and the median of the data set. I.e. at 25% of the ordered data.");
            public static readonly GUIContent min = new GUIContent("Min", "The minimum value in the data set");
            public static readonly GUIContent individualMin = new GUIContent("Individual Min", "The minimum value in the data set for an individual marker instance (not the totla in the frame)");
            public static readonly GUIContent individualMax = new GUIContent("Individual Max", "The minimum value in the data set for an individual marker instance (not the totla in the frame)");

            public static readonly GUIContent export = new GUIContent("Export", "Export profiler data as CSV files");
            public static readonly GUIContent pullOpen = new GUIContent("Pull Data", "Pull data from Unity profiler.\nFirst you must open Unity profiler to pull data from it");
            public static readonly GUIContent pullRange = new GUIContent("Pull Data", "Pull data from Unity profiler.\nFirst you must use the Unity profiler to capture data from application");
            public static readonly GUIContent pull = new GUIContent("Pull Data", "Pull data from Unity profiler");
            public static readonly GUIContent nameFilter = new GUIContent("Name Filter : ", "Only show markers containing the strings");
            public static readonly GUIContent nameExclude = new GUIContent("Exclude Names : ", "Excludes markers containing the strings");
            public static readonly GUIContent threadFilter = new GUIContent("Thread : ", "Select threads to focus on");
            public static readonly GUIContent threadFilterSelect = new GUIContent("Select", "Select threads to focus on");
            public static readonly GUIContent unitFilter = new GUIContent("Units : ", "Units to show in UI");
            public static readonly GUIContent timingFilter = new GUIContent("Analysis Type : ", TimingOptions.Tooltip);
            public static readonly GUIContent markerColumns = new GUIContent("Marker Columns : ");
            public static readonly GUIContent graphPairing = new GUIContent("Pair Graph Selection", "Selections on one graph will affect the other");

            public static readonly GUIContent frameSummary = new GUIContent("Frame Summary", "");
            public static readonly GUIContent threadSummary = new GUIContent("Thread Summary", "");
            public static readonly GUIContent markerSummary = new GUIContent("Marker Summary", "");
            public static readonly GUIContent frameRange = new GUIContent("Frame Range", "");
            public static readonly GUIContent filters = new GUIContent("Filters", "");
            public static readonly GUIContent profileTable = new GUIContent("Marker Details for currently selected range", "");
            public static readonly GUIContent comparisonTable = new GUIContent("Marker Comparison for currently selected range", "");

            public static readonly GUIContent depthTitle = new GUIContent("Depth Slice : ", "Marker callstack depth to analyze");
            public static readonly GUIContent leftDepthTitle = new GUIContent("Left : ", "Marker callstack depth to analyze");
            public static readonly GUIContent rightDepthTitle = new GUIContent(" Right : ", "Marker callstack depth to analyze");
            public static readonly GUIContent parentMarker = new GUIContent("Parent Marker : ", "Marker to start analysis from. Parent of the hierarchy to analyze.");
            public static readonly GUIContent selectParentMarker = new GUIContent("None", "Select using right click context menu on marker names in marker table");

            public static readonly GUIContent topMarkerRatio = new GUIContent("Ratio : ", "Normalise\tNormalised to time of the individual set\nLongest\tRatio based on longest time of the two");

            public static readonly GUIContent[] topTenDisplayOptions = {
                new GUIContent("Normalised", "Ratio normalised to time of the individual data set"),
                new GUIContent("Longest", "Ratio based on longest time of the two data sets")
            };

            public static readonly GUIContent[] nameFilterOperation = {
                new GUIContent("All", "Marker name contains all strings"),
                new GUIContent("Any", "Marker name contains any of the strings")
            };

            public static readonly GUIContent menuItemSelectFramesInAll = new GUIContent("Select Frames that contain this marker (within whole data set)", "");
            public static readonly GUIContent menuItemSelectFramesInCurrent = new GUIContent("Select Frames that contain this marker (within current selection)", "");
            public static readonly GUIContent menuItemSelectFramesAll = new GUIContent("Clear Selection", "");

            public static readonly GUIContent frameCosts = new GUIContent(" by frame costs", "Contains accumulated marker cost within the frame");
            public static readonly GUIContent dataMissing = new GUIContent("Pull or load a data set for analysis", "Pull data from Unity Profiler or load a previously saved analysis data set");
            public static readonly GUIContent comparisonDataMissing = new GUIContent("Pull or load a data set for comparison", "Pull data from Unity Profiler or load previously saved analysis data sets");

            public static readonly string topMarkersTooltip = "Top markers for the median frame.\nThe length of this frame is the median of those in the data set.\nIt is likely to be the most representative frame.";
            public static readonly string medianFrameTooltip = "The length of this frame is the median of those in the data set.\nIt is likely to be the most representative frame.";

            public static readonly string helpText = 
@"This tool can analyze Unity Profiler data, to find representative frames and perform comparisons of data sets.

To gather data to analyze:
* Open the Unity Profiler. Either via the Unity menu under 'Windows', 'Analysis' or via the 'Open Profile Window' in the tool bar.
* Capture some profiling data in the Unity Profiler by selecting a target application and click the 'Record' button.
* Stop the capture by clicking again on the 'Record' button.

To analyze the data:
* Pull the Unity Profiler data into this tool by clicking the 'Pull Data' button in the single or compare views.
* The analysis will be automatically triggered (in the compare view two data sets are required before analysis is performed).
* Select a marker to see more detailed information about its time utilization over the frame time range.
* Save off a data file from here to keep for future use. (Recommend saving the profile .data file in the same folder).

To compare two data sets:
* Click the compare tab. The data in the single tab will be used by default. You can also load previously saved analysis data.
* Drag select a region in the frame time graph (above) to choose 1 or more frames for each of the two data sets.
* The comparison will be automatically triggered as the selection is made.";
        }


        static public Color Color256(int r, int g, int b, int a)
        {
            return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, (float)a / 255.0f);
        }

        private ProgressBarDisplay m_ProgressBar;
        private ProfileAnalyzer m_ProfileAnalyzer;

        private ProfilerWindowInterface m_ProfilerWindowInterface;
        private string m_LastProfilerSelectedMarker;

        private int m_TopNumber;
        private string[] m_TopStrings;
        private int[] m_TopValues;
        private string[] m_DepthStrings;
        private int[] m_DepthValues;
        private string[] m_DepthStrings1;
        private int[] m_DepthValues1;
        private string[] m_DepthStrings2;
        private int[] m_DepthValues2;

        [SerializeField]
        private int m_DepthFilter = -1;
        [SerializeField]
        private int m_DepthFilter1 = -1;
        [SerializeField]
        private int m_DepthFilter2 = -1;
        [SerializeField]
        private bool m_DepthFilter2Auto = true;
        [SerializeField]
        private TimingOptions.TimingOption m_TimingOption = TimingOptions.TimingOption.Time;

        [SerializeField]
        private string m_ParentMarker = null;

        private List<string> m_ThreadUINames = new List<string>();
        private List<string> m_ThreadNames = new List<string>();
        private ThreadSelection m_ThreadSelection = new ThreadSelection();
        private ThreadSelection m_ThreadSelectionNew;
        
        private DisplayUnits m_DisplayUnits = new DisplayUnits(Units.Milliseconds);
        private string[] m_UnitNames;
        private string m_NameFilter = "";
        private string m_NameExclude = "";
        private MarkerColumnFilter m_SingleModeFilter = new MarkerColumnFilter(MarkerColumnFilter.Mode.TimeAndCount);
        private MarkerColumnFilter m_CompareModeFilter = new MarkerColumnFilter(MarkerColumnFilter.Mode.TimeAndCount);
        private TopTenDisplay m_TopTenDisplay = TopTenDisplay.Normalised;
        private NameFilterOperation m_NameFilterOperation = NameFilterOperation.All;
        private NameFilterOperation m_NameExcludeOperation = NameFilterOperation.Any;

        private int m_ProfilerFirstFrameIndex = 0;
        private int m_ProfilerLastFrameIndex = 0;
        private int m_PullFirstFrameindex = 0;
        private int m_PullLastFrameindex = 0;

        private ActiveTab m_NextActiveTab = ActiveTab.Summary;
        private ActiveTab m_ActiveTab = ActiveTab.Summary;
        private bool m_OtherTabDirty = false;
        private bool m_OtherTableDirty = false;

        private bool m_ShowFilters = true;
        private bool m_ShowTopNMarkers = true;
        private bool m_ShowFrameSummary = true;
        private bool m_ShowThreadSummary = false;
        private bool m_ShowMarkerSummary = true;
        private bool m_ShowMarkerTable = true;

        public Color m_ColorWhite = new Color(1.0f, 1.0f, 1.0f);
        public Color m_ColorBarBackground = new Color(0.5f, 0.5f, 0.5f);
        public Color m_ColorBarBackgroundSelected = new Color(0.6f, 0.6f, 0.6f);
        public Color m_ColorBoxAndWhiskerBoxColor = Color256(112, 112, 112, 255);
        public Color m_ColorBoxAndWhiskerLineColorLeft = Color256(206, 219, 238, 255);
        public Color m_ColorBoxAndWhiskerBoxColorLeft = Color256(59, 104, 144, 255);
        public Color m_ColorBoxAndWhiskerLineColorRight = Color256(247, 212, 201, 255);
        public Color m_ColorBoxAndWhiskerBoxColorRight = Color256(161, 83, 30, 255);
        public Color m_ColorBar = new Color(0.95f, 0.95f, 0.95f);
        public Color m_ColorBarSelected = new Color(0.5f, 1.0f, 0.5f);
        public Color m_ColorStandardLine = new Color(1.0f, 1.0f, 1.0f);
        public Color m_ColorGridLines = new Color(0.4f, 0.4f, 0.4f);

        public Color m_ColorLeft = Color256(111, 163, 216, 255);
        public Color m_ColorLeftSelected = Color256(06, 219, 238, 255);
        public Color m_ColorRight = Color256(238, 134, 84, 255);
        public Color m_ColorRightSelected = Color256(247, 212, 201, 255);
        public Color m_ColorBoth = Color256(175, 150, 150, 255);
        public Color m_ColorTextTopMarkers = Color256(0, 0, 0, 255);
        public Color m_ColorMarker = new Color(0.0f, 0.5f, 0.5f);
        public Color m_ColorMarkerSelected = new Color(0.0f, 0.6f, 0.6f);
        public Color m_ColorThread = new Color(0.5f, 0.0f, 0.5f);
        public Color m_ColorThreadSelected = new Color(0.6f, 0.0f, 0.6f);

        [SerializeField]
        ProfileDataView m_ProfileSingleView;
        [SerializeField]
        ProfileDataView m_ProfileLeftView;
        [SerializeField]
        ProfileDataView m_ProfileRightView;

        int m_SelectedMarker = 0;
        string m_SelectedMarkerName;

        FrameTimeGraphGlobalSettings m_FrameTimeGraphGlobalSettings;
        FrameTimeGraph m_FrameTimeGraph;
        FrameTimeGraph m_LeftFrameTimeGraph;
        FrameTimeGraph m_RightFrameTimeGraph;
        bool m_FrameTimeGraphsPaired = true;

        List<MarkerPairing> m_PairingsNew;
        List<MarkerPairing> m_Pairings = new List<MarkerPairing>();
        int m_SelectedPairing = 0;
        int m_DepthDiff = 0;

        TreeViewState m_ProfileTreeViewState;
        MultiColumnHeaderState m_ProfileMulticolumnHeaderState;
        ProfileTable m_ProfileTable;

        TreeViewState m_ComparisonTreeViewState;
        MultiColumnHeaderState m_ComparisonMulticolumnHeaderState;
        ComparisonTable m_ComparisonTable;

        internal static class LayoutSize
        {
            public static readonly int WindowWidth = 1170;
            public static readonly int WindowHeight = 840;
            public static readonly int WidthRHS = 276;        // Column widths + label padding between
            public static readonly int WidthColumn0 = 100;
            public static readonly int WidthColumn1 = 50;
            public static readonly int WidthColumn2 = 50;
            public static readonly int WidthColumn3 = 50;
            public static readonly int FilterOptionsLeftLabelWidth = 100;
            public static readonly int FilterOptionsEnumWidth = 50;
            public static readonly int FilterOptionsRightLabelWidth = 110;
            public static readonly int FilterOptionsRightEnumWidth = 150;
            public static readonly int HistogramWidth = 150;
        }

        Columns m_Columns = new Columns(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);

        ThreadRange m_ThreadRange = ThreadRange.UpperQuartile;
        string[] m_ThreadRanges = { "Median frame time", "Upper quartile of frame time", "Max frame time" };

        public Draw2D m_2D;

        bool m_Async = true;
        Thread m_BackgroundThread;
        ThreadActivity m_ThreadActivity;
        int m_ThreadPhase;
        int m_ThreadPhases;
        int m_ThreadProgress;

        bool m_RequestRepaint;
        bool m_RequestAnalysis;
        bool m_RequestCompare;
        bool m_FullAnalysisRequired;
        bool m_FullCompareRequired;

        int m_TopNBars = 10;

        bool m_EnableAnalysisProfiling = false;
        int m_AnalyzeInUpdatePhase = 0;

        string m_LastAnalysisTime = "";
        string m_LastCompareTime = "";
        float m_LastAnalysisTimeMilliseconds;
        float m_LastCompareTimeMilliseconds;
        bool m_NewDataLoaded = false;
        bool m_NewComparisonDataLoaded = false;

        Vector2 m_HelpScroll = new Vector2(0, 0);
        Vector2 m_ThreadScroll = new Vector2(0, 0);

        Vector2 m_LastScreenSize = new Vector2(0, 0);
        bool m_ScreenSizeChanged;
        double m_ScreenSizeChangedTimeStarted;
        double m_ScreenSizeChangedTimeFinished;
        ActiveTab m_ScreenSizeChangedTab;

        GUIStyle m_StyleMiddleRight;
        bool m_StylesSetup = false;

#if UNITY_2018_1_OR_NEWER
        [MenuItem("Window/Analysis/Profile Analyzer")]
#else
        [MenuItem("Window/Profile Analyzer")]
#endif
        private static void Init()
        {
            var window = GetWindow<ProfileAnalyzerWindow>("Profile Analyzer");
            window.minSize = new Vector2(800, 480);
            window.position.size.Set(LayoutSize.WindowWidth, LayoutSize.WindowHeight);
            window.Show();
            window.m_LastScreenSize = window.position.size;
        }

        public static void OpenProfileAnalyzer()
        {
			Init();
        }

        private void Awake()
        {
            m_ScreenSizeChanged = false;
            m_ScreenSizeChangedTimeStarted = 0.0;
            m_ScreenSizeChangedTimeFinished = 0.0;
            m_ScreenSizeChangedTab = ActiveTab.Summary;

            m_ProfileSingleView = new ProfileDataView();
            m_ProfileLeftView = new ProfileDataView();
            m_ProfileRightView = new ProfileDataView();

            m_RequestRepaint = false;
            m_RequestAnalysis = false;
            m_RequestCompare = false;

            m_FrameTimeGraphGlobalSettings = new FrameTimeGraphGlobalSettings();
        }

        private void OnEnable()
        {
            ProfileAnalyzerAnalytics.EnableAnalytics();

            m_ProfilerWindowInterface = new ProfilerWindowInterface();

            m_ProgressBar = new ProgressBarDisplay();
            m_ProfileAnalyzer = new ProfileAnalyzer(m_ProgressBar);

            ThreadIdentifier mainThreadSelection = new ThreadIdentifier("Main Thread", 1);
            m_ThreadSelection.Set(mainThreadSelection.threadNameWithIndex);

            m_2D = new Draw2D("Unlit/ProfileAnalyzerShader");
            FrameTimeGraph.SetGlobalSettings(m_FrameTimeGraphGlobalSettings);
            m_FrameTimeGraph = new FrameTimeGraph(m_2D, m_DisplayUnits.Units, m_ColorBarBackground, m_ColorBarBackgroundSelected, m_ColorBar, m_ColorBarSelected, m_ColorMarker, m_ColorMarkerSelected, m_ColorThread, m_ColorThreadSelected, m_ColorGridLines);
            m_FrameTimeGraph.SetRangeCallback(SetRange); 
            m_LeftFrameTimeGraph = new FrameTimeGraph(m_2D, m_DisplayUnits.Units, m_ColorBarBackground, m_ColorBarBackgroundSelected, m_ColorLeft, m_ColorLeftSelected, m_ColorMarker, m_ColorMarkerSelected, m_ColorThread, m_ColorThreadSelected, m_ColorGridLines);
            m_LeftFrameTimeGraph.SetRangeCallback(SetLeftRange);
            m_RightFrameTimeGraph = new FrameTimeGraph(m_2D, m_DisplayUnits.Units, m_ColorBarBackground, m_ColorBarBackgroundSelected, m_ColorRight, m_ColorRightSelected, m_ColorMarker, m_ColorMarkerSelected, m_ColorThread, m_ColorThreadSelected, m_ColorGridLines);
            m_RightFrameTimeGraph.SetRangeCallback(SetRightRange);
            m_LeftFrameTimeGraph.PairWith(m_FrameTimeGraphsPaired ? m_RightFrameTimeGraph : null);

            m_ThreadActivity = ThreadActivity.None;
            m_ThreadProgress = 0;
            m_ThreadPhase = 0;

            List<int> values = new List<int>();
            List<String> strings = new List<string>();
            for (int i = 1; i <= 10; i++)
            {
                values.Add(i);
                strings.Add(i.ToString());
            }
            m_TopValues = values.ToArray();
            m_TopStrings = strings.ToArray();
            m_TopNumber = 3;
         
            List<string> unitNames = new List<string>(DisplayUnits.UnitNames);
            unitNames.RemoveAt(unitNames.Count-1);
            m_UnitNames = unitNames.ToArray();

            // Regrenerate analysis if just re initialised with the existing profile data reloaded from serialisation (e.g. on enter play mode)
            // As we don't serialise the analysis itself.
            UpdateActiveTab(true);
        }
        
        private bool DisplayCount()
        {
            switch (m_SingleModeFilter.mode)
            {
                case MarkerColumnFilter.Mode.CountTotals:
                case MarkerColumnFilter.Mode.CountPerFrame:
                    return true;
                default:
                    return false;
            }
        }

        private void OpenProfilerOrUseExisting()
        {
            m_ProfilerWindowInterface.OpenProfilerOrUseExisting();
            m_ProfilerWindowInterface.GetFrameRangeFromProfiler(out m_ProfilerFirstFrameIndex, out m_ProfilerLastFrameIndex);
            m_PullFirstFrameindex = m_ProfilerFirstFrameIndex;
            m_PullLastFrameindex = m_ProfilerLastFrameIndex;
        }

        private void OnGUI()
        {
            m_2D.OnGUI();

            Draw();
        }

        private void SetView(ProfileDataView dst, ProfileData data, string path, FrameTimeGraph graph)
        {
            if (!data.IsSame(dst.data))
            {
                if (dst == m_ProfileSingleView)
                    m_NewDataLoaded = true;
                else
                    m_NewComparisonDataLoaded = true;
            }

            dst.data = data;
            dst.path = path;
            dst.SelectFullRange();

            graph.Reset();
            graph.SetData(GetFrameTimeData(dst.data));
        }

        private void SetView(ProfileDataView dst, ProfileDataView src, FrameTimeGraph graph)
        {
            SetView(dst, src.data, src.path, graph);
        }

        private void ProcessTabSwitch()
        {
            if (m_NextActiveTab != m_ActiveTab)
            {
                m_ActiveTab = m_NextActiveTab;

                // Copy data if none present for this tab
                switch (m_ActiveTab)
                {
                    case ActiveTab.Summary:
                        if (m_ProfileSingleView.data == null)
                        {
                            if (m_ProfileLeftView.data != null)
                            {
                                SetView(m_ProfileSingleView, m_ProfileLeftView, m_FrameTimeGraph);

                                m_RequestAnalysis = true;
                                m_FullAnalysisRequired = true;
                            }
                            else if (m_ProfileRightView.data != null)
                            {
                                SetView(m_ProfileSingleView, m_ProfileRightView, m_FrameTimeGraph);

                                m_RequestAnalysis = true;
                                m_FullAnalysisRequired = true;
                            }
                        }
                        break;
                    case ActiveTab.Compare:
                        if ((m_ProfileLeftView.data == null || m_ProfileRightView.data == null) && m_ProfileSingleView.data != null)
                        {
                            if (m_ProfileLeftView.data == null)
                            {
                                SetView(m_ProfileLeftView, m_ProfileSingleView, m_LeftFrameTimeGraph);
                            }

                            if (m_ProfileRightView.data == null)
                            {
                                SetView(m_ProfileRightView, m_ProfileSingleView, m_RightFrameTimeGraph);
                            }

                            // Remove pairing of both left/right point at the same data
                            if (m_ProfileLeftView.path == m_ProfileRightView.path)
                            {
                                SetFrameTimeGraphPairing(false);
                            }

                            m_RequestCompare = true;
                            m_FullCompareRequired = true;
                        }
                        break;
                }

                // Update threads list
                switch (m_ActiveTab)
                {
                    case ActiveTab.Summary:
                        GetThreadNames(m_ProfileSingleView.data, out m_ThreadUINames, out m_ThreadNames);
                        UpdateThreadGroupSelection(m_ThreadNames, m_ThreadSelection);
                        break;
                    case ActiveTab.Compare:
                        GetThreadNames(m_ProfileLeftView.data, m_ProfileRightView.data, out m_ThreadUINames, out m_ThreadNames);
                        UpdateThreadGroupSelection(m_ThreadNames, m_ThreadSelection);
                        break;
                }

                SelectMarker(m_SelectedMarkerName);

                if (m_OtherTabDirty)
                {
                    UpdateActiveTab(true,false);  // Make sure any depth/thread updates are applied when switching tabs, but don't dirty the other tab
                    m_OtherTabDirty = false;
                }

                if (m_OtherTableDirty)
                {
                    UpdateMarkerTable(false);  // Make sure any marker selection updates are applied when switching tabs, but don't dirty the other tab
                    m_OtherTableDirty = false;
                }
            }
        }

        bool IsDocked()
        {
            BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            MethodInfo isDockedMethod = typeof(EditorWindow).GetProperty("docked", fullBinding).GetGetMethod(true);
            bool isDocked = (bool)isDockedMethod.Invoke(this, null);

            return isDocked;
        }

        private void CheckScreenSizeChanges()
        {
            // We get a 5 pixel change in y height during initialization.
            // We could wait before considering size changes but using a delta is also useful
            float sizeDeltaForChange = 10;

            Vector2 sizeDiff = position.size - m_LastScreenSize;
            if (Math.Abs(sizeDiff.x) > sizeDeltaForChange || Math.Abs(sizeDiff.y) > sizeDeltaForChange)
            {
                if (m_LastScreenSize.x != 0) // At initialization time the screen size has not yet been recorded. Don't consider this a screen size change
                {
                    m_LastScreenSize = position.size;
                    if (!m_ScreenSizeChanged)
                    {
                        // Record when we started the change
                        m_ScreenSizeChanged = true;
                        m_ScreenSizeChangedTimeStarted = EditorApplication.timeSinceStartup;
                    }
                    // Record the last time of a change
                    m_ScreenSizeChangedTimeFinished = EditorApplication.timeSinceStartup;

                    // Record which tab we were on when it was changed
                    m_ScreenSizeChangedTab = m_ActiveTab;
                }
            }

            if (m_ScreenSizeChanged)
            {
                double secondsSinceChanged = (EditorApplication.timeSinceStartup - m_ScreenSizeChangedTimeFinished);
                double secondsToDelay = 3f;
                if (secondsSinceChanged > secondsToDelay)
                {
                    // Send analytic 
                    var uiResizeView = m_ScreenSizeChangedTab == ActiveTab.Summary ? ProfileAnalyzerAnalytics.UIResizeView.Single : ProfileAnalyzerAnalytics.UIResizeView.Comparison;
                    float durationInSeconds = (float)(m_ScreenSizeChangedTimeFinished - m_ScreenSizeChangedTimeStarted);
                    ProfileAnalyzerAnalytics.SendUIResizeEvent(uiResizeView, durationInSeconds, position.size.x, position.size.y, IsDocked());

                    m_ScreenSizeChanged = false;
                }
            }
        }

        public void RequestRepaint()
        {
            m_RequestRepaint = true;
        }

		private void Update()
		{
            CheckScreenSizeChanges();

            // Check if profiler is open
            if (m_ProfilerWindowInterface.IsReady())
            {
                // Check if the selected marker in the profiler has changed
                var selectedMarker = m_ProfilerWindowInterface.GetProfilerWindowMarkerName();
                if (selectedMarker != null && selectedMarker != m_LastProfilerSelectedMarker)
                {
                    m_LastProfilerSelectedMarker = selectedMarker;
                    SelectMarker(selectedMarker);
                }

                // Check if a new profile has been recorded (or loaded) by checking the frame index range.
                int first;
                int last;
                m_ProfilerWindowInterface.GetFrameRangeFromProfiler(out first, out last);
                if (first != m_ProfilerFirstFrameIndex || last != m_ProfilerLastFrameIndex)
                {
                    // Store the updated range and alter the pull range
                    m_ProfilerFirstFrameIndex = first;
                    m_ProfilerLastFrameIndex = last;
                    m_PullFirstFrameindex = m_ProfilerFirstFrameIndex;
                    m_PullLastFrameindex = m_ProfilerLastFrameIndex;
                }
            }
            else
            {
                if (m_ProfilerWindowInterface.IsProfilerWindowOpen())
                {
                    m_ProfilerWindowInterface.OpenProfilerOrUseExisting();
                }
            }


            // Deferred to here so drawing isn't messed up by changing tab half way through a function rendering the old tab
            ProcessTabSwitch();

            // Force repaint for the progress bar
            if (IsAnalysisRunning())
            {
                int progress = m_ProfileAnalyzer.GetProgress();
                if (m_ThreadPhases > 1)
                {
                    progress = ((100 * m_ThreadPhase) + progress) / m_ThreadPhases;
                }
                   
                
                if (m_ThreadProgress != progress)
                {
                    m_ThreadProgress = progress;
                    m_RequestRepaint = true;
                }
            }
            
            if (m_ThreadSelectionNew != null)
            {
                m_ThreadSelection = new ThreadSelection(m_ThreadSelectionNew);
                m_ThreadSelectionNew = null;
            }

            switch (m_ThreadActivity)
            {
                case ThreadActivity.AnalyzeDone:
                    // Create table when analysis complete
                    UpdateAnalysisFromAsyncProcessing(m_ProfileSingleView, m_FullAnalysisRequired);
                    m_FullAnalysisRequired = false;

                    if (m_ProfileSingleView.analysis != null)
                    {
                        CreateProfileTable();
                        m_RequestRepaint = true;
                    }
                    m_ThreadActivity = ThreadActivity.None;

                    if (m_NewDataLoaded)
                    {
                        if (m_ProfileSingleView.data != null)
                        {
                            // Don't bother sending an analytic if the data set is empty (should never occur anyway but consistent with comparison flow)
                            ProfileAnalyzerAnalytics.SendUIUsageModeEvent(ProfileAnalyzerAnalytics.UIUsageMode.Single, m_LastAnalysisTimeMilliseconds / 1000f);
                        }
                        m_NewDataLoaded = false;
                    }
                    break;

                case ThreadActivity.CompareDone:
                    UpdateAnalysisFromAsyncProcessing(m_ProfileLeftView, m_FullCompareRequired);
                    UpdateAnalysisFromAsyncProcessing(m_ProfileRightView, m_FullCompareRequired);
                    m_FullCompareRequired = false;
                    m_Pairings = m_PairingsNew;
                    
                    GetThreadNames(m_ProfileLeftView.data, m_ProfileRightView.data, out m_ThreadUINames, out m_ThreadNames);

                    if (m_ProfileLeftView.analysis != null && m_ProfileRightView.analysis != null)
                    {
                        CreateComparisonTable();
                        m_RequestRepaint = true;
                    }
                    m_ThreadActivity = ThreadActivity.None;

                    if (m_NewComparisonDataLoaded)
                    {
                        if (m_ProfileLeftView.data != null && m_ProfileRightView.data != null)
                        {
                            // Don't bother sending an analytic when one (or more) of the data sets is blank (as no comparison is really made)
                            ProfileAnalyzerAnalytics.SendUIUsageModeEvent(ProfileAnalyzerAnalytics.UIUsageMode.Comparison, m_LastCompareTimeMilliseconds / 1000f);
                        }
                        m_NewComparisonDataLoaded = false;
                    }
                    break;
            }

            if (m_RequestAnalysis)
            {
                if (!IsAnalysisRunning())
                {
                    Analyze();
                    m_RequestAnalysis = false;
                }
            }
            if (m_RequestCompare)
            {
                if (!IsAnalysisRunning())
                {
                    Compare();
                    m_RequestCompare = false;
                }
            }

            if (m_RequestRepaint)
            {
                Repaint();
                m_RequestRepaint = false;
            }

            if (m_AnalyzeInUpdatePhase > 0)
            {
                switch (m_AnalyzeInUpdatePhase)
                {
                    case 1:
                        UnityEngine.Profiling.Profiler.enabled = true;
                        m_AnalyzeInUpdatePhase++;
                        return;
                    case 2:
                        AnalyzeSync();
                        UpdateAnalysisFromAsyncProcessing(m_ProfileSingleView, m_FullAnalysisRequired);
                        m_FullAnalysisRequired = false;
                        m_AnalyzeInUpdatePhase++;
                        return;
                    case 3:
                        m_AnalyzeInUpdatePhase++;
                        return;
                    case 4:
                        UnityEngine.Profiling.Profiler.enabled = false;
                        m_AnalyzeInUpdatePhase++;
                        return;
                    default:
                        m_AnalyzeInUpdatePhase = 0;
                        break;
                }
            }
        }

        private void UpdateAnalysisFromAsyncProcessing(ProfileDataView view, bool full)
        {
            view.analysis = view.analysisNew;
            if (full)
            {
                if (view.selectedIndices!=null && view.data!=null && view.selectedIndices.Count == view.data.GetFrameCount())
                    view.analysisFull = view.analysis;
                else
                    view.analysisFull = view.analysisFullNew;
            }
        }

        private List<FrameTimeGraph.Data> GetFrameTimeData(ProfileData profileData)
        {
            List<FrameTimeGraph.Data> data = new List<FrameTimeGraph.Data>();
            int frames = profileData.GetFrameCount();

            for (int frameOffset = 0; frameOffset < frames; frameOffset++)
            {
                float ms = profileData.GetFrame(frameOffset).msFrame;
                FrameTimeGraph.Data dataPoint = new FrameTimeGraph.Data(ms, frameOffset);
                data.Add(dataPoint);
            }

            return data;
        }

        private void Load()
        {
            string path = EditorUtility.OpenFilePanel("Load profile analyzer data file", "", "pdata");
            if (path.Length != 0)
            {
                ProfileData newData;
                if (ProfileData.Load(path, out newData))
                {
                    SetView(m_ProfileSingleView, newData, path, m_FrameTimeGraph);

                    m_RequestAnalysis = true;
                    m_FullAnalysisRequired = true;
                }
            }
        }

        private void UpdateMatchingProfileData(ProfileData data, ref string path, ProfileAnalysis analysis, string newPath)
        {
            // Update left/right data if we are effectively overwriting it.
            if (m_ProfileLeftView.path == newPath)
            {
                SetView(m_ProfileLeftView, data, newPath, m_LeftFrameTimeGraph);

                m_RequestCompare = true;
                m_FullCompareRequired = true;
            }
            if (m_ProfileRightView.path == newPath)
            {
                SetView(m_ProfileRightView, data, newPath, m_RightFrameTimeGraph);

                m_RequestCompare = true;
                m_FullCompareRequired = true;
            }

            // Update single view if needed
            if (m_ProfileSingleView.path == newPath)
            {
                SetView(m_ProfileSingleView, data, newPath, m_FrameTimeGraph);

                m_ProfileSingleView.analysis = analysis;
            }

            path = newPath;
        }

        private void Save()
        {
            string newPath = EditorUtility.SaveFilePanel("Save profile analyzer data file", "", "capture.pdata", "pdata");
            if (newPath.Length != 0)
            {
                if (ProfileData.Save(newPath, m_ProfileSingleView.data))
                {
                    UpdateMatchingProfileData(m_ProfileSingleView.data, ref m_ProfileSingleView.path, m_ProfileSingleView.analysis, newPath);
                }
            }
        }

        List<MarkerPairing> GeneratePairings(ProfileAnalysis leftAnalysis, ProfileAnalysis rightAnalysis)
        {
            if (leftAnalysis == null)
                return null;
            if (rightAnalysis == null)
                return null;
            List<MarkerData> leftMarkers = leftAnalysis.GetMarkers();
            if (leftMarkers == null)
                return null;
            List<MarkerData> rightMarkers = rightAnalysis.GetMarkers();
            if (rightMarkers == null)
                return null;

            Dictionary<string, MarkerPairing> markerPairs = new Dictionary<string, MarkerPairing>();
            for (int index = 0; index < leftMarkers.Count; index++)
            {
                MarkerData marker = leftMarkers[index];

                MarkerPairing pair = new MarkerPairing
                {
                    name = marker.name,
                    leftIndex = index,
                    rightIndex = -1
                };
                markerPairs[marker.name] = pair;
            }
            for (int index = 0; index < rightMarkers.Count; index++)
            {
                MarkerData marker = rightMarkers[index];

                if (markerPairs.ContainsKey(marker.name))
                {
                    MarkerPairing pair = markerPairs[marker.name];
                    pair.rightIndex = index;
                    markerPairs[marker.name] = pair;
                }
                else
                {
                    MarkerPairing pair = new MarkerPairing
                    {
                        name = marker.name,
                        leftIndex = -1,
                        rightIndex = index
                    };
                    markerPairs[marker.name] = pair;
                }
            }

            List<MarkerPairing> pairings = new List<MarkerPairing>();
            foreach (MarkerPairing pair in markerPairs.Values)
                pairings.Add(pair);

            return pairings;
        }

        private void SetThreadPhaseCount(ThreadActivity activity)
        {
            // Will be refined by the analysis functions
            if (activity == ThreadActivity.Compare)
            {
                m_ThreadPhases = 8;
            }
            else
            {
                m_ThreadPhases = 2;
            }
        }

        private void BeginAsyncAction(ThreadActivity activity)
        {
            if (IsAnalysisRunning())
                return;

            m_ThreadActivity = activity;
            m_ThreadProgress = 0;
            m_ThreadPhase = 0;
            SetThreadPhaseCount(activity);

            m_BackgroundThread = new Thread(BackgroundThread);
            m_BackgroundThread.Start();
        }

        private void CreateComparisonTable()
        {
            GetThreadNames(m_ProfileLeftView.data, m_ProfileRightView.data, out m_ThreadUINames, out m_ThreadNames);
            UpdateThreadGroupSelection(m_ThreadNames, m_ThreadSelection);

            if (m_ComparisonTreeViewState == null)
                m_ComparisonTreeViewState = new TreeViewState();

            //if (m_ComparisonMulticolumnHeaderState == null)
            m_ComparisonMulticolumnHeaderState = ComparisonTable.CreateDefaultMultiColumnHeaderState(700, m_CompareModeFilter);

            var multiColumnHeader = new MultiColumnHeader(m_ComparisonMulticolumnHeaderState);
            multiColumnHeader.SetSorting((int)ComparisonTable.MyColumns.AbsDiff, false);
            multiColumnHeader.ResizeToFit();
            m_ComparisonTable = new ComparisonTable(m_ComparisonTreeViewState, multiColumnHeader, m_ProfileLeftView.analysis, m_ProfileRightView.analysis, m_Pairings, this);

            if (string.IsNullOrEmpty(m_SelectedMarkerName))
                SelectPairing(0);
            else
                SelectPairingByName(m_SelectedMarkerName);
        }

        private void CalculatePairingbuckets(ProfileAnalysis left, ProfileAnalysis right, List<MarkerPairing> pairings)
        {
            var leftMarkers = left.GetMarkers();
            var rightMarkers = right.GetMarkers();
            foreach (var pairing in pairings)
            {
                float min = float.MaxValue;
                float max = 0.0f;
                MarkerData leftMarker = null;
                MarkerData rightMarker = null;
                if (pairing.leftIndex >= 0)
                {
                    leftMarker = leftMarkers[pairing.leftIndex];
                    max = Math.Max(max, leftMarker.msMax);
                    min = Math.Min(min, leftMarker.msMin);
                }
                if (pairing.rightIndex >= 0)
                {
                    rightMarker = rightMarkers[pairing.rightIndex];
                    max = Math.Max(max, rightMarker.msMax);
                    min = Math.Min(min, rightMarker.msMin);
                }
                
                int countMin = int.MaxValue;
                int countMax = 0;
                if (leftMarker!=null)
                {
                    countMax = Math.Max(countMax, leftMarker.countMax);
                    countMin = Math.Min(countMin, leftMarker.countMin);
                }
                if (rightMarker!=null)
                {
                    countMax = Math.Max(countMax, rightMarker.countMax);
                    countMin = Math.Min(countMin, rightMarker.countMin);
                }

                if (leftMarker != null)
                {
                    leftMarker.ComputeBuckets(min, max);
                    leftMarker.ComputeCountBuckets(countMin, countMax);
                }
                if (rightMarker != null)
                {
                    rightMarker.ComputeBuckets(min, max);
                    rightMarker.ComputeCountBuckets(countMin, countMax);
                }
            }
        }

        private int CalculateDepthDifference(ProfileAnalysis leftAnalysis, ProfileAnalysis rightAnalysis, List<MarkerPairing> pairings)
        {
            if (pairings.Count <= 0)
                return 0;

            var leftMarkers = leftAnalysis.GetMarkers();
            var rightMarkers = rightAnalysis.GetMarkers();

            int totalCount = 0;
            int[] depthDifferences = new int[pairings.Count];
            foreach (var pairing in pairings)
            {
                if (pairing.leftIndex >= 0 && pairing.rightIndex >= 0)
                {
                    MarkerData leftMarker = leftMarkers[pairing.leftIndex];
                    MarkerData rightMarker = rightMarkers[pairing.rightIndex];
                    int markerDepthDiff = rightMarker.minDepth - leftMarker.minDepth;
                    depthDifferences[totalCount] = markerDepthDiff;
                    totalCount += 1;
                }
            }

            // Take Median depth difference 
            // The average (totalDepthDiff / totalCount) would round down if one marker had 0 difference when others had 1 difference)
            m_DepthDiff = totalCount == 0 ? 0 : depthDifferences[totalCount/2];

            return m_DepthDiff;
        }

        private bool CompareSync()
        {
            if (m_ProfileLeftView.data == null)
                return false;
            if (m_ProfileRightView.data == null)
                return false;
           
            List<string> threadUINamesNew;
            List<string> threadNamesNew;

            GetThreadNames(m_ProfileLeftView.data, m_ProfileRightView.data, out threadUINamesNew, out threadNamesNew);
            List<string> threadSelection = GetLimitedThreadSelection(threadNamesNew, m_ThreadSelection);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            int updateDepthPhase = m_NewComparisonDataLoaded ? 2 : 0;
            int fullLeftPhase = (m_FullCompareRequired && m_ProfileLeftView.selectedIndices.Count != m_ProfileLeftView.data.GetFrameCount()) ? 1 : 0;
            int fullRightPhase = (m_FullCompareRequired && m_ProfileLeftView.selectedIndices.Count != m_ProfileLeftView.data.GetFrameCount()) ? 1 : 0;
            m_ThreadPhases = 2 + updateDepthPhase + fullLeftPhase + fullRightPhase + 2;

            bool selfTimes = IsSelfTime();

            // First scan just the frames
            m_ThreadPhase = 0;
            var leftAnalysisNew = m_ProfileAnalyzer.Analyze(m_ProfileLeftView.data, m_ProfileLeftView.selectedIndices, null, m_DepthFilter1, selfTimes, m_ParentMarker);
            m_ThreadPhase++;
            var rightAnalysisNew = m_ProfileAnalyzer.Analyze(m_ProfileRightView.data, m_ProfileRightView.selectedIndices, null, m_DepthFilter2, selfTimes, m_ParentMarker);
            m_ThreadPhase++;

            if (leftAnalysisNew == null || rightAnalysisNew == null)
            {
                stopwatch.Stop();
                return false;
            }

            // Calculate the max frame time of the two scans 
            float timeScaleMax = Math.Max(leftAnalysisNew.GetFrameSummary().msMax, rightAnalysisNew.GetFrameSummary().msMax);

            // Need to recalculate the depth difference when thread filters change
            // For now do it always if the depth is auto and not 'all'
            if (updateDepthPhase != 0)
            {
                var leftAnalysis = m_ProfileAnalyzer.Analyze(m_ProfileLeftView.data, m_ProfileLeftView.selectedIndices, threadSelection, -1, selfTimes, m_ParentMarker, timeScaleMax);
                m_ThreadPhase++;
                var rightAnalysis = m_ProfileAnalyzer.Analyze(m_ProfileRightView.data, m_ProfileRightView.selectedIndices, threadSelection, -1, selfTimes, m_ParentMarker, timeScaleMax);
                m_ThreadPhase++;

                var pairings = GeneratePairings(leftAnalysis, rightAnalysis);

                int originalDepthDiff = m_DepthDiff;
                int newDepthDiff = CalculateDepthDifference(leftAnalysis, rightAnalysis, pairings);
                if (newDepthDiff != originalDepthDiff)
                {
                    UpdateAutoDepthFilter();
                    
                    // New depth diff calculated to we need to do the full analysis
                    if (fullLeftPhase == 0)
                    {
                        fullLeftPhase = 1;
                        m_ThreadPhases++;
                    }
                    if (fullRightPhase == 0)
                    {
                        fullRightPhase = 1;
                        m_ThreadPhases++;
                    }
                }
            }

            // Now process the markers and setup buckets using the overall max frame time
            List<int> selection = new List<int>();
            if (fullLeftPhase != 0)
            {
                selection.Clear();
                for (int frameOffset = 0; frameOffset < m_ProfileLeftView.data.GetFrameCount(); frameOffset++)
                {
                    selection.Add(m_ProfileLeftView.data.OffsetToDisplayFrame(frameOffset));
                }

                m_ProfileLeftView.analysisFullNew = m_ProfileAnalyzer.Analyze(m_ProfileLeftView.data, selection, threadSelection, m_DepthFilter1, selfTimes, m_ParentMarker, timeScaleMax);
                m_ThreadPhase++;
            }

            if (fullRightPhase != 0)
            {
                selection.Clear();
                for (int frameOffset = 0; frameOffset < m_ProfileRightView.data.GetFrameCount(); frameOffset++)
                {
                    selection.Add(m_ProfileRightView.data.OffsetToDisplayFrame(frameOffset));
                }

                m_ProfileRightView.analysisFullNew = m_ProfileAnalyzer.Analyze(m_ProfileRightView.data, selection, threadSelection, m_DepthFilter2, selfTimes, m_ParentMarker, timeScaleMax);
                m_ThreadPhase++;
            }

            m_ProfileLeftView.analysisNew = m_ProfileAnalyzer.Analyze(m_ProfileLeftView.data, m_ProfileLeftView.selectedIndices, threadSelection, m_DepthFilter1, selfTimes, m_ParentMarker, timeScaleMax);
            m_ThreadPhase++;

            m_ProfileRightView.analysisNew = m_ProfileAnalyzer.Analyze(m_ProfileRightView.data, m_ProfileRightView.selectedIndices, threadSelection, m_DepthFilter2, selfTimes, m_ParentMarker, timeScaleMax);

            m_PairingsNew = GeneratePairings(m_ProfileLeftView.analysisNew, m_ProfileRightView.analysisNew);

            CalculatePairingbuckets(m_ProfileLeftView.analysisNew, m_ProfileRightView.analysisNew, m_PairingsNew);

            stopwatch.Stop();
            m_LastCompareTimeMilliseconds = stopwatch.ElapsedMilliseconds;

            TimeSpan ts = stopwatch.Elapsed;
            if (ts.Minutes > 0)
                m_LastCompareTime = string.Format("Last compare time {0} mins {1} secs {2} ms ", ts.Minutes, ts.Seconds, ts.Milliseconds);
            else if (ts.Seconds > 0)
                m_LastCompareTime = string.Format("Last compare time {0} secs {1} ms ", ts.Seconds, ts.Milliseconds);
            else
                m_LastCompareTime = string.Format("Last compare time {0} ms ", ts.Milliseconds);

            return true;
        }

        private void Compare()
        {
            if (m_Async)
            {
                m_ShowFilters = true;   // To see loading bar

                //m_comparisonTable = null;
                //m_ProfileLeftView.analysis = null;
                //m_ProfileRightView.analysis = null;
                BeginAsyncAction(ThreadActivity.Compare);
            }
            else
            {
                CompareSync();

                UpdateAnalysisFromAsyncProcessing(m_ProfileLeftView, m_FullCompareRequired);
                UpdateAnalysisFromAsyncProcessing(m_ProfileRightView, m_FullCompareRequired);
                m_FullCompareRequired = false;
            }
        }

        private List<MarkerPairing> GetPairings()
        {
            return m_Pairings;
        }

        private void GetFrameRangeFromProfiler()
        {
            m_ProfilerWindowInterface.GetFrameRangeFromProfiler(out m_PullFirstFrameindex, out m_PullLastFrameindex);
            m_ProfileAnalyzer.QuickScan();
        }

        private int GetUnsavedIndex(string path)
        {
            if (path == null)
                return 0;
            
            Regex unsavedRegExp = new Regex(@"^Unsaved[\s*]([\d]*)", RegexOptions.IgnoreCase);
            Match match = unsavedRegExp.Match(path);
            if (match.Length <= 0)
                return 0;

            return Int32.Parse(match.Groups[1].Value);
        }

        private void PullFromProfiler(int firstFrame, int lastFrame, ProfileDataView view, FrameTimeGraph frameTimeGraph)
        {
            m_ProgressBar.InitProgressBar("Pulling Frames from Profiler", "Please wait...", lastFrame - firstFrame);

            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            ProfileData newProfileData = m_ProfileAnalyzer.PullFromProfiler(firstFrame, lastFrame);
            ProfileAnalyzerAnalytics.SendUIButtonEvent(ProfileAnalyzerAnalytics.UIButton.Pull, analytic);

            frameTimeGraph.Reset();
            frameTimeGraph.SetData(GetFrameTimeData(newProfileData));

            // Check if this is new data (rather than repulling the same data)
            if (!newProfileData.IsSame(view.data))
            {
                if (view == m_ProfileSingleView)
                    m_NewDataLoaded = true;
                else
                    m_NewComparisonDataLoaded = true;
            }

            // Update the path to use the same saved file name if this is the same data as another view
            if (newProfileData.IsSame(m_ProfileLeftView.data))
            {
                view.path = m_ProfileLeftView.path;
            }
            else if (newProfileData.IsSame(m_ProfileRightView.data))
            {
                view.path = m_ProfileRightView.path;
            }
            else if (newProfileData.IsSame(m_ProfileSingleView.data))
            {
                view.path = m_ProfileSingleView.path;
            }
            else
            {
                int lastIndex = 0;
                lastIndex = Math.Max(lastIndex, GetUnsavedIndex(m_ProfileLeftView.path));
                lastIndex = Math.Max(lastIndex, GetUnsavedIndex(m_ProfileRightView.path));
                view.path = string.Format("Unsaved {0}", lastIndex + 1);
            }

            view.data = newProfileData;
            view.SelectFullRange();

            // Remove pairing if both left/right point at the same data
            if (m_ProfileLeftView.path == m_ProfileRightView.path)
            {
                SetFrameTimeGraphPairing(false);
            }

            m_ProgressBar.ClearProgressBar();
        }
            
        private void BackgroundThread()
        {
            switch (m_ThreadActivity)
            {
                case ThreadActivity.Analyze:
                    AnalyzeSync();
                    m_ThreadActivity = ThreadActivity.AnalyzeDone;
                    break;

                case ThreadActivity.Compare:
                    CompareSync();
                    m_ThreadActivity = ThreadActivity.CompareDone;
                    break;

                case ThreadActivity.AnalyzeDone: 
                    break;

                case ThreadActivity.CompareDone:    
                    break;

                default:
                    // m_threadActivity = ThreadActivity.None;
                    break;
            }
        }

        private void SelectFirstMarkerInTable()
        {
            // SelectMarker(0) would only select the first one found, not the first in the sorted list

            if (m_ProfileTable == null)
                return;

            var rows = m_ProfileTable.GetRows();
            if (rows==null || rows.Count<1)
                return;

            SelectMarkerByName(rows[0].displayName);
        }

        private void CreateProfileTable()
        {
            if (m_ProfileTreeViewState == null)
                m_ProfileTreeViewState = new TreeViewState();

            //if (m_ProfileMulticolumnHeaderState == null)
            m_ProfileMulticolumnHeaderState = ProfileTable.CreateDefaultMultiColumnHeaderState(700, m_SingleModeFilter);

            var multiColumnHeader = new MultiColumnHeader(m_ProfileMulticolumnHeaderState);
            multiColumnHeader.SetSorting((int)ProfileTable.MyColumns.Median, false);
            multiColumnHeader.ResizeToFit();
            m_ProfileTable = new ProfileTable(m_ProfileTreeViewState, multiColumnHeader, m_ProfileSingleView.analysis, this);

            if (string.IsNullOrEmpty(m_SelectedMarkerName))
                SelectFirstMarkerInTable();
            else
                SelectMarkerByName(m_SelectedMarkerName);

            GetThreadNames(m_ProfileSingleView.data, out m_ThreadUINames, out m_ThreadNames);
            UpdateThreadGroupSelection(m_ThreadNames, m_ThreadSelection);
        }

        private void AnalyzeSync()
        {
            if (m_ProfileSingleView.data==null)
                return;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            
            List<string> threadUINamesNew;
            List<string> threadNamesNew;

            GetThreadNames(m_ProfileSingleView.data, out threadUINamesNew, out threadNamesNew);
            List<string> threadSelection = GetLimitedThreadSelection(threadNamesNew, m_ThreadSelection);

            int fullPhase = (m_FullAnalysisRequired && (m_ProfileSingleView.selectedIndices.Count != m_ProfileSingleView.data.GetFrameCount())) ? 1 : 0;
            m_ThreadPhases = 1 + fullPhase;

            bool selfTimes = IsSelfTime();

            m_ThreadPhase = 0;
            if (fullPhase == 1)
            {
                List<int> selection = new List<int>();
                for (int frameOffset = 0; frameOffset < m_ProfileSingleView.data.GetFrameCount(); frameOffset++)
                {
                    selection.Add(m_ProfileSingleView.data.OffsetToDisplayFrame(frameOffset));
                }

                m_ProfileSingleView.analysisFullNew = m_ProfileAnalyzer.Analyze(m_ProfileSingleView.data, selection, threadSelection, m_DepthFilter, selfTimes, m_ParentMarker);
                m_ThreadPhase++;
            }
            m_ProfileSingleView.analysisNew = m_ProfileAnalyzer.Analyze(m_ProfileSingleView.data, m_ProfileSingleView.selectedIndices, threadSelection, m_DepthFilter, selfTimes, m_ParentMarker);
            m_ThreadPhase++;
            stopwatch.Stop();
            m_LastAnalysisTimeMilliseconds = stopwatch.ElapsedMilliseconds;

            TimeSpan ts = stopwatch.Elapsed;
            if (ts.Minutes > 0)
                m_LastAnalysisTime = string.Format("Last analysis time {0} mins {1} secs {2} ms ", ts.Minutes, ts.Seconds, ts.Milliseconds);
            else if (ts.Seconds > 0)
                m_LastAnalysisTime = string.Format("Last analysis time {0} secs {1} ms ", ts.Seconds, ts.Milliseconds);
            else 
                m_LastAnalysisTime = string.Format("Last analysis time {0} ms ", ts.Milliseconds);
        }

        private void Analyze()
        {
            if (m_EnableAnalysisProfiling)
            {
                m_AnalyzeInUpdatePhase = 1;
                return;
            }

            if (m_Async)
            {
                m_ShowFilters = true;   // To see loading bar

                //m_profileTable = null;
                //m_ProfileSingleView.analysis = null;
                BeginAsyncAction(ThreadActivity.Analyze);
            }
            else
            {
                AnalyzeSync();
                UpdateAnalysisFromAsyncProcessing(m_ProfileSingleView, m_FullAnalysisRequired);
                m_FullAnalysisRequired = false;
            }
        }

        private void GetThreadNames(ProfileData profleData, out List<string> threadUINames, out List<string> threadFilters)
        {
            GetThreadNames(profleData, null, out threadUINames, out threadFilters);
        }

        private string GetFriendlyThreadName(string threadNameWithIndex, bool single)
        {
            var info = threadNameWithIndex.Split(':');
            int threadGroupIndex = int.Parse(info[0]);
            var threadName = info[1].Trim();

            if (single) // Single instance of this thread name
            {
                return threadName;
            }
            else
            {
                // The original format was "Worker 0"
                // The internal formatting is 1:Worker (1+original value).
                // Hence the -1 here
                return string.Format("{0} {1}", threadName, threadGroupIndex - 1);
            }
        }

        private int CompareUINames(string a, string b)
        {
            string[] aTokens = a.Split(':');
            string[] bTokens = b.Split(':');

            if (aTokens.Length > 1 && bTokens.Length > 1)
            {
                var aThreadName = aTokens[0].Trim();
                var bThreadName = bTokens[0].Trim();

                if (aThreadName == bThreadName)
                {
                    string aThreadIndex = aTokens[1].Trim();
                    string bThreadIndex = bTokens[1].Trim();

                    if (aThreadIndex == "All" && bThreadIndex != "All")
                        return -1;
                    if (aThreadIndex != "All" && bThreadIndex == "All")
                        return 1;

                    int aGroupIndex;
                    if (int.TryParse(aThreadIndex, out aGroupIndex))
                    {
                        int bGroupIndex;
                        if (int.TryParse(bThreadIndex, out bGroupIndex))
                        {
                            return aGroupIndex.CompareTo(bGroupIndex);
                        }
                    }
                }
            }

            return a.CompareTo(b);
        }

        private void GetThreadNames(ProfileData leftData, ProfileData rightData, out List<string> threadUINames, out List<string> threadFilters)
        {
            threadUINames = new List<string>();
            threadFilters = new List<string>();

            List<string> threadNames = (leftData != null) ? leftData.GetThreadNames() : new List<string>();
            if (rightData != null)
            {
                foreach (var threadNameWithIndex in rightData.GetThreadNames())
                {
                    if (!threadNames.Contains(threadNameWithIndex))
                    {
                        // TODO: Insert after last thread with same name (or at end)
                        threadNames.Add(threadNameWithIndex);
                    }
                }
            }

            Dictionary<string, string> threadNamesDict = new Dictionary<string, string>();
            for (int index = 0; index < threadNames.Count; index++)
            {
                var threadNameWithIndex = threadNames[index];
                var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);

                if (threadIdentifier.index == 1)
                {
                    if (threadNames.Contains(string.Format("2:{0}", threadIdentifier.name)))
                    {
                        var threadGroupIdentifier = new ThreadIdentifier(threadIdentifier);
                        threadGroupIdentifier.SetAll();

                        // First thread name of a group with the same name
                        // Add an 'all' selection
                        threadNamesDict[string.Format("{0} : All", threadIdentifier.name)] = threadGroupIdentifier.threadNameWithIndex;
                        // And add the first item too
                        threadNamesDict[GetFriendlyThreadName(threadNameWithIndex, false)] = threadNameWithIndex;
                    }
                    else
                    {
                        // Single instance of this thread name
                        threadNamesDict[GetFriendlyThreadName(threadNameWithIndex, true)] = threadNameWithIndex;
                    }
                }
                else
                {
                    threadNamesDict[GetFriendlyThreadName(threadNameWithIndex, false)] = threadNameWithIndex;
                }
            }

            List<string> uiNames = new List<string>();
            foreach (string uiName in threadNamesDict.Keys)
                uiNames.Add(uiName);

            uiNames.Sort(CompareUINames);


            var allThreadIdentifier = new ThreadIdentifier();
            allThreadIdentifier.SetName("All");
            allThreadIdentifier.SetAll();
            threadUINames.Add(allThreadIdentifier.name);
            threadFilters.Add(allThreadIdentifier.threadNameWithIndex);

            foreach (string uiName in uiNames)
            {
                // Strip off the group name
                // Note we don't do this in GetFriendlyThreadName else we would collapse the same named threads (in different groups) in the dict
                string groupName;
                string threadName = ProfileData.GetThreadNameWithoutGroup(uiName, out groupName);
                threadUINames.Add(threadName);
                threadFilters.Add(threadNamesDict[uiName]);
            }
        }

        private void UpdateThreadGroupSelection(List<string> threadNames, ThreadSelection threadSelection)
        {
            // Make sure all members of active groups are present
            foreach (string threadGroupNameWithIndex in threadSelection.groups)
            {
                var threadGroupIdentifier = new ThreadIdentifier(threadGroupNameWithIndex);
                if (threadGroupIdentifier.name == "All" && threadGroupIdentifier.index == ThreadIdentifier.kAll)
                {
                    foreach (string threadNameWithIndex in threadNames)
                    {
                        var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                        if (threadIdentifier.index != ThreadIdentifier.kAll)
                        {
                            if (!threadSelection.selection.Contains(threadNameWithIndex))
                                threadSelection.selection.Add(threadNameWithIndex);
                        }
                    }
                }
                else
                {
                    foreach (string threadNameWithIndex in threadNames)
                    {
                        var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                        if (threadIdentifier.name == threadGroupIdentifier.name &&
                            threadIdentifier.index != ThreadIdentifier.kAll)
                        {
                            if (!threadSelection.selection.Contains(threadNameWithIndex))
                                threadSelection.selection.Add(threadNameWithIndex);
                        }
                    }
                }
            }
        }

        private List<string> GetLimitedThreadSelection(List<string> threadNames, ThreadSelection threadSelection)
        {
            List<string> limitedThreadSelection = new List<string>();
            if (threadSelection.selection == null)
                return limitedThreadSelection;

            foreach (string threadNameWithIndex in threadSelection.selection)
            {
                if (threadNames.Contains(threadNameWithIndex))
                    limitedThreadSelection.Add(threadNameWithIndex);
            }

            // Make sure all members of active groups are present
            foreach (string threadGroupNameWithIndex in threadSelection.groups)
            {
                var threadGroupIdentifier = new ThreadIdentifier(threadGroupNameWithIndex);
                if (threadGroupIdentifier.name == "All" && threadGroupIdentifier.index == ThreadIdentifier.kAll)
                {
                    foreach (string threadNameWithIndex in threadNames)
                    {
                        var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                        if (threadIdentifier.index != ThreadIdentifier.kAll)
                        {
                            if (!limitedThreadSelection.Contains(threadNameWithIndex))
                                limitedThreadSelection.Add(threadNameWithIndex);
                        }
                    }
                }
                else
                {
                    foreach (string threadNameWithIndex in threadNames)
                    {
                        var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                        if (threadIdentifier.name == threadGroupIdentifier.name &&
                            threadIdentifier.index != ThreadIdentifier.kAll)
                        {
                            if (!limitedThreadSelection.Contains(threadNameWithIndex))
                                limitedThreadSelection.Add(threadNameWithIndex);
                        }
                    }
                }
            }

            return limitedThreadSelection;
        }


        private int ClampToRange(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;

            return value;
        }

        private void DrawProfilerWindowPullControls()
        {
            int minFrameindex = 1;
            int maxFrameindex = 1;
#if UNITY_2017_1_OR_NEWER
            if (ProfilerDriver.enabled)
#endif
            {
                minFrameindex = 1 + ProfilerDriver.firstFrameIndex;
                maxFrameindex = 1 + ProfilerDriver.lastFrameIndex;
            }

            EditorGUILayout.LabelField("Range : ", GUILayout.Width(50));
            string pullFirstFrameString = EditorGUILayout.DelayedTextField(m_PullFirstFrameindex.ToString(), GUILayout.Width(50));
            EditorGUILayout.LabelField(" : ", GUILayout.Width(20));
            var pullLastFrameString = EditorGUILayout.DelayedTextField(m_PullLastFrameindex.ToString(), GUILayout.Width(50));
            if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false), GUILayout.Width(50)))
            {
                m_PullFirstFrameindex = minFrameindex;
                m_PullLastFrameindex = maxFrameindex;
            }
            else
            {
                Int32.TryParse(pullFirstFrameString, out m_PullFirstFrameindex);
                m_PullFirstFrameindex = ClampToRange(m_PullFirstFrameindex, minFrameindex, maxFrameindex);
                Int32.TryParse(pullLastFrameString, out m_PullLastFrameindex);
                m_PullLastFrameindex = ClampToRange(m_PullLastFrameindex, minFrameindex, maxFrameindex);
            }
        }

        private void SetRange(List<int> selectedOffsets, int clickCount, bool singleControlAction, FrameTimeGraph.State inputStatus)
        {
            if (inputStatus == FrameTimeGraph.State.Dragging)
                return;

            if (clickCount == 2)
            {
                if (selectedOffsets.Count > 0)
                    JumpToFrame(m_ProfileSingleView.data.OffsetToDisplayFrame(selectedOffsets[0]), false);
            }
            else
            {
                m_ProfileSingleView.selectedIndices.Clear();
                foreach (int offset in selectedOffsets)
                {
                    m_ProfileSingleView.selectedIndices.Add(m_ProfileSingleView.data.OffsetToDisplayFrame(offset));
                }

                m_RequestAnalysis = true;
            }
        }

        public void SelectAllFrames()
        {
            if (m_ActiveTab == ActiveTab.Summary)
            {
                m_ProfileSingleView.SelectFullRange();

                m_RequestAnalysis = true;
            }
            if (m_ActiveTab == ActiveTab.Compare)
            {
                m_ProfileLeftView.SelectFullRange();
                m_ProfileRightView.SelectFullRange();

                m_RequestCompare = true;
            }
        }

        public void SelectFramesContainingMarker(string markerName, bool inSelection)
        {
            if (m_ActiveTab == ActiveTab.Summary)
            {
                if (m_ProfileSingleView.SelectAllFramesContainingMarker(markerName, inSelection))
                {
                    m_RequestAnalysis = true;
                }
            }
            if (m_ActiveTab == ActiveTab.Compare)
            {
                if (m_ProfileLeftView.SelectAllFramesContainingMarker(markerName, inSelection))
                {
                    m_RequestCompare = true;
                }
                if (m_ProfileRightView.SelectAllFramesContainingMarker(markerName, inSelection))
                {
                    m_RequestCompare = true;
                }
            }
        }

        static private List<string> GetNameFilters(string nameFilter)
        {
            List<string> nameFilters = new List<string>();
            if (string.IsNullOrEmpty(nameFilter))
                return nameFilters;

            // Get all quoted strings, without the quotes
            Regex quotedStringWithoutQuotes = new Regex("\"([^\"]*)\"");
            MatchCollection matches = quotedStringWithoutQuotes.Matches(nameFilter);
            foreach (Match match in matches)
            {
                var theData = match.Groups[1].Value;
                nameFilters.Add(theData);
            }

            // Get a new string with the quoted strings removed
            Regex quotedString = new Regex("(\"[^\"]*\")");
            string remaining = quotedString.Replace(nameFilter, "");

            // Get all the remaining strings (that are space separated)
            Regex stringWithoutWhiteSpace = new Regex("([^ \t]+)");
            matches = stringWithoutWhiteSpace.Matches(remaining);
            foreach (Match match in matches)
            {
                string theData = match.Groups[1].Value;
                nameFilters.Add(theData);
            }

            return nameFilters;
        }

        public List<string> GetNameFilters()
        {
            return GetNameFilters(m_NameFilter);
        }

        public List<string> GetNameExcludes()
        {
            return GetNameFilters(m_NameExclude);
        }

        public bool NameInFilterList(string name, List<string> nameFilters)
        {
            switch (m_NameFilterOperation)
            {
                default:
                //case NameFilterOperation.All:
                    return NameInAllFilterList(name, nameFilters);
                case NameFilterOperation.Any:
                    return NameInAnyFilterList(name, nameFilters);
            }
        }

        public bool NameInExcludeList(string name, List<string> nameFilters)
        {
            switch (m_NameExcludeOperation)
            {
                default:
                //case NameFilterOperation.All:
                    return NameInAllFilterList(name, nameFilters);
                case NameFilterOperation.Any:
                    return NameInAnyFilterList(name, nameFilters);
            }
        }

        private bool NameInAllFilterList(string name, List<string> nameFilters)
        {
            foreach (string subString in nameFilters)
            {
                // As soon as name doesn't match one in the list then return false
                if (name.IndexOf(subString, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            // Name is matching all the filters in the list
            return true;
        }

        private bool NameInAnyFilterList(string name, List<string> nameFilters)
        {
            foreach (string subString in nameFilters)
            {
                // As soon as names matches one in the list then return true
                if (name.IndexOf(subString, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        private static bool AddFilter(List<string> nameFilters, ref string filter, string markerName)
        {
            bool justAdded = false;

            string quotedMarkerName = markerName.Contains(" ") ? string.Format("\"{0}\"", markerName) : markerName;
            if (!nameFilters.Contains(quotedMarkerName))
            {
                if (string.IsNullOrEmpty(filter))
                    filter = quotedMarkerName;
                else
                    filter = string.Format("{0} {1}", filter, quotedMarkerName);

                justAdded = true;
            }

            return justAdded;
        }

        public void AddToIncludeFilter(string markerName)
        {
            if (AddFilter(GetNameFilters(), ref m_NameFilter, markerName))
                UpdateMarkerTable();

            // Remove from exclude list if in the include list
            RemoveFromExcludeFilter(markerName);
        }

        public void AddToExcludeFilter(string markerName)
        {
            if (AddFilter(GetNameExcludes(), ref m_NameExclude, markerName))
                UpdateMarkerTable();

            // Remove from include list if in the include list
            RemoveFromIncludeFilter(markerName);
        }

        public void RemoveFromIncludeFilter(string markerName)
        {
            List<string> nameFilters = GetNameFilters();
            if (nameFilters.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();
            bool updated = false;

            foreach (string filter in nameFilters)
            {
                if (filter != markerName)
                    sb.Append(filter);
                else
                    updated = true;
            }

            if (updated)
            {
                m_NameFilter = sb.ToString();

                UpdateMarkerTable();
            }
        }

        public void RemoveFromExcludeFilter(string markerName)
        {
            List<string> nameFilters = GetNameExcludes();
            if (nameFilters.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();
            bool updated = false;

            foreach (string filter in nameFilters)
            {
                if (filter != markerName)
                    sb.Append(filter);
                else
                    updated = true;
            }

            if (updated)
            {
                m_NameExclude = sb.ToString();

                UpdateMarkerTable();
            }
        }

        public void SetAsParentMarkerFilter(string markerName)
        {
            if (markerName != m_ParentMarker)
            {
                m_ParentMarker = markerName;
                UpdateActiveTab(true);
            }
        }

        private float GetFilenameWidth(string path)
        {
            if (path == null)
                return 0f;

            string filename = System.IO.Path.GetFileName(path);
            GUIContent content = new GUIContent(filename, path);
            Vector2 size = GUI.skin.label.CalcSize(content);
            return size.x;
        }

        private void ShowFilename(string path)
        {
            if (path != null)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                GUIContent content = new GUIContent(filename, path);
                Vector2 size = GUI.skin.label.CalcSize(content);
                float width = Math.Min(size.x, 200f);
                EditorGUILayout.LabelField(content, GUILayout.MaxWidth(width));
            }
        }

        private void DrawLoadSave()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300), GUILayout.ExpandWidth(false));

            GUIStyle buttonStyle = GUI.skin.button;
            bool lastEnabled = GUI.enabled;
            GUI.enabled = !IsAnalysisRunning();
            if (GUILayout.Button("Load", buttonStyle, GUILayout.ExpandWidth(false), GUILayout.Width(50)))
                Load();
            if (GUILayout.Button("Save", buttonStyle, GUILayout.ExpandWidth(false), GUILayout.Width(50)))
                Save();
            GUI.enabled = lastEnabled;

            ShowFilename(m_ProfileSingleView.path);
            EditorGUILayout.EndHorizontal();
        }

        private void ShowSelectedMarker()
        {
            DrawSelectedText(m_SelectedMarkerName);
        }

        private void DrawFrameTimeGraph(float height)
        {
            Profiler.BeginSample("DrawFrameTimeGraph");

            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));

            if (m_ProfileSingleView.data != null)
            {
                if (!m_FrameTimeGraph.HasData())
                    m_FrameTimeGraph.SetData(GetFrameTimeData(m_ProfileSingleView.data));
                if (!m_ProfileSingleView.HasValidSelection())
                    m_ProfileSingleView.SelectFullRange();


                List<int> selectedOffsets = new List<int>();
                foreach (int index in m_ProfileSingleView.selectedIndices)
                {
                    selectedOffsets.Add(m_ProfileSingleView.data.DisplayFrameToOffset(index));
                }

                float yRange = m_FrameTimeGraph.GetDataRange();
                int displayOffset = m_ProfileSingleView.data.OffsetToDisplayFrame(0);

                bool enabled = !IsAnalysisRunning();
                m_FrameTimeGraph.SetEnabled(enabled);
                m_FrameTimeGraph.Draw(rect, m_ProfileSingleView.analysis, selectedOffsets, yRange, displayOffset, m_SelectedMarkerName, 0, m_ProfileSingleView.analysisFull);

                FrameTimeGraph.State inputStatus = m_FrameTimeGraph.ProcessInput(rect, selectedOffsets);
                switch (inputStatus)
                {
                    case FrameTimeGraph.State.Dragging:
                        m_RequestRepaint = true;
                        break;
                    case FrameTimeGraph.State.DragComplete:
                        m_RequestAnalysis = true;
                        break;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ShowSelectedMarker();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUI.Label(rect,Styles.dataMissing);
            }

            Profiler.EndSample();
        }

        private void SetDepthStrings(int maxDepth, out string[] strings, out int[] values)
        {
            List<string> depthStrings = new List<string>();
            List<int> depthValues = new List<int>();
            depthStrings.Add("All");
            depthValues.Add(-1);
            // Depth 0 is not used
            for (int depth = 1; depth <= maxDepth; depth++)
            {
                depthStrings.Add(depth.ToString());
                depthValues.Add(depth);
            }
            strings = depthStrings.ToArray();
            values = depthValues.ToArray();
        }

        private void DrawDepthFilter()
        {
            if (!IsAnalysisRunning())
            {
                if (m_ActiveTab == ActiveTab.Summary)
                {
                    int maxDepth = (m_ProfileSingleView.analysis == null) ? 1 : m_ProfileSingleView.analysis.GetFrameSummary().maxMarkerDepth;
                    if (m_DepthStrings==null || maxDepth != (1 + m_DepthStrings.Length))
                        SetDepthStrings(maxDepth, out m_DepthStrings, out m_DepthValues);
                }
                else
                {
                    int maxDepth = (m_ProfileLeftView.analysis == null) ? 1 : m_ProfileLeftView.analysis.GetFrameSummary().maxMarkerDepth;
                    if (m_DepthStrings1 == null || maxDepth != (1 + m_DepthStrings1.Length))
                        SetDepthStrings(maxDepth, out m_DepthStrings1, out m_DepthValues1);

                    maxDepth = (m_ProfileRightView.analysis == null) ? 1 : m_ProfileRightView.analysis.GetFrameSummary().maxMarkerDepth;
                    if (m_DepthStrings2 == null || maxDepth != (1 + m_DepthStrings2.Length))
                        SetDepthStrings(maxDepth, out m_DepthStrings2, out m_DepthValues2);
                }
            }

            bool triggerRefresh = false;

            bool lastEnabled = GUI.enabled;
            bool enabled = !IsAnalysisRunning();

            EditorGUILayout.BeginHorizontal();// GUILayout.MaxWidth(300));
            if (m_ActiveTab == ActiveTab.Summary)
            {
                if (m_DepthStrings != null)
                {
                    EditorGUILayout.LabelField(Styles.depthTitle, GUILayout.Width(LayoutSize.FilterOptionsLeftLabelWidth));
                    int lastDepthFilter = m_DepthFilter;
                    if (m_DepthFilter >= m_DepthStrings.Length)
                        m_DepthFilter = -1;

                    GUI.enabled = enabled;
                    m_DepthFilter = EditorGUILayout.IntPopup(m_DepthFilter, m_DepthStrings, m_DepthValues, GUILayout.Width(LayoutSize.FilterOptionsEnumWidth));
                    GUI.enabled = lastEnabled;
                    if (m_DepthFilter != lastDepthFilter)
                        triggerRefresh = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField(Styles.depthTitle, GUILayout.Width(LayoutSize.FilterOptionsLeftLabelWidth));

                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200));
                if (m_DepthStrings1 != null)
                {
                    EditorGUILayout.LabelField(Styles.leftDepthTitle, GUILayout.Width(LayoutSize.FilterOptionsEnumWidth));
                    int lastDepthFilter1 = m_DepthFilter1;
                    if (m_DepthFilter1 >= m_DepthStrings1.Length)
                        m_DepthFilter1 = -1;

                    GUI.enabled = enabled;
                    m_DepthFilter1 = EditorGUILayout.IntPopup(m_DepthFilter1, m_DepthStrings1, m_DepthValues1, GUILayout.Width(50));
                    GUI.enabled = lastEnabled;
                    if (m_DepthFilter1 != lastDepthFilter1)
                        triggerRefresh = true;
                }

                if (m_DepthStrings2 != null)
                {
                    int lastDepthFilter2 = m_DepthFilter2;

                    EditorGUILayout.LabelField(Styles.rightDepthTitle, GUILayout.Width(45));
                    if (m_DepthFilter2 >= m_DepthStrings2.Length)
                        m_DepthFilter2 = -1;
                    GUI.enabled = enabled && !m_DepthFilter2Auto;
                    m_DepthFilter2 = EditorGUILayout.IntPopup(m_DepthFilter2, m_DepthStrings2, m_DepthValues2, GUILayout.Width(50));
                    GUI.enabled = lastEnabled;
                    if (m_DepthFilter2 != lastDepthFilter2)
                        triggerRefresh = true;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                bool lastDepthFilter2Auto = m_DepthFilter2Auto;
                GUI.enabled = enabled;
                var content = new GUIContent(string.Format("Auto Right (+{0})", m_DepthDiff));
                m_DepthFilter2Auto = EditorGUILayout.ToggleLeft(content, m_DepthFilter2Auto);
                GUI.enabled = lastEnabled;
                if (m_DepthFilter2Auto != lastDepthFilter2Auto)
                    triggerRefresh = true;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            if (triggerRefresh)
            {
                UpdateDepthFilters();
                UpdateActiveTab(true);
            }
        }

        private void DrawParentFilter()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.parentMarker, GUILayout.Width(100));

            if (!string.IsNullOrEmpty(m_ParentMarker))
            {
                bool lastEnabled = GUI.enabled;
                bool enabled = !IsAnalysisRunning();
                GUI.enabled = enabled;
                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.MaxWidth(LayoutSize.FilterOptionsEnumWidth)))
                {
                    SetAsParentMarkerFilter("");
                }
                GUI.enabled = lastEnabled;

                DrawSelectedText(m_ParentMarker);
            }
            else
            {
                EditorGUILayout.LabelField(Styles.selectParentMarker);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void UpdateAutoDepthFilter()
        {
            if (m_DepthFilter2Auto)
            {
                m_DepthFilter2 = m_DepthFilter1 == -1 ? -1 : m_DepthFilter1 + m_DepthDiff;
            }
        }

        private void UpdateDepthFilters()
        {
            if (m_ActiveTab == ActiveTab.Compare)
            {
                // First respect the auto flag
                UpdateAutoDepthFilter();


                // Make sure Single matches the updated comparison view
                if (m_ProfileLeftView.path == m_ProfileSingleView.path)
                {
                    // Use same filter on single view if its the same file
                    m_DepthFilter = m_DepthFilter1;
                }
                if (m_ProfileRightView.path == m_ProfileSingleView.path)
                {
                    // Use same filter on single view if its the same file
                    m_DepthFilter = m_DepthFilter2;
                }
            }
            else
            {
                // Make sure comparisons match updated single view

                if (m_ProfileLeftView.path == m_ProfileSingleView.path)
                {
                    // Use same filter on comparison left view if its the same file
                    m_DepthFilter1 = m_DepthFilter;
                    UpdateAutoDepthFilter();
                }

                if (m_ProfileRightView.path == m_ProfileSingleView.path)
                {
                    // Use same filter on comparison right view if its the same file
                    if (m_DepthFilter2Auto)
                    {
                        // When auto selected we have to set the left filter to new depth - diff
                        m_DepthFilter1 = m_DepthFilter == -1 ? -1 : m_DepthFilter - m_DepthDiff;
                        // Although it this is not a valid depth, then set it to all
                        if (m_DepthFilter1 <= 0)
                            m_DepthFilter1 = -1;

                        UpdateAutoDepthFilter();
                    }
                    else
                    {
                        m_DepthFilter2 = m_DepthFilter;
                    }
                }
            }
        }

        public void SetThreadSelection(ThreadSelection threadSelection)
        {
            m_ThreadSelectionNew = new ThreadSelection(threadSelection);

            UpdateActiveTab(true);
        }

        string GetSelectedThreadsSummary()
        {
            if (m_ThreadSelection.selection == null || m_ThreadSelection.selection.Count == 0)
                return "None";

            // Count all threads in a group
            var threadDict = new Dictionary<string, int>();
            var threadSelectionDict = new Dictionary<string, int>();
            foreach (var threadNameWithIndex in m_ThreadNames)
            {
                var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                if (threadIdentifier.index == ThreadIdentifier.kAll)
                    continue;

                int count;
                if (threadDict.TryGetValue(threadIdentifier.name, out count))
                    threadDict[threadIdentifier.name] = count + 1;
                else
                    threadDict[threadIdentifier.name] = 1;

                threadSelectionDict[threadIdentifier.name] = 0;
            }

            // Count all the threads we have 'selected' in a group
            foreach (var threadNameWithIndex in m_ThreadSelection.selection)
            {
                var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);

                if (threadDict.ContainsKey(threadIdentifier.name) &&
                    threadSelectionDict.ContainsKey(threadIdentifier.name) &&
                    threadIdentifier.index <= threadDict[threadIdentifier.name])
                {
                    // Selected thread valid and in the thread list
                    // and also within the range of valid threads for this data set
                    threadSelectionDict[threadIdentifier.name]++;
                }
            }

            // Count all thread groups where we have 'selected all the threads'
            int threadsSelected = 0;
            foreach (var threadName in threadDict.Keys)
            {
                if (threadSelectionDict[threadName] != threadDict[threadName])
                    continue;

                threadsSelected++;
            }
            
            // If we've just added all the thread names we have everything selected
            // Note we don't compare against the m_ThreadNames directly as this contains the 'all' versions
            if (threadsSelected == threadDict.Keys.Count)
                return "All";

            // Add all the individual threads were we haven't already added the group
            List<string> threads = new List<string>();
            foreach (var threadName in threadSelectionDict.Keys)
            {
                int selectionCount = threadSelectionDict[threadName];
                if (selectionCount <= 0)
                    continue;
                int threadCount = threadDict[threadName];
                if (threadCount == 1)
                    threads.Add(threadName);
                else if (selectionCount != threadCount)
                    threads.Add(string.Format("{0} ({1} of {2})", threadName, selectionCount, threadCount));
                else
                    threads.Add(string.Format("{0} (All)", threadName));
            }

            // Maintain alphabetical order
            threads.Sort(CompareUINames);

            if (threads.Count == 0)
                return "None";

            string threadsSelectedText = string.Join(", ", threads.ToArray());
            return threadsSelectedText;
        }

        private void DrawThreadFilter(ProfileData profileData)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.threadFilter, GUILayout.Width(LayoutSize.FilterOptionsLeftLabelWidth));
            if (profileData != null)
            {
                if (m_ThreadNames.Count > 0)
                {
                    bool lastEnabled = GUI.enabled;
                    bool enabled = !IsAnalysisRunning() && !ThreadSelectionWindow.IsOpen();
                    GUI.enabled = enabled;
                    if (GUILayout.Button(Styles.threadFilterSelect, EditorStyles.miniButton, GUILayout.Width(LayoutSize.FilterOptionsEnumWidth)))
                    {
                        // Note: Window auto closes as it loses focus so this isn't strictly required
                        if (ThreadSelectionWindow.IsOpen())
                        {
                            ThreadSelectionWindow.CloseAll();
                        }
                        else
                        {
                            Vector2 windowPosition = new Vector2(Event.current.mousePosition.x + LayoutSize.FilterOptionsEnumWidth, Event.current.mousePosition.y + GUI.skin.label.lineHeight);
                            Vector2 screenPosition = GUIUtility.GUIToScreenPoint(windowPosition);

                            ThreadSelectionWindow.Open(screenPosition.x, screenPosition.y, this, m_ThreadSelection, m_ThreadNames, m_ThreadUINames);
                        }
                    }

                    GUI.enabled = lastEnabled;
                    ShowSelectedThreads();
                    GUILayout.FlexibleSpace();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSelectedText(string text)
        {
#if UNITY_2019_1_OR_NEWER
            GUIStyle treeViewSelectionStyle = "TV Selection";
            GUIStyle backgroundStyle = new GUIStyle(treeViewSelectionStyle);

            GUIStyle treeViewLineStyle = "TV Line";
            GUIStyle textStyle = new GUIStyle(treeViewLineStyle);
#else
            GUIStyle textStyle = GUI.skin.label;
#endif

            GUIContent content = new GUIContent(text, text);
            Vector2 size = textStyle.CalcSize(content);
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.MaxWidth(size.x), GUILayout.Height(size.y));
            if (Event.current.type == EventType.Repaint)
            {
#if UNITY_2019_1_OR_NEWER
                backgroundStyle.Draw(rect, false, false, true, true);
#endif
                GUI.Label(rect, content, textStyle);
            }
        }

        private void ShowSelectedThreads()
        {
            string threadsSelected = GetSelectedThreadsSummary();

            DrawSelectedText(threadsSelected);
        }

        private void DrawUnitFilter()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(LayoutSize.FilterOptionsRightLabelWidth + LayoutSize.FilterOptionsRightEnumWidth));
            EditorGUILayout.LabelField(Styles.unitFilter, m_StyleMiddleRight, GUILayout.Width(LayoutSize.FilterOptionsRightEnumWidth));

            bool lastEnabled = GUI.enabled;
            bool enabled = !IsAnalysisRunning();
            GUI.enabled = enabled;
            //Units units = (Units)EditorGUILayout.EnumPopup(m_DisplayUnits.Units, GUILayout.Width(LayoutSize.FilterOptionsRightEnumWidth));
            Units units = (Units)EditorGUILayout.Popup((int)m_DisplayUnits.Units, m_UnitNames, GUILayout.Width(LayoutSize.FilterOptionsRightEnumWidth));

            GUI.enabled = lastEnabled;
            if (units != m_DisplayUnits.Units)
            {
                SetUnits(units);
                m_FrameTimeGraph.SetUnits(m_DisplayUnits.Units);
                m_LeftFrameTimeGraph.SetUnits(m_DisplayUnits.Units);
                m_RightFrameTimeGraph.SetUnits(m_DisplayUnits.Units);
                UpdateMarkerTable();
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool IsSelfTime()
        {
            return (m_TimingOption == TimingOptions.TimingOption.Self) ? true : false;
        }

        private void DrawTimingFilter()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(LayoutSize.FilterOptionsRightLabelWidth + LayoutSize.FilterOptionsRightEnumWidth));

            EditorGUILayout.LabelField(Styles.timingFilter, m_StyleMiddleRight, GUILayout.Width(LayoutSize.FilterOptionsRightLabelWidth));

            bool lastEnabled = GUI.enabled;
            bool enabled = !IsAnalysisRunning();
            GUI.enabled = enabled;
            var timingOption = (TimingOptions.TimingOption)EditorGUILayout.Popup((int)m_TimingOption, TimingOptions.TimingOptionNames, GUILayout.Width(LayoutSize.FilterOptionsRightEnumWidth));
            GUI.enabled = lastEnabled;
            if (timingOption != m_TimingOption)
            {
                m_TimingOption = timingOption;
                UpdateActiveTab(true, true);
            }
            EditorGUILayout.EndHorizontal();
        }

        public string GetDisplayUnits()
        {
            return m_DisplayUnits.Postfix();
        }

        public string ToDisplayUnits(float ms, bool showUnits = false, int limitToDigits = 5)
        {
            return m_DisplayUnits.ToString(ms, showUnits, limitToDigits);
        }

        public string ToDisplayUnits(double ms, bool showUnits = false, int limitToDigits = 5)
        {
            return m_DisplayUnits.ToString((float)ms, showUnits, limitToDigits);
        }

        public void SetUnits(Units units)
        {
            m_DisplayUnits = new DisplayUnits(units);
        }

        private void UpdateActiveTab(bool fullAnalysisRequired = false, bool markOtherDirty = true)
        {
            switch (m_ActiveTab)
            {
                case ActiveTab.Summary:
                    m_RequestAnalysis = true;
                    m_FullAnalysisRequired = fullAnalysisRequired;
                    break;
                case ActiveTab.Compare:
                    m_RequestCompare = true;
                    m_FullCompareRequired = fullAnalysisRequired;
                    break;
            }

            if (markOtherDirty)
                m_OtherTabDirty = true;
        }

        private void UpdateMarkerTable(bool markOtherDirty = true)
        {
            switch (m_ActiveTab)
            {
                case ActiveTab.Summary:
                    if (m_ProfileTable!=null)
                        m_ProfileTable.Reload();
                    break;
                case ActiveTab.Compare:
                    if (m_ComparisonTable!=null)
                        m_ComparisonTable.Reload();
                    break;
            }

            if (markOtherDirty)
                m_OtherTableDirty = true;
        }

        private void DrawNameFilter()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.nameFilter, GUILayout.Width(LayoutSize.FilterOptionsLeftLabelWidth));

            NameFilterOperation lastNameFilterOperation = m_NameFilterOperation;

            bool lastEnabled = GUI.enabled;
            bool enabled = !IsAnalysisRunning();
            GUI.enabled = enabled;
            m_NameFilterOperation = (NameFilterOperation)EditorGUILayout.Popup((int)m_NameFilterOperation, Styles.nameFilterOperation, GUILayout.MaxWidth(LayoutSize.FilterOptionsEnumWidth));
            GUI.enabled = lastEnabled;

            if (m_NameFilterOperation != lastNameFilterOperation)
            {
                UpdateMarkerTable();
            }
            string lastFilter = m_NameFilter;
            GUI.enabled = enabled;
            m_NameFilter = EditorGUILayout.DelayedTextField(m_NameFilter, GUILayout.MinWidth(200 - LayoutSize.FilterOptionsEnumWidth));
            GUI.enabled = lastEnabled;
            if (m_NameFilter != lastFilter)
            {
                UpdateMarkerTable();
            }

            EditorGUILayout.LabelField(Styles.nameExclude, GUILayout.Width(LayoutSize.FilterOptionsLeftLabelWidth));
            NameFilterOperation lastNameExcludeOperation = m_NameExcludeOperation;
            GUI.enabled = enabled;
            m_NameExcludeOperation = (NameFilterOperation)EditorGUILayout.Popup((int)m_NameExcludeOperation, Styles.nameFilterOperation, GUILayout.MaxWidth(LayoutSize.FilterOptionsEnumWidth));
            GUI.enabled = lastEnabled;
            if (m_NameExcludeOperation != lastNameExcludeOperation)
            {
                UpdateMarkerTable();
            }
            string lastExclude = m_NameExclude;
            GUI.enabled = enabled;
            m_NameExclude = EditorGUILayout.DelayedTextField(m_NameExclude, GUILayout.MinWidth(200 - LayoutSize.FilterOptionsEnumWidth));
            GUI.enabled = lastEnabled;
            if (m_NameExclude != lastExclude)
            {
                UpdateMarkerTable();
            }
            EditorGUILayout.EndHorizontal();
        }

        public void SetMode(MarkerColumnFilter.Mode newMode)
        {
            m_SingleModeFilter.mode = newMode;
            m_CompareModeFilter.mode = newMode;

            if (m_ProfileTable!=null)
                m_ProfileTable.SetMode(m_SingleModeFilter);
            if (m_ComparisonTable!=null)
                m_ComparisonTable.SetMode(m_CompareModeFilter);
        }

        public void SetSingleModeColumns(int[] visibleColumns)
        {
            // If selecting the columns manually then override the currently stored selection with the current
            m_ProfileMulticolumnHeaderState.visibleColumns = visibleColumns;

            m_SingleModeFilter.mode = MarkerColumnFilter.Mode.Custom;
            m_SingleModeFilter.visibleColumns = visibleColumns;

            // Also move other view to the custom set
            m_CompareModeFilter.mode = MarkerColumnFilter.Mode.Custom;
        }
        public void SetComparisonModeColumns(int[] visibleColumns)
        {
            // If selecting the columns manually then override the currently stored selection with the current
            m_ComparisonMulticolumnHeaderState.visibleColumns = visibleColumns;

            m_CompareModeFilter.mode = MarkerColumnFilter.Mode.Custom;
            m_CompareModeFilter.visibleColumns = visibleColumns;

            // Also move other view to the custom set
            m_SingleModeFilter.mode = MarkerColumnFilter.Mode.Custom;
        }

        private void DrawMarkerColumnFilter()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(LayoutSize.FilterOptionsRightLabelWidth + LayoutSize.FilterOptionsRightEnumWidth));
            EditorGUILayout.LabelField(Styles.markerColumns, m_StyleMiddleRight, GUILayout.Width(LayoutSize.FilterOptionsRightLabelWidth));

            bool lastEnabled = GUI.enabled;
            bool enabled = !IsAnalysisRunning();
            GUI.enabled = enabled;
            MarkerColumnFilter.Mode newMode = (MarkerColumnFilter.Mode)EditorGUILayout.IntPopup((int)m_SingleModeFilter.mode, MarkerColumnFilter.ModeNames, MarkerColumnFilter.ModeValues, GUILayout.Width(LayoutSize.FilterOptionsRightEnumWidth));
            GUI.enabled = lastEnabled;
            if (newMode != m_SingleModeFilter.mode)
            {
                SetMode(newMode);
            }

            EditorGUILayout.EndHorizontal();
        }

        private int GetCombinedThreadCount()
        {
            var threads = new Dictionary<string, int>();
            foreach (var threadName in m_ProfileLeftView.data.GetThreadNames())
            {
                threads[threadName] = 1;
            }
            foreach (var threadName in m_ProfileRightView.data.GetThreadNames())
            {
                if (threads.ContainsKey(threadName))
                    threads[threadName] += 1;
                else
                    threads[threadName] = 1;
            }

            return threads.Keys.Count;
        }

        private void DrawMarkerCount()
        {
            if (!IsAnalysisValid())
                return;

            if (m_ActiveTab == ActiveTab.Summary)
            {
                int markersCount = m_ProfileSingleView.analysis.GetFrameSummary().totalMarkers;
                int filteredMarkersCount = (m_ProfileTable != null) ? m_ProfileTable.GetRows().Count : 0;
                
                var content = new GUIContent(
                    String.Format("{0} of {1} markers", filteredMarkersCount, markersCount),
                    "Number of markers in the filtered set, compared to the total in the data set");
                Vector2 size = GUI.skin.label.CalcSize(content);
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(size.x), GUILayout.Height(size.y));
                EditorGUI.LabelField(rect, content);
            }
            if (m_ActiveTab == ActiveTab.Compare)
            {
                int markersCount = m_Pairings.Count;
                int filteredMarkersCount = (m_ComparisonTable != null) ? m_ComparisonTable.GetRows().Count : 0;
                
                var content = new GUIContent(
                    String.Format("{0} of {1} markers", filteredMarkersCount, markersCount),
                    "Number of markers in the filtered set, compared to total unique markers in the combined data sets");
                Vector2 size = GUI.skin.label.CalcSize(content);
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(size.x), GUILayout.Height(size.y));
                EditorGUI.LabelField(rect, content);
            }
        }

        private void DrawThreadCount()
        {
            if (!IsAnalysisValid())
                return;

            if (m_ActiveTab == ActiveTab.Summary)
            {
                int allThreadsCount = m_ProfileSingleView.data.GetThreadNames().Count;
                List<string> threadSelection = GetLimitedThreadSelection(m_ThreadNames, m_ThreadSelection);
                int selectedThreads = threadSelection.Count;

                var content = new GUIContent(
                    String.Format("{0} of {1} threads", selectedThreads, allThreadsCount),
                    "Number of threads in the filtered set, compared to the total in the data set");
                Vector2 size = GUI.skin.label.CalcSize(content);
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(size.x), GUILayout.Height(size.y));
                EditorGUI.LabelField(rect, content);
            }
            if (m_ActiveTab == ActiveTab.Compare)
            {
                int allThreadsCount = GetCombinedThreadCount();
                List<string> threadSelection = GetLimitedThreadSelection(m_ThreadNames, m_ThreadSelection);
                int selectedThreads = threadSelection.Count;

                var content = new GUIContent(
                    String.Format("{0} of {1} threads", selectedThreads, allThreadsCount),
                    "Number of threads in the filtered set, compared to total unique threads in the combined data sets");
                Vector2 size = GUI.skin.label.CalcSize(content);
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(size.x), GUILayout.Height(size.y));
                EditorGUI.LabelField(rect, content);
            }
        }

        private void DrawAnalysisOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            bool lastShowFilters = m_ShowFilters;
            m_ShowFilters = BoldFoldout(m_ShowFilters, Styles.filters);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowFilters)
            {
                DrawNameFilter();
                EditorGUILayout.BeginHorizontal();
                DrawThreadFilter(m_ProfileSingleView.data);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                DrawDepthFilter();
                DrawTimingFilter();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                DrawParentFilter();
                DrawUnitFilter();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Analyze", m_LastAnalysisTime), GUILayout.Width(100)))
                    m_RequestAnalysis = true;
                DrawMarkerCount();
                EditorGUILayout.LabelField(",", GUILayout.Width(10), GUILayout.ExpandWidth(false));
                DrawThreadCount();
                DrawProgress();
                GUILayout.FlexibleSpace();
                DrawMarkerColumnFilter();
                EditorGUILayout.EndHorizontal();
            }

            if (m_ShowFilters != lastShowFilters)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Filters, analytic.GetDurationInSeconds(), m_ShowFilters);
            }

            EditorGUILayout.EndVertical();
        }

        private bool IsAnalysisRunning()
        {
            if (m_ThreadActivity != ThreadActivity.None)
                return true;

            return false;
        }

        private int GetProgress()
        {
            // We return the value from the update loop so the data doesn't change over the time onGui is called for layout and repaint
            return m_ThreadProgress;
        }

        private bool IsAnalysisValid()
        {
            switch (m_ActiveTab)
            {
                case ActiveTab.Summary:
                    if (m_ProfileSingleView.data == null)
                        return false;

                    if (m_ProfileSingleView.analysis == null)
                        return false;

                    if (m_ProfileSingleView.analysis.GetFrameSummary().frames.Count <= 0)
                        return false;
                    break;

                case ActiveTab.Compare:
                    if (m_ProfileLeftView.data == null)
                        return false;
                    if (m_ProfileRightView.data == null)
                        return false;

                    if (m_ProfileLeftView.analysis == null)
                        return false;
                    if (m_ProfileRightView.analysis == null)
                        return false;

                    if (m_ProfileLeftView.analysis.GetFrameSummary().frames.Count <= 0)
                        return false;
                    if (m_ProfileRightView.analysis.GetFrameSummary().frames.Count <= 0)
                        return false;
                    break;
            }

            //if (IsAnalysisRunning())
            //    return false;

            return true;
        }

        void DrawProgress()
        {
            if (IsAnalysisRunning())
            {
                int progress = GetProgress();

                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(140));
                float x = 0;
                float y = 0;
                float width = 100;
                float height = GUI.skin.label.lineHeight;
                if (m_2D.DrawStart(width, height, Draw2D.Origin.TopLeft, GUI.skin.label))
                {
                    float barLength = (width * progress) / 100;
                    m_2D.DrawFilledBox(x, y, barLength, height, m_ColorWhite);
                      
                    m_2D.DrawEnd();
                }
                EditorGUILayout.LabelField(string.Format("{0}%", progress), GUILayout.MaxWidth(40));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("", GUILayout.Width(140));
            }
        }

        private void DrawPullButton(Color color, ProfileDataView view, FrameTimeGraph frameTimeGraph)
        {
            bool lastEnabled = GUI.enabled;
            GUI.enabled = !IsAnalysisRunning();

            GUIContent content;
            if (!IsProfilerWindowOpen())
            {
                content = Styles.pullOpen;
                GUI.enabled = false;
            }
            else if (m_PullFirstFrameindex == 0 && m_PullLastFrameindex == 0)
            {
                content = Styles.pullRange;
                GUI.enabled = false;
            }
            else
            {
                content = Styles.pull;
            }

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            bool pull  = GUILayout.Button(content, GUILayout.Width(100));
            GUI.backgroundColor = oldColor;
            if (pull)
            {
                PullFromProfiler(m_PullFirstFrameindex, m_PullLastFrameindex, view, frameTimeGraph);
                UpdateActiveTab(true,false);
            }
            GUI.enabled = lastEnabled;
        }

        private void DrawFilesLoaded()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (m_ActiveTab == ActiveTab.Summary)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(100 + GUI.skin.label.lineHeight + (2 * (GUI.skin.label.margin.vertical + GUI.skin.label.padding.vertical))));

                float filenameWidth = GetFilenameWidth(m_ProfileSingleView.path);
                filenameWidth = Math.Min(filenameWidth, 200);
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(100 + filenameWidth), GUILayout.ExpandWidth(false));
                DrawPullButton(GUI.backgroundColor, m_ProfileSingleView, m_FrameTimeGraph);
                DrawLoadSave();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                DrawFrameTimeGraph(100);
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            if (m_ActiveTab == ActiveTab.Compare)
            {
                DrawComparisonLoadSave();
            }

            EditorGUILayout.EndVertical();
        }

        private void ShowHelp()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            m_HelpScroll = EditorGUILayout.BeginScrollView(m_HelpScroll, GUILayout.ExpandHeight(true));
#if UNITY_2019_3_OR_NEWER
            GUILayout.TextArea(Styles.helpText);
#else
            GUIStyle helpStyle = new GUIStyle(EditorStyles.textField);
            helpStyle.wordWrap = true;
            EditorGUILayout.LabelField(Styles.helpText, helpStyle);
#endif
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private void DrawAnalysis()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            DrawFilesLoaded();

            if (m_ProfileSingleView.data != null && m_ProfileSingleView.data.GetFrameCount()>0)
            {
                DrawAnalysisOptions();
            }
 
            if (IsAnalysisValid())
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                string title = string.Format("Top {0} markers on median frame", m_TopNBars);
                GUIContent markersTitle = new GUIContent(title, Styles.topMarkersTooltip);
                bool lastShowTopMarkers = m_ShowTopNMarkers;
                m_ShowTopNMarkers = BoldFoldout(m_ShowTopNMarkers, markersTitle);
                var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                if (m_ShowTopNMarkers)
                {

                    EditorGUILayout.BeginVertical(GUILayout.Height(20));
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    var topMarkers = new TopMarkers(this, this.m_2D, m_ColorBarBackground, m_ColorTextTopMarkers);
                    float range = topMarkers.GetTopMarkerTimeRange(m_ProfileSingleView.analysis, m_TopNBars, m_DepthFilter);
                    topMarkers.Draw(m_ProfileSingleView.analysis, rect, m_ColorBar, m_TopNBars, range, m_DepthFilter, m_ColorBarBackground, Color.black, Color.white, true, true);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(GUILayout.Height(20));
                    GUIContent info;
                    if (m_DepthFilter >= 0)
                        info = new GUIContent(string.Format("(Top markers from median frame, depth filtered to level {0} only)", m_DepthFilter), Styles.medianFrameTooltip);
                    else
                        info = new GUIContent("(Top markers from median frame, all depths)", string.Format("{0}\n\nSet depth 1 to get an overview of the frame", Styles.medianFrameTooltip));
                    GUILayout.Label(info);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("");
                    EditorGUILayout.EndHorizontal();
                }

                if (m_ShowTopNMarkers != lastShowTopMarkers)
                {
                    ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.TopTen, analytic.GetDurationInSeconds(), m_ShowTopNMarkers);
                }

                EditorGUILayout.EndVertical();

                if (m_ProfileTable != null)
                {
                    m_ShowMarkerTable = BoldFoldout(m_ShowMarkerTable, Styles.profileTable);
                    if (m_ShowMarkerTable)
                    {
                        Profiler.BeginSample("DrawMarkerTable");
                        Rect r = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
                        m_ProfileTable.OnGUI(r);
                        Profiler.EndSample();
                    }
                }
            }

            if (m_ProfileTable == null)
                ShowHelp();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(LayoutSize.WidthRHS));
            GUILayout.Space(4);
            DrawFrameSummary();
            DrawThreadSummary();
            DrawSelected();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void SetRange(List<int> selectedOffsets, int clickCount, bool singleControlAction, FrameTimeGraph.State inputStatus, ProfileData mainData, List<int> selectedIndices)
        {
            if (inputStatus == FrameTimeGraph.State.Dragging)
                return;

            if (clickCount == 2)
            {
                int index = mainData.OffsetToDisplayFrame(selectedOffsets[0]);
                JumpToFrame(index, false);
            }
            else
            {
                selectedIndices.Clear();
                foreach (int offset in selectedOffsets)
                {
                    selectedIndices.Add(mainData.OffsetToDisplayFrame(offset));
                }

                m_RequestCompare = true;
            }
        }

        private void SetLeftRange(List<int> selectedOffsets, int clickCount, bool singleControlAction, FrameTimeGraph.State inputStatus)
        {
            SetRange(selectedOffsets, clickCount, singleControlAction, inputStatus, m_ProfileLeftView.data, m_ProfileLeftView.selectedIndices);
        }

        private void SetRightRange(List<int> selectedOffsets, int clickCount, bool singleControlAction, FrameTimeGraph.State inputStatus)
        {
            SetRange(selectedOffsets, clickCount, singleControlAction, inputStatus, m_ProfileRightView.data, m_ProfileRightView.selectedIndices);
        }

        private void DrawComparisonLoadSaveButton(Color color, ProfileDataView view, FrameTimeGraph frameTimeGraph)
        {
            bool lastEnabled = GUI.enabled;
            bool enabled = !IsAnalysisRunning();

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300), GUILayout.ExpandWidth(false));

            GUIStyle buttonStyle = GUI.skin.button;
            Color oldColor = GUI.backgroundColor;

            GUI.backgroundColor = color;
            GUI.enabled = enabled;
            bool load = GUILayout.Button("Load", buttonStyle, GUILayout.ExpandWidth(false), GUILayout.Width(50));
            GUI.enabled = lastEnabled;
            GUI.backgroundColor = oldColor;
            if (load)
            {
                string newPath = EditorUtility.OpenFilePanel("Load profile analyzer data file", "", "pdata");
                if (newPath.Length != 0)
                {
                    ProfileData newData;
                    if (ProfileData.Load(newPath, out newData))
                    {
                        SetView(view, newData, newPath, frameTimeGraph);

                        // Remove pairing if both left/right point at the same data
                        if (m_ProfileLeftView.path == m_ProfileRightView.path)
                        {
                            SetFrameTimeGraphPairing(false);
                        }

                        m_FullCompareRequired = true;
                        m_RequestCompare = true;
                    }
                }
            }

            GUI.backgroundColor = color;
            GUI.enabled = enabled;
            bool save = GUILayout.Button("Save", buttonStyle, GUILayout.ExpandWidth(false), GUILayout.Width(50));
            GUI.enabled = lastEnabled;
            GUI.backgroundColor = oldColor;
            if (save)
            {
                string newPath = EditorUtility.SaveFilePanel("Save profile analyzer data file", "", "capture.pdata", "pdata");
                if (newPath.Length != 0)
                {
                    view.path = newPath;
                    if (ProfileData.Save(newPath, view.data))
                    {
                        UpdateMatchingProfileData(view.data, ref view.path, view.analysis, newPath);
                    }
                }
            }

            ShowFilename(view.path);

            EditorGUILayout.EndHorizontal();
        }

        private float GetComparisonYRange()
        {
            float yRangeLeft = m_ProfileLeftView.data != null ? m_LeftFrameTimeGraph.GetDataRange() : 0f;
            float yRangeRight = m_ProfileRightView.data != null ? m_RightFrameTimeGraph.GetDataRange() : 0f;
            float yRange = Math.Max(yRangeLeft, yRangeRight);

            return yRange;
        }

        private void SetFrameTimeGraphPairing(bool paired)
        {
            if (paired != m_FrameTimeGraphsPaired)
            {
                m_FrameTimeGraphsPaired = paired;
                m_LeftFrameTimeGraph.PairWith(m_FrameTimeGraphsPaired ? m_RightFrameTimeGraph : null);
            }
        }

        private void DrawComparisonLoadSave()
        {
            GUIStyle buttonStyle = GUI.skin.button;

            Color oldColor = GUI.backgroundColor;

            int leftFrames = m_ProfileLeftView.data != null ? m_ProfileLeftView.data.GetFrameCount() : 0;
            int rightFrames = m_ProfileRightView.data != null ? m_ProfileRightView.data.GetFrameCount() : 0;
            int maxFrames = Math.Max(leftFrames, rightFrames);

            float yRange = GetComparisonYRange();

            EditorGUILayout.BeginHorizontal(GUILayout.Height(100 + GUI.skin.label.lineHeight + (2 * (GUI.skin.label.margin.vertical + GUI.skin.label.padding.vertical))));

            float leftFilenameWidth = GetFilenameWidth(m_ProfileLeftView.path);
            float rightFilenameWidth = GetFilenameWidth(m_ProfileRightView.path);
            float filenameWidth = Math.Max(leftFilenameWidth, rightFilenameWidth);
            filenameWidth = Math.Min(filenameWidth, 200);

            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(100 + filenameWidth), GUILayout.ExpandWidth(false));
            DrawPullButton(m_ColorLeft, m_ProfileLeftView, m_LeftFrameTimeGraph);
            DrawComparisonLoadSaveButton(m_ColorLeft, m_ProfileLeftView, m_LeftFrameTimeGraph);
            DrawPullButton(m_ColorRight, m_ProfileRightView, m_RightFrameTimeGraph);
            DrawComparisonLoadSaveButton(m_ColorRight, m_ProfileRightView, m_RightFrameTimeGraph);
            EditorGUILayout.EndVertical();


            bool lastEnabled = GUI.enabled;
            bool enabled = !IsAnalysisRunning();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            FrameTimeGraph.State inputStatus = FrameTimeGraph.State.None;

            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(50));
            if (m_ProfileLeftView.data != null)
            {
                if (!m_LeftFrameTimeGraph.HasData())
                    m_LeftFrameTimeGraph.SetData(GetFrameTimeData(m_ProfileLeftView.data));
                if (!m_ProfileLeftView.HasValidSelection())
                    m_ProfileLeftView.SelectFullRange();

                List<int> selectedOffsets = new List<int>();
                foreach (int index in m_ProfileLeftView.selectedIndices)
                {
                    selectedOffsets.Add(m_ProfileLeftView.data.DisplayFrameToOffset(index));
                }

                int displayOffset = m_ProfileLeftView.data.OffsetToDisplayFrame(0);
                m_LeftFrameTimeGraph.SetEnabled(enabled);
                m_LeftFrameTimeGraph.Draw(rect, m_ProfileLeftView.analysis, selectedOffsets, yRange, displayOffset, m_SelectedMarkerName, maxFrames, m_ProfileLeftView.analysisFull);

                if (inputStatus == FrameTimeGraph.State.None)
                    inputStatus = m_LeftFrameTimeGraph.ProcessInput(rect, selectedOffsets, maxFrames);
            }
            else
            {
                GUI.Label(rect,Styles.comparisonDataMissing);
            }

            rect = EditorGUILayout.GetControlRect(GUILayout.Height(50));
            if (m_ProfileRightView.data != null)
            {
                if (!m_RightFrameTimeGraph.HasData())
                    m_RightFrameTimeGraph.SetData(GetFrameTimeData(m_ProfileRightView.data));
                if (!m_ProfileRightView.HasValidSelection())
                    m_ProfileRightView.SelectFullRange();

                List<int> selectedOffsets = new List<int>();
                foreach (int index in m_ProfileRightView.selectedIndices)
                {
                    selectedOffsets.Add(m_ProfileRightView.data.DisplayFrameToOffset(index));
                }

                int displayOffset = m_ProfileRightView.data.OffsetToDisplayFrame(0);
                m_RightFrameTimeGraph.SetEnabled(enabled);
                m_RightFrameTimeGraph.Draw(rect, m_ProfileRightView.analysis, selectedOffsets, yRange, displayOffset, m_SelectedMarkerName, maxFrames, m_ProfileRightView.analysisFull);

                if (inputStatus == FrameTimeGraph.State.None)
                    inputStatus = m_RightFrameTimeGraph.ProcessInput(rect, selectedOffsets, maxFrames);
            }
            else
            {
                GUI.Label(rect, Styles.comparisonDataMissing);
            }

            EditorGUILayout.BeginHorizontal();
            if (m_ProfileLeftView.data != null && m_ProfileRightView.data != null && m_ProfileLeftView.data.GetFrameCount()>0 && m_ProfileRightView.data.GetFrameCount()>0)
            {
                GUIStyle lockButtonStyle = "IN LockButton";
                GUIStyle style = new GUIStyle(lockButtonStyle);
                style.padding.left = 20;
                //bool paired = GUILayout.Toggle(m_frameTimeGraphsPaired, Styles.graphPairing, style);
                GUI.enabled = enabled;
                bool paired = EditorGUILayout.ToggleLeft(Styles.graphPairing, m_FrameTimeGraphsPaired, style, GUILayout.MaxWidth(200));
                GUI.enabled = lastEnabled;
                SetFrameTimeGraphPairing(paired);
            }
            GUILayout.FlexibleSpace();


            ShowSelectedMarker();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


            switch (inputStatus)
            {
                case FrameTimeGraph.State.Dragging:
                    m_RequestRepaint = true;
                    break;
                case FrameTimeGraph.State.DragComplete:
                    m_RequestCompare = true;
                    break;
            }
        }

        private void DrawComparisonFrameSummary()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LayoutSize.WidthRHS));

            bool lastShowFrameSummary = m_ShowFrameSummary;
            m_ShowFrameSummary = BoldFoldout(m_ShowFrameSummary, Styles.frameSummary);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowFrameSummary)
            {
                if (IsComparisonValid())
                {
                    var leftFrameSummary = m_ProfileLeftView.analysis.GetFrameSummary();
                    var rightFrameSummary = m_ProfileRightView.analysis.GetFrameSummary();

                    m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);
                    m_Columns.Draw4("", "Left", "Right", "Diff");

                    int diff = rightFrameSummary.count - leftFrameSummary.count;
                    m_Columns.Draw4("Frame Count", leftFrameSummary.count.ToString(), rightFrameSummary.count.ToString(), diff.ToString());
                    m_Columns.Draw3("Frame Range", GetFrameRangeText(m_ProfileLeftView.analysis), GetFrameRangeText(m_ProfileRightView.analysis));

                    m_Columns.Draw(0, "");
                    string units = GetDisplayUnits();
                    m_Columns.Draw4("", units, units, units);

                    Draw4DiffMs(Styles.max, leftFrameSummary.msMax, leftFrameSummary.maxFrameIndex, rightFrameSummary.msMax, rightFrameSummary.maxFrameIndex);
                    Draw4DiffMs(Styles.upperQuartile, leftFrameSummary.msUpperQuartile, rightFrameSummary.msUpperQuartile);
                    Draw4DiffMs(Styles.median, leftFrameSummary.msMedian, leftFrameSummary.medianFrameIndex, rightFrameSummary.msMedian, rightFrameSummary.medianFrameIndex);
                    Draw4DiffMs(Styles.mean, leftFrameSummary.msMean, rightFrameSummary.msMean);
                    Draw4DiffMs(Styles.lowerQuartile, leftFrameSummary.msLowerQuartile, rightFrameSummary.msLowerQuartile);
                    Draw4DiffMs(Styles.min, leftFrameSummary.msMin, leftFrameSummary.minFrameIndex, rightFrameSummary.msMin, rightFrameSummary.minFrameIndex);

                    GUIStyle style = GUI.skin.label;
                    GUILayout.Space(style.lineHeight);

                    EditorGUILayout.BeginHorizontal();
                    int leftBucketCount = leftFrameSummary.buckets.Length;
                    int rightBucketCount = rightFrameSummary.buckets.Length;

                    float msFrameMax = Math.Max(leftFrameSummary.msMax, rightFrameSummary.msMax);
                    float yRange = msFrameMax;

                    if (leftBucketCount != rightBucketCount)
                    {
                        Debug.Log("Error left frame summary bucket count doesn't equal right summary");
                    }
                    else
                    {
                        Histogram histogram = new Histogram(m_2D,m_DisplayUnits.Units);
                        float width = LayoutSize.HistogramWidth;
                        float height = 40;
                        float min = 0;
                        float max = yRange;
                        float spacing = 2;

                        int bucketCount = leftBucketCount;
                        float x = (spacing / 2);
                        float y = 0;
                        float w = ((width + spacing) / bucketCount) - spacing;
                        float h = height;

                        histogram.DrawStart(width);

                        if (m_2D.DrawStart(width, height, Draw2D.Origin.BottomLeft))
                        {
                            float bucketWidth = ((max - min) / bucketCount);
                            Rect rect = GUILayoutUtility.GetLastRect();

                            histogram.DrawBackground(width, height, bucketCount, spacing);

                            if (!IsAnalysisRunning())
                            {
                                for (int bucketAt = 0; bucketAt < bucketCount; bucketAt++)
                                {
                                    int leftBarCount = leftFrameSummary.buckets[bucketAt];
                                    int rightBarCount = rightFrameSummary.buckets[bucketAt];
                                    float leftBarHeight = (h * leftBarCount) / leftFrameSummary.count;
                                    float rightBarHeight = (h * rightBarCount) / rightFrameSummary.count;

                                    if ((int)rightBarHeight == (int)leftBarHeight)
                                    {
                                        m_2D.DrawFilledBox(x, y, w, leftBarHeight, m_ColorBoth);
                                    }
                                    else if (rightBarHeight > leftBarHeight)
                                    {
                                        m_2D.DrawFilledBox(x, y, w, rightBarHeight, m_ColorRight);
                                        m_2D.DrawFilledBox(x, y, w, leftBarHeight, m_ColorBoth);
                                    }
                                    else
                                    {
                                        m_2D.DrawFilledBox(x, y, w, leftBarHeight, m_ColorLeft);
                                        m_2D.DrawFilledBox(x, y, w, rightBarHeight, m_ColorBoth);
                                    }

                                    float bucketStart = min + (bucketAt * bucketWidth);
                                    float bucketEnd = bucketStart + bucketWidth;
                                    GUI.Label(new Rect(rect.x + x, rect.y + y, w, h),
                                              new GUIContent("", string.Format("{0}-{1}\nLeft: {2} frames\nRight: {3} frames", ToDisplayUnits(bucketStart), ToDisplayUnits(bucketEnd, true), leftBarCount, rightBarCount))
                                             );

                                    x += w;
                                    x += spacing;
                                }
                            }

                            m_2D.DrawEnd();
                        }

                        histogram.DrawEnd(width, min, max, spacing);
                    }

                    BoxAndWhiskerPlot boxAndWhiskerPlot = new BoxAndWhiskerPlot(m_2D, m_DisplayUnits.Units);

                    float plotWidth = 40 + GUI.skin.box.padding.horizontal;
                    float plotHeight = 40;
                    plotWidth /= 2.0f;
                    boxAndWhiskerPlot.Draw(plotWidth, plotHeight, leftFrameSummary.msMin, leftFrameSummary.msLowerQuartile,
                        leftFrameSummary.msMedian, leftFrameSummary.msUpperQuartile, leftFrameSummary.msMax, 0, yRange,
                        m_ColorBoxAndWhiskerLineColorLeft, m_ColorBoxAndWhiskerBoxColorLeft);
                    boxAndWhiskerPlot.Draw(plotWidth, plotHeight, rightFrameSummary.msMin, rightFrameSummary.msLowerQuartile,
                        rightFrameSummary.msMedian, rightFrameSummary.msUpperQuartile, rightFrameSummary.msMax, 0, yRange, 
                        m_ColorBoxAndWhiskerLineColorRight, m_ColorBoxAndWhiskerBoxColorRight);

                    boxAndWhiskerPlot.DrawText(m_Columns.GetColumnWidth(3), plotHeight, 0, yRange, 
                        "Min frame time for selected frames in the 2 data sets", 
                        "Max frame time for selected frames in the 2 data sets");

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField("No analysis data selected");
                }
            }

            if (m_ShowFrameSummary != lastShowFrameSummary)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Frames, analytic.GetDurationInSeconds(), m_ShowFrameSummary);
            }

            EditorGUILayout.EndVertical();
        }

        private void ShowThreadRange()
        {
            EditorGUILayout.BeginHorizontal();
            m_Columns.Draw(0, "Graph Scale : ");
            m_ThreadRange = (ThreadRange)EditorGUILayout.Popup((int)m_ThreadRange, m_ThreadRanges, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        private float GetThreadTimeRange(ProfileAnalysis profileAnalysis)
        {
            if (profileAnalysis==null)
                return 0.0f;

            var frameSummary = profileAnalysis.GetFrameSummary();
            float range = frameSummary.msMax;
            switch (m_ThreadRange)
            {
                case ThreadRange.Median:
                    range = frameSummary.msMedian;
                    break;
                case ThreadRange.UpperQuartile:
                    range = frameSummary.msUpperQuartile;
                    break;
                case ThreadRange.Max:
                    range = frameSummary.msMax;
                    break;
            }

            return range;
        }

        private void DrawComparisonThreadSummary()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LayoutSize.WidthRHS));

            bool lastShowThreadSummary = m_ShowThreadSummary;
            m_ShowThreadSummary = BoldFoldout(m_ShowThreadSummary, Styles.threadSummary);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowThreadSummary)
            {
                if (IsAnalysisValid())
                {
                    m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2 + LayoutSize.WidthColumn3, 0);
                    ShowThreadRange();

                    float width = 100;
                    float height = GUI.skin.label.lineHeight;

                    float xAxisMin = 0.0f;
                    float xAxisMax = GetThreadTimeRange(m_ProfileLeftView.analysis);

                    m_Columns.Draw3(Styles.emptyString, Styles.median, Styles.thread);

                    m_ThreadScroll = EditorGUILayout.BeginScrollView(m_ThreadScroll, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(200));
                    Rect clipRect = new Rect(m_ThreadScroll.x, m_ThreadScroll.y, 400, 200);
                    m_2D.SetClipRect(clipRect);
                    for (int i = 0; i < m_ThreadUINames.Count; i++)
                    {
                        string threadNameWithIndex = m_ThreadNames[i];

                        bool include = ProfileAnalyzer.MatchThreadFilter(threadNameWithIndex, m_ThreadSelection.selection);
                        if (!include)
                            continue;
                            
                        ThreadData threadLeft = m_ProfileLeftView.analysis.GetThreadByName(threadNameWithIndex);
                        ThreadData threadRight = m_ProfileRightView.analysis.GetThreadByName(threadNameWithIndex);

                        ThreadData thread = threadLeft != null ? threadLeft : threadRight;
                        if (thread == null)
                            continue;
                        
                        bool singleThread = thread.threadsInGroup > 1 ? false : true;

                        BoxAndWhiskerPlot boxAndWhiskerPlot = new BoxAndWhiskerPlot(m_2D, m_DisplayUnits.Units);
                        EditorGUILayout.BeginHorizontal();
                        if (threadLeft != null)
                            boxAndWhiskerPlot.DrawHorizontal(width, height, threadLeft.msMin, threadLeft.msLowerQuartile, threadLeft.msMedian, threadLeft.msUpperQuartile, threadLeft.msMax, xAxisMin, xAxisMax, m_ColorBoxAndWhiskerLineColorLeft, m_ColorBoxAndWhiskerBoxColorLeft, GUI.skin.label);
                        else
                            EditorGUILayout.LabelField("", GUILayout.Width(width));
                        float left = (threadLeft != null) ? threadLeft.msMedian : 0.0f;
                        m_Columns.Draw(1, ToDisplayUnitsWithTooltips(left));
                        m_Columns.Draw(2, GetThreadNameWithGroupTooltip(thread.threadNameWithIndex, singleThread));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        if (threadRight != null)
                            boxAndWhiskerPlot.DrawHorizontal(width, height, threadRight.msMin, threadRight.msLowerQuartile, threadRight.msMedian, threadRight.msUpperQuartile, threadRight.msMax, xAxisMin, xAxisMax, m_ColorBoxAndWhiskerLineColorRight, m_ColorBoxAndWhiskerBoxColorRight, GUI.skin.label);
                        else
                            EditorGUILayout.LabelField("", GUILayout.Width(width));
                        float right = (threadRight != null) ? threadRight.msMedian : 0.0f;
                        m_Columns.Draw(1, ToDisplayUnitsWithTooltips(right));
                        m_Columns.Draw(2, "");
                        EditorGUILayout.EndHorizontal();
                    }
                    m_2D.ClearClipRect();
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("No analysis data selected");
                }
            }

            if (m_ShowThreadSummary != lastShowThreadSummary)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Threads, analytic.GetDurationInSeconds(), m_ShowThreadSummary);
            }

            EditorGUILayout.EndVertical();
        }

        private bool IsComparisonValid()
        {
            if (m_ProfileLeftView.data == null)
                return false;
            if (m_ProfileRightView.data == null)
                return false;

            if (m_ProfileLeftView.analysis == null)
                return false;
            if (m_ProfileRightView.analysis == null)
                return false;

            if (m_ProfileLeftView.analysis.GetFrameSummary().frames.Count <= 0)
                return false;
            if (m_ProfileRightView.analysis.GetFrameSummary().frames.Count <= 0)
                return false;

            //if (IsAnalysisRunning())
            //    return false;

            return true;
        }

        private void DrawCompareOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            bool lastShowFilters = m_ShowFilters;
            m_ShowFilters = BoldFoldout(m_ShowFilters, Styles.filters);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowFilters)
            {
                DrawNameFilter();
                EditorGUILayout.BeginHorizontal();
                DrawThreadFilter(m_ProfileLeftView.data);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                DrawDepthFilter();
                DrawTimingFilter();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                DrawParentFilter();
                DrawUnitFilter();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (m_ProfileLeftView.data != null && m_ProfileRightView.data != null)
                {
                    bool lastEnabled = GUI.enabled;
                    GUI.enabled = !IsAnalysisRunning();
                    if (GUILayout.Button(new GUIContent("Compare", m_LastCompareTime), GUILayout.Width(100)))
                        m_RequestCompare = true;
                    GUI.enabled = lastEnabled;
                }
                DrawMarkerCount();
                EditorGUILayout.LabelField(",", GUILayout.Width(10), GUILayout.ExpandWidth(false));
                DrawThreadCount();
                DrawProgress();
                GUILayout.FlexibleSpace();
                DrawMarkerColumnFilter();
                EditorGUILayout.EndHorizontal();
            }
            if (m_ShowFilters != lastShowFilters)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Filters, analytic.GetDurationInSeconds(), m_ShowFilters);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawComparison()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            DrawFilesLoaded();

            if (m_ProfileLeftView.data!=null && m_ProfileRightView.data!=null && m_ProfileLeftView.data.GetFrameCount()>0 && m_ProfileRightView.data.GetFrameCount()>0)
                DrawCompareOptions();

            if (m_ComparisonTable != null)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                string title = string.Format("Top {0} markers on median frames", m_TopNBars);
                GUIContent markersTitle = new GUIContent(title, Styles.topMarkersTooltip);

                bool lastShowTopMarkers = m_ShowTopNMarkers;
                m_ShowTopNMarkers = BoldFoldout(m_ShowTopNMarkers, markersTitle);
                var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                if (m_ShowTopNMarkers)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Height(40));
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    rect.height = rect.height / 2;
                    var topMarkers = new TopMarkers(this, this.m_2D, m_ColorBarBackground, m_ColorTextTopMarkers);

                    float leftRange = topMarkers.GetTopMarkerTimeRange(m_ProfileLeftView.analysis, m_TopNBars, m_DepthFilter1);
                    float rightRange = topMarkers.GetTopMarkerTimeRange(m_ProfileRightView.analysis, m_TopNBars, m_DepthFilter2);
                    if (m_TopTenDisplay == TopTenDisplay.LongestTime)
                    {
                        float max = Math.Max(leftRange, rightRange);
                        leftRange = max;
                        rightRange = max;
                    }

                    topMarkers.Draw(m_ProfileLeftView.analysis, rect, m_ColorLeft, m_TopNBars, leftRange, m_DepthFilter1, m_ColorBarBackground, Color.black, Color.white, true, true);
                    rect.y += rect.height;
                    topMarkers.Draw(m_ProfileRightView.analysis, rect, m_ColorRight, m_TopNBars, rightRange, m_DepthFilter2, m_ColorBarBackground, Color.black, Color.white, true, true);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginHorizontal();
                    GUIContent info;
                    if (m_DepthFilter1 >= 0 || m_DepthFilter2 >= 0)
                    {
                        if (m_DepthFilter1 != m_DepthFilter2)
                            info = new GUIContent(string.Format("(Top markers from median frame, depth filtered to levels {0} and {1} only)", m_DepthFilter1, m_DepthFilter2), Styles.medianFrameTooltip);
                        else
                            info = new GUIContent(string.Format("(Top markers from median frame, depth filtered to level {0} only)", m_DepthFilter1), Styles.medianFrameTooltip);
                    }
                    else
                    {
                        info = new GUIContent("(Top markers from median frame, all depths)", string.Format("{0}\n\nSet depth 1 to get an overview of the frame", Styles.medianFrameTooltip));
                    }
                    GUILayout.Label(info, GUILayout.ExpandWidth(true));
                    GUILayout.Label(Styles.topMarkerRatio, GUILayout.ExpandWidth(false));
                    m_TopTenDisplay = (TopTenDisplay)EditorGUILayout.Popup((int)m_TopTenDisplay, Styles.topTenDisplayOptions, GUILayout.MaxWidth(100));
                    EditorGUILayout.EndHorizontal();
                }

                if (m_ShowTopNMarkers != lastShowTopMarkers)
                {
                    ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Markers, analytic.GetDurationInSeconds(), m_ShowTopNMarkers);
                }

                EditorGUILayout.EndVertical();

                if (m_ComparisonTable != null)
                {
                    m_ShowMarkerTable = BoldFoldout(m_ShowMarkerTable, Styles.comparisonTable);
                    if (m_ShowMarkerTable)
                    {
                        Profiler.BeginSample("DrawComparisonTable");
                        Rect r = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
                        m_ComparisonTable.OnGUI(r);
                        Profiler.EndSample();
                    }
                }
            }
            else
            {
                ShowHelp();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(LayoutSize.WidthRHS));
            GUILayout.Space(4);
            DrawComparisonFrameSummary();
            DrawComparisonThreadSummary();
            DrawComparisonSelected();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private bool BoldFoldout(bool toggle, GUIContent content)
        {
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            return EditorGUILayout.Foldout(toggle, content, foldoutStyle);
        }

        void DrawComparisonSelectedStats(MarkerData leftMarker, MarkerData rightMarker)
        {
            GUIStyle style = GUI.skin.label;

            string units = GetDisplayUnits();
            m_Columns.Draw4("", units, units, units);
            Draw4DiffMs(Styles.max, MarkerData.GetMsMax(leftMarker), MarkerData.GetMaxFrameIndex(leftMarker), MarkerData.GetMsMax(rightMarker), MarkerData.GetMaxFrameIndex(rightMarker));
            Draw4DiffMs(Styles.upperQuartile, MarkerData.GetMsUpperQuartile(leftMarker), MarkerData.GetMsUpperQuartile(rightMarker));
            Draw4DiffMs(Styles.median, MarkerData.GetMsMedian(leftMarker), MarkerData.GetMedianFrameIndex(leftMarker), MarkerData.GetMsMedian(rightMarker), MarkerData.GetMedianFrameIndex(rightMarker));
            Draw4DiffMs(Styles.mean, MarkerData.GetMsMean(leftMarker), MarkerData.GetMsMean(rightMarker));
            Draw4DiffMs(Styles.lowerQuartile, MarkerData.GetMsLowerQuartile(leftMarker), MarkerData.GetMsLowerQuartile(rightMarker));
            Draw4DiffMs(Styles.min, MarkerData.GetMsMin(leftMarker), MarkerData.GetMinFrameIndex(leftMarker), MarkerData.GetMsMin(rightMarker), MarkerData.GetMinFrameIndex(rightMarker));

            GUILayout.Space(style.lineHeight);

            Draw4DiffMs(Styles.individualMax, MarkerData.GetMsMaxIndividual(leftMarker), MarkerData.GetMsMaxIndividual(rightMarker));
            Draw4DiffMs(Styles.individualMin, MarkerData.GetMsMinIndividual(leftMarker), MarkerData.GetMsMinIndividual(rightMarker));
        }

        void DrawComparisonSelected()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LayoutSize.WidthRHS));

            GUIStyle style = GUI.skin.label;

            bool lastMarkerSummary = m_ShowMarkerSummary;
            m_ShowMarkerSummary = BoldFoldout(m_ShowMarkerSummary, Styles.markerSummary);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowMarkerSummary)
            {
                if (IsComparisonValid())
                {
                    List<MarkerData> leftMarkers = m_ProfileLeftView.analysis.GetMarkers();
                    List<MarkerData> rightMarkers = m_ProfileRightView.analysis.GetMarkers();
                    int pairingAt = m_SelectedPairing;
                    if (leftMarkers != null && rightMarkers != null && m_Pairings!=null && pairingAt >= 0 && pairingAt < m_Pairings.Count)
                    {
                        var pairing = m_Pairings[pairingAt];

                        var leftMarker = (pairing.leftIndex >= 0 && pairing.leftIndex < leftMarkers.Count) ? leftMarkers[pairing.leftIndex] : null;
                        var rightMarker = (pairing.rightIndex >= 0 && pairing.rightIndex < rightMarkers.Count) ? rightMarkers[pairing.rightIndex] : null;

                        EditorGUILayout.LabelField(pairing.name,
                            GUILayout.MaxWidth(LayoutSize.WidthRHS -
                                               (GUI.skin.box.padding.horizontal + GUI.skin.box.margin.horizontal)));
                        DrawComparisonFrameRatio(leftMarker, rightMarker);

                        m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);

                        EditorGUILayout.BeginHorizontal();
                        m_Columns.Draw(0, "First frame");
                        if (leftMarker != null)
                            DrawFrameIndexButton(leftMarker.firstFrameIndex);
                        else
                            m_Columns.Draw(1, "");
                        if (rightMarker != null)
                            DrawFrameIndexButton(rightMarker.firstFrameIndex);
                        else
                            m_Columns.Draw(2, "");
                        EditorGUILayout.EndHorizontal();

                        DrawTopComparison(leftMarker, rightMarker);

                        GUILayout.Space(style.lineHeight);

                        EditorGUILayout.BeginHorizontal();

                        int leftBucketCount = leftMarker != null ? leftMarker.buckets.Length : 0;
                        int rightBucketCount = rightMarker != null ? rightMarker.buckets.Length : 0;

                        float leftMin = MarkerData.GetMsMin(leftMarker);
                        float rightMin = MarkerData.GetMsMin(rightMarker);

                        float leftMax = MarkerData.GetMsMax(leftMarker);
                        float rightMax = MarkerData.GetMsMax(rightMarker);
                        
                        int[] leftBuckets = leftMarker != null ? leftMarker.buckets : new int[0];
                        int[] rightBuckets = rightMarker != null ? rightMarker.buckets : new int[0];

                        Units units = m_DisplayUnits.Units;
                        string unitName = "marker time";
                        if (DisplayCount())
                        {
                            units = Units.Count;
                            unitName = "count";
                            
                            leftBucketCount = leftMarker != null ? leftMarker.countBuckets.Length : 0;
                            rightBucketCount = rightMarker != null ? rightMarker.countBuckets.Length : 0;

                            leftMin = MarkerData.GetCountMin(leftMarker);
                            rightMin = MarkerData.GetCountMin(rightMarker);

                            leftMax = MarkerData.GetCountMax(leftMarker);
                            rightMax = MarkerData.GetCountMax(rightMarker);
                            
                            leftBuckets = leftMarker != null ? leftMarker.countBuckets : new int[0];
                            rightBuckets = rightMarker != null ? rightMarker.countBuckets : new int[0];
                        }

                        DisplayUnits displayUnits = new DisplayUnits(units);
                        
                        float minValue = Math.Min(leftMin, rightMin);
                        float maxValue = Math.Max(leftMax, rightMax);

                        if (leftBucketCount > 0 && rightBucketCount > 0 && leftBucketCount != rightBucketCount)
                        {
                            Debug.Log("Error - number of buckets doesn't match in the left and right marker analysis");
                        }
                        else
                        {
                            Histogram histogram = new Histogram(m_2D, units);
                            float width = LayoutSize.HistogramWidth;
                            float height = 100;
                            float min = minValue;
                            float max = maxValue;
                            float spacing = 2;

                            int bucketCount = Math.Max(leftBucketCount, rightBucketCount);
                            int leftFrameCount = MarkerData.GetPresentOnFrameCount(leftMarker);
                            int rightFrameCount = MarkerData.GetPresentOnFrameCount(rightMarker);
                            float x = (spacing / 2);
                            float y = 0;
                            float w = ((width + spacing) / bucketCount) - spacing;
                            float h = height;

                            histogram.DrawStart(width);

                            if (m_2D.DrawStart(width, height, Draw2D.Origin.BottomLeft))
                            {
                                float bucketWidth = ((max - min) / bucketCount);
                                Rect rect = GUILayoutUtility.GetLastRect();

                                histogram.DrawBackground(width, height, bucketCount, spacing);

                                for (int bucketAt = 0; bucketAt < bucketCount; bucketAt++)
                                {
                                    float leftBarCount = leftMarker != null ? leftBuckets[bucketAt] : 0;
                                    float rightBarCount = rightMarker != null ? rightBuckets[bucketAt] : 0;
                                    float leftBarHeight = leftMarker != null ? ((h * leftBarCount) / leftFrameCount) : 0;
                                    float rightBarHeight = rightMarker != null ? ((h * rightBarCount) / rightFrameCount) : 0;

                                    if ((int)rightBarHeight == (int)leftBarHeight)
                                    {
                                        m_2D.DrawFilledBox(x, y, w, leftBarHeight, m_ColorBoth);
                                    }
                                    else if (rightBarHeight > leftBarHeight)
                                    {
                                        m_2D.DrawFilledBox(x, y, w, rightBarHeight, m_ColorRight);
                                        m_2D.DrawFilledBox(x, y, w, leftBarHeight, m_ColorBoth);
                                    }
                                    else
                                    {
                                        m_2D.DrawFilledBox(x, y, w, leftBarHeight, m_ColorLeft);
                                        m_2D.DrawFilledBox(x, y, w, rightBarHeight, m_ColorBoth);
                                    }

                                    float bucketStart = min + (bucketAt * bucketWidth);
                                    float bucketEnd = bucketStart + bucketWidth;
                                    string tooltip = string.Format("{0}-{1}\nLeft: {2} frames\nRight: {3} frames",
                                        displayUnits.ToString(bucketStart, false, 5), displayUnits.ToString(bucketEnd, true, 5), 
                                        leftBarCount, rightBarCount);
                                    GUI.Label(new Rect(rect.x + x, rect.y + y, w, h),
                                              new GUIContent("", tooltip)
                                             );

                                    x += w;
                                    x += spacing;
                                }

                                m_2D.DrawEnd();
                            }

                            histogram.DrawEnd(width, min, max, spacing);
                        }

                        float plotWidth = 40 + GUI.skin.box.padding.horizontal;
                        float plotHeight = 100;
                        plotWidth /= 2.0f;
                        BoxAndWhiskerPlot boxAndWhiskerPlot = new BoxAndWhiskerPlot(m_2D, units);
                        DrawBoxAndWhiskerPlotForMarker(boxAndWhiskerPlot, plotWidth, plotHeight, m_ProfileLeftView.analysis, leftMarker, minValue, maxValue,
                             m_ColorBoxAndWhiskerLineColorLeft, m_ColorBoxAndWhiskerBoxColorLeft);
                        DrawBoxAndWhiskerPlotForMarker(boxAndWhiskerPlot, plotWidth, plotHeight, m_ProfileRightView.analysis, rightMarker, minValue, maxValue,
                             m_ColorBoxAndWhiskerLineColorRight, m_ColorBoxAndWhiskerBoxColorRight);

                        boxAndWhiskerPlot.DrawText(m_Columns.GetColumnWidth(3), plotHeight, minValue, maxValue,
                            string.Format("Min {0} for selected frames in the 2 data sets", unitName), 
                            string.Format("Max {0} for selected frames in the 2 data sets", unitName));

                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(style.lineHeight);

                        DrawComparisonSelectedStats(leftMarker, rightMarker);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No marker data selected");
                }
            }

            if (m_ShowMarkerSummary != lastMarkerSummary)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Markers, analytic.GetDurationInSeconds(), m_ShowMarkerSummary);
            }

            EditorGUILayout.EndVertical();
        }

        private void SelectTab(ActiveTab newTab)
        {
            m_NextActiveTab = newTab;
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.LabelField("Mode:", EditorStyles.miniLabel, GUILayout.Width(40));
            ActiveTab newTab = (ActiveTab)GUILayout.Toolbar((int)m_ActiveTab, new string[] { "Single", "Compare" }, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            if (newTab != m_ActiveTab)
            {
                SelectTab(newTab);
            }

            //GUILayout.FlexibleSpace();
            EditorGUILayout.Separator();
            bool lastEnabled = GUI.enabled;
            bool enabled = GUI.enabled;
            if (m_ProfileSingleView.data != null || (m_ProfileLeftView.data != null && m_ProfileRightView.data != null))
                GUI.enabled = true;
            else
                GUI.enabled = false;
            if (GUILayout.Button(Styles.export, EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                var window = GetWindow<ProfileAnalyzerExportWindow>("Export");
                window.SetData(m_ProfileSingleView, m_ProfileLeftView, m_ProfileRightView);
                window.minSize = new Vector2(220, 100);
                window.position.size.Set(220, 100);
                window.Show();
            }
            GUI.enabled = lastEnabled;

            bool profilerOpen = IsProfilerWindowOpen();
            if (!profilerOpen)
            {
                if (GUILayout.Toggle(profilerOpen, "Open Profiler Window", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))==true)
                {
                    var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                    m_ProfilerWindowInterface.OpenProfilerOrUseExisting();
                    ProfileAnalyzerAnalytics.SendUIButtonEvent(ProfileAnalyzerAnalytics.UIButton.OpenProfiler, analytic);
                }
            }
            else
            {
                if (GUILayout.Toggle(profilerOpen, "Close Profiler Window", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))==false)
                {
                    var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                    m_ProfilerWindowInterface.CloseProfiler();
                    ProfileAnalyzerAnalytics.SendUIButtonEvent(ProfileAnalyzerAnalytics.UIButton.CloseProfiler, analytic);
                }
            }
            EditorGUILayout.Separator();

            GUILayout.FlexibleSpace();

            /*
            enabled = !IsAnalysisRunning();


            // Analysis type: Self/total
            EditorGUILayout.LabelField(Styles.timingFilter, EditorStyles.miniLabel, GUILayout.Width(80));

            GUI.enabled = enabled;
            var timingOption = (TimingOptions.TimingOption)EditorGUILayout.Popup((int)m_TimingOption, TimingOptions.TimingOptionNames, EditorStyles.toolbarPopup, GUILayout.Width(50));
            GUI.enabled = lastEnabled;
            if (timingOption != m_TimingOption)
            {
                m_TimingOption = timingOption;
                UpdateActiveTab(true, true);
            }

            // Units
            EditorGUILayout.LabelField(Styles.unitFilter, EditorStyles.miniLabel, GUILayout.Width(40));

            GUI.enabled = enabled;
            Units units = (Units)EditorGUILayout.EnumPopup(m_DisplayUnits.Units, EditorStyles.toolbarPopup, GUILayout.Width(90));
            GUI.enabled = lastEnabled;
            if (units != m_DisplayUnits.Units)
            {
                SetUnits(units);
                m_FrameTimeGraph.SetUnits(m_DisplayUnits.Units);
                m_LeftFrameTimeGraph.SetUnits(m_DisplayUnits.Units);
                m_RightFrameTimeGraph.SetUnits(m_DisplayUnits.Units);
            }
            */

            EditorGUILayout.EndHorizontal();
        }

        private void SetupStyles()
        {
            if (!m_StylesSetup)
            {
                m_StyleMiddleRight = GUI.skin.label;
                m_StyleMiddleRight.alignment = TextAnchor.MiddleRight;

                m_StylesSetup = true;
            }
        }

        private void Draw()
        {
            // Make sure we start enabled (in case something overrode it last frame)
            GUI.enabled = true;

            Profiler.BeginSample("ProfileAnalyzer.Draw");
            SetupStyles();

            EditorGUILayout.BeginVertical();

            DrawToolbar();

            switch (m_ActiveTab)
            {
                case ActiveTab.Summary:
                    DrawAnalysis();
                    break;
                case ActiveTab.Compare:
                    DrawComparison();
                    break;
            }

            EditorGUILayout.EndVertical();

            Profiler.EndSample();
        }

        private int FindSelectionByName(List<MarkerData> markers, string name)
        {
            int index = 0;
            foreach (var marker in markers)
            {
                if (marker.name == name)
                    return index;
                index++;
            }
            return -1; // not found
        }

        public void SelectMarker(string name)
        {
            switch (m_ActiveTab)
            {
                case ActiveTab.Summary:
                    SelectMarkerByName(name);
                    break;
                case ActiveTab.Compare:
                    SelectPairingByName(name);
                    break;
            }
        }

        private void UpdateSelectedMarkerName(string markerName)
        {
            m_SelectedMarkerName = markerName;
            if (m_ThreadSelection.selection != null && m_ThreadSelection.selection.Count > 0)
            {
                m_ProfilerWindowInterface.SetProfilerWindowMarkerName(markerName, m_ThreadSelection.selection);
            }
        }

        public void SelectMarker(int index)
        {
            m_SelectedMarker = index;

            if (m_ProfileTable != null)
            {
                List<int> selection = new List<int>();
                if (index >= 0)
                    selection.Add(index);
                m_ProfileTable.SetSelection(selection);
            }

            var markerName = GetMarkerName(index);
            UpdateSelectedMarkerName(markerName);
        }

        public string GetSelectedMarkerName()
        {
            switch (m_ActiveTab)
            {
                case ActiveTab.Summary:
                    return GetMarkerName(m_SelectedMarker);
                case ActiveTab.Compare:
                    return GetPairingName(m_SelectedPairing);
            }

            return null;
        }

        private string GetMarkerName(int index)
        {
            if (m_ProfileSingleView.analysis == null)
                return null;

            var marker = m_ProfileSingleView.analysis.GetMarker(index);
            if (marker==null)
                return null;

            return marker.name;
        }

        private void SelectMarkerByName(string markerName)
        {
            int index = (m_ProfileSingleView.analysis != null) ? m_ProfileSingleView.analysis.GetMarkerIndexByName(markerName) : -1;

            SelectMarker(index);
        }

        public void SelectPairing(int index)
        {
            m_SelectedPairing = index;

            if (m_ComparisonTable != null)
            {
                List<int> selection = new List<int>();
                if (index >= 0)
                    selection.Add(index);
                m_ComparisonTable.SetSelection(selection);
            }

            var markerName = GetPairingName(index);
            UpdateSelectedMarkerName(markerName);
        }

        private string GetPairingName(int index)
        {
            if (m_Pairings == null)
                return null;

            if (index < 0 || index >= m_Pairings.Count)
                return null;

            return m_Pairings[index].name;
        }

        private void SelectPairingByName(string pairingName)
        {
            if (m_Pairings != null && pairingName != null)
            {
                for (int index = 0; index < m_Pairings.Count; index++)
                {
                    var pairing = m_Pairings[index];
                    if (pairing.name == pairingName)
                    {
                        SelectPairing(index);
                        return;
                    }
                }
            }

            SelectPairing(-1);
        }

        private string GetFrameRangeText(ProfileAnalysis analysis)
        {
            var frameSummary = analysis.GetFrameSummary();

            string frameRange;
            if (frameSummary.first == frameSummary.last)
                frameRange = string.Format("{0}", frameSummary.first);
            else
                frameRange = string.Format("{0}-{1}", frameSummary.first, frameSummary.last);

            return frameRange;
        }

        private void DrawFrameSummary()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LayoutSize.WidthRHS));

            bool lastShowFrameSummary = m_ShowFrameSummary;
            m_ShowFrameSummary = BoldFoldout(m_ShowFrameSummary, Styles.frameSummary);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowFrameSummary)
            {
                if (IsAnalysisValid())
                {
                    var frameSummary = m_ProfileSingleView.analysis.GetFrameSummary();

                    m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);
                    m_Columns.Draw(0, "");

                    m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1 + LayoutSize.WidthColumn2 + LayoutSize.WidthColumn3, 0, 0); // Allow last column to go wide
                    m_Columns.Draw2("Frame Count", string.Format("{0}", frameSummary.count));
                    m_Columns.Draw2("Frame Range", GetFrameRangeText(m_ProfileSingleView.analysis));

                    m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);
                    m_Columns.Draw(0, "");
                    m_Columns.Draw3("", GetDisplayUnits(), "Frame");

                    Draw3LabelMsFrame(Styles.max, frameSummary.msMax, frameSummary.maxFrameIndex);
                    Draw2LabelMs(Styles.upperQuartile, frameSummary.msUpperQuartile);
                    Draw3LabelMsFrame(Styles.median, frameSummary.msMedian, frameSummary.medianFrameIndex);
                    Draw2LabelMs(Styles.mean, frameSummary.msMean);
                    Draw2LabelMs(Styles.lowerQuartile, frameSummary.msLowerQuartile);
                    Draw3LabelMsFrame(Styles.min, frameSummary.msMin, frameSummary.minFrameIndex);

                    GUIStyle style = GUI.skin.label;
                    GUILayout.Space(style.lineHeight);

                    EditorGUILayout.BeginHorizontal();
                    Histogram histogram = new Histogram(m_2D, m_DisplayUnits.Units);
                    histogram.Draw(LayoutSize.HistogramWidth, 40, frameSummary.buckets, frameSummary.count, 0, frameSummary.msMax, m_ColorBar);

                    BoxAndWhiskerPlot boxAndWhiskerPlot = new BoxAndWhiskerPlot(m_2D, m_DisplayUnits.Units);

                    float plotWidth = 40 + GUI.skin.box.padding.horizontal;
                    float plotHeight = 40;
                    boxAndWhiskerPlot.Draw(plotWidth, plotHeight, frameSummary.msMin, frameSummary.msLowerQuartile, frameSummary.msMedian, frameSummary.msUpperQuartile, frameSummary.msMax, 0, frameSummary.msMax, m_ColorStandardLine, m_ColorStandardLine);

                    boxAndWhiskerPlot.DrawText(m_Columns.GetColumnWidth(3), plotHeight, frameSummary.msMin, frameSummary.msMax,
                        "Min frame time for selected frames",
                        "Max frame time for selected frames");

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField("No analysis data selected");
                }
            }

            if (m_ShowFrameSummary != lastShowFrameSummary)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Frames, analytic.GetDurationInSeconds(), m_ShowFrameSummary);
            }

            EditorGUILayout.EndVertical();
        }

        private GUIContent GetThreadNameWithGroupTooltip(string threadNameWithIndex, bool singleThread)
        {
            string friendlyThreadName = GetFriendlyThreadName(threadNameWithIndex, singleThread);
            string groupName;
            friendlyThreadName = ProfileData.GetThreadNameWithoutGroup(friendlyThreadName, out groupName);

            if (groupName=="")
                return new GUIContent(friendlyThreadName, string.Format("{0}", friendlyThreadName));
            else 
                return new GUIContent(friendlyThreadName, string.Format("{0}\n{1}", friendlyThreadName, groupName));
        }

        private void DrawThreadSummary()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LayoutSize.WidthRHS));

            bool lastShowThreadSummary = m_ShowThreadSummary;
            m_ShowThreadSummary = BoldFoldout(m_ShowThreadSummary, Styles.threadSummary);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowThreadSummary)
            {
                if (IsAnalysisValid())
                {
                    float xAxisMin = 0.0f;
                    float xAxisMax = GetThreadTimeRange(m_ProfileSingleView.analysis);

                    m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2 + LayoutSize.WidthColumn3, 0);
                    ShowThreadRange();
                    
                    m_Columns.Draw3("", "Median", "Thread");

                    m_ThreadScroll = EditorGUILayout.BeginScrollView(m_ThreadScroll, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(200));
                    Rect clipRect = new Rect(m_ThreadScroll.x, m_ThreadScroll.y, 400, 200);
                    m_2D.SetClipRect(clipRect);
                    for (int i = 0; i < m_ThreadUINames.Count; i++)
                    {
                        string threadNameWithIndex = m_ThreadNames[i];
                        if (!threadNameWithIndex.Contains(":"))
                            continue;    // Ignore 'All' 

                        bool include = ProfileAnalyzer.MatchThreadFilter(threadNameWithIndex, m_ThreadSelection.selection);
                        if (!include)
                            continue;

                        ThreadData thread = m_ProfileSingleView.analysis.GetThreadByName(threadNameWithIndex);
                        if (thread==null)    // May be the 'all' field
                            continue;
                    
                        bool singleThread = thread.threadsInGroup > 1 ? false : true;

                        BoxAndWhiskerPlot boxAndWhiskerPlot = new BoxAndWhiskerPlot(m_2D, m_DisplayUnits.Units);
                        EditorGUILayout.BeginHorizontal();
                        boxAndWhiskerPlot.DrawHorizontal(100, GUI.skin.label.lineHeight, thread.msMin, thread.msLowerQuartile, thread.msMedian, thread.msUpperQuartile, thread.msMax, xAxisMin, xAxisMax, m_ColorBar, m_ColorBarBackground, GUI.skin.label);

                        m_Columns.Draw(1, ToDisplayUnitsWithTooltips(thread.msMedian));
                        m_Columns.Draw(2, GetThreadNameWithGroupTooltip(thread.threadNameWithIndex, singleThread));
                        EditorGUILayout.EndHorizontal();
                    }
                    m_2D.ClearClipRect();
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("No analysis data selected");
                }
            }

            if (m_ShowThreadSummary != lastShowThreadSummary)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Threads, analytic.GetDurationInSeconds(), m_ShowThreadSummary);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawHistogramForMarker(Histogram histogram, MarkerData marker)
        {
            if (DisplayCount())
                histogram.Draw(LayoutSize.HistogramWidth, 100, marker.countBuckets, marker.presentOnFrameCount, marker.countMin, marker.countMax, m_ColorBar);
            else
                histogram.Draw(LayoutSize.HistogramWidth, 100, marker.buckets, marker.presentOnFrameCount, marker.msMin, marker.msMax, m_ColorBar);
        }

        public bool IsProfilerWindowOpen()
        {
            return m_ProfilerWindowInterface.IsReady();
        }

        public bool DataMatchesProfiler(ProfileData data, int frameIndex, out string message)
        {
            if (data == null)
            {
                message = "";
                return false;
            }

            // Don't check full range match as we may have only captured a single frame from the data
            /*
            int dataFirstFrameIndex = data.OffsetToDisplayFrame(0);
            int dataLastFrameIndex = data.OffsetToDisplayFrame(m_ProfileSingleView.data.GetFrameCount() - 1);
            int profilerFirstFrameIndex;
            int profilerLastFrameIndex;
            m_profilerWindowInterface.GetFrameRangeFromProfiler(out firstFrameIndex, out lastFrameIndex);

            if (dataFirstFrameIndex != profilerFirstFrameIndex ||
                dataLastFrameIndex != profilerLastFrameIndex)
            {
                message = string.Format("Data in profiler doesn't match data range({0}-{1}) != profiler range ({2}-{3}",
                                        dataFirstFrameIndex,
                                        dataLastFrameIndex,
                                        profilerFirstFrameIndex,
                                        profilerLastFrameIndex);
                return false;
            }
            */

            // Check check the frame we are jumping to.
            int dataFrameOffset = data.DisplayFrameToOffset(frameIndex);            // Convert from user facing index to zero based offset into analysis data
            int frames = data.GetFrameCount();
            if (dataFrameOffset < 0 || dataFrameOffset >= frames)
            {
                message = string.Format("Timing data in profiler doesn't match : Frame {0} out of range {1}-{2}",
                                        frameIndex, data.OffsetToDisplayFrame(0), data.OffsetToDisplayFrame(frames - 1));
                return false;
            }
            float msData = data.GetFrame(dataFrameOffset).msFrame;
            float msProfiler = m_ProfilerWindowInterface.GetFrameTime(frameIndex - 1); // Convert from user facing index to zero based index into profiler data
            if (msData != msProfiler)
            {
                message = string.Format("Timing data in profiler doesn't match for frame {0} : {1:f2}!={2:f2}",
                                        frameIndex, msData, msProfiler);
                return false;
            }

            message = "";
            return true;
        }

        public void JumpToFrame(int frameindex, bool reportErrors=true)
        {
            if (m_ProfilerWindowInterface.JumpToFrame(frameindex))
            {
                var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
                ProfileData data = m_ProfileSingleView.data;
                string message;
                bool dataMatch = DataMatchesProfiler(data, frameindex, out message);
                if (!dataMatch && reportErrors)
                {
                    Debug.Log(message);
                }
                ProfileAnalyzerAnalytics.SendUIButtonEvent(ProfileAnalyzerAnalytics.UIButton.JumpToFrame, analytic);
            }
        }

        public void DrawFrameIndexButton(int index)
        {
            if (index < 0)
                return;

            bool enabled = GUI.enabled;
            if (!IsProfilerWindowOpen())
                GUI.enabled = false;
            
            if (GUILayout.Button(new GUIContent(string.Format("{0}", index),string.Format("Jump to frame {0} in the Unity Profiler", index)), GUILayout.Height(14), GUILayout.Width(50)))
            {
                JumpToFrame(index);
            }

            GUI.enabled = enabled;
        }

        public void DrawFrameIndexButton(Rect rect, int index)
        {
            if (index < 0)
                return;

            bool enabled = GUI.enabled;
            if (!IsProfilerWindowOpen())
                GUI.enabled = false;

            // Clamp to max height to match other buttons
            // And centre vertically if needed
            rect.y += (rect.height - 14) / 2;
            rect.height = Math.Min(rect.height, 14);

            if (GUI.Button(rect, new GUIContent(string.Format("{0}", index), string.Format("Jump to frame {0} in the Unity Profiler", index))))
            {
                JumpToFrame(index);
            }

            GUI.enabled = enabled;
        }

        GUIContent ToDisplayUnitsWithTooltips(float ms, bool showUnits = false, int frameIndex = -1)
        {
            return m_DisplayUnits.ToGUIContentWithTooltips(ms, showUnits, 5, frameIndex);
        }

        private void Draw3LabelMsFrame(string col1, float ms, int frameIndex)
        {
            EditorGUILayout.BeginHorizontal();
            m_Columns.Draw(0, col1);
            m_Columns.Draw(1, ToDisplayUnitsWithTooltips(ms));
            DrawFrameIndexButton(frameIndex);
            EditorGUILayout.EndHorizontal();
        }

        private void Draw3LabelMsFrame(GUIContent col1, float ms, int frameIndex)
        {
            EditorGUILayout.BeginHorizontal();
            m_Columns.Draw(0, col1);
            m_Columns.Draw(1, ToDisplayUnitsWithTooltips(ms));
            DrawFrameIndexButton(frameIndex);
            EditorGUILayout.EndHorizontal();
        }

        private void Draw2LabelMs(GUIContent col1, float ms)
        {
            EditorGUILayout.BeginHorizontal();
            m_Columns.Draw(0, col1);
            m_Columns.Draw(1, ToDisplayUnitsWithTooltips(ms));
            EditorGUILayout.EndHorizontal();
        }

        private void Draw4DiffMs(GUIContent col1, float msLeft, float msRight)
        {
            EditorGUILayout.BeginHorizontal();
            m_Columns.Draw(0, col1);
            m_Columns.Draw(1, ToDisplayUnitsWithTooltips(msLeft));
            m_Columns.Draw(2, ToDisplayUnitsWithTooltips(msRight));
            m_Columns.Draw(3, ToDisplayUnitsWithTooltips(msRight-msLeft));
            EditorGUILayout.EndHorizontal();
        }

        private void Draw4DiffMs(GUIContent col1, float msLeft, int frameIndexLeft, float msRight, int frameIndexRight)
        {
            EditorGUILayout.BeginHorizontal();
            m_Columns.Draw(0, col1);
            m_Columns.Draw(1, ToDisplayUnitsWithTooltips(msLeft, false, frameIndexLeft));
            m_Columns.Draw(2, ToDisplayUnitsWithTooltips(msRight, false, frameIndexRight));
            m_Columns.Draw(3, ToDisplayUnitsWithTooltips(msRight - msLeft));
            EditorGUILayout.EndHorizontal();
        }

        private void Draw4Ms(GUIContent col1, float value2, float value3, float value4)
        {
            EditorGUILayout.BeginHorizontal();
            m_Columns.Draw(0, col1);
            m_Columns.Draw(1, ToDisplayUnitsWithTooltips(value2));
            m_Columns.Draw(2, ToDisplayUnitsWithTooltips(value3));
            m_Columns.Draw(3, ToDisplayUnitsWithTooltips(value4));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBoxAndWhiskerPlotForMarker(BoxAndWhiskerPlot boxAndWhiskerPlot, float width, float height,ProfileAnalysis analysis, MarkerData marker, float yAxisStart, float yAxisEnd, Color color, Color colorBackground)
        {
            if (marker == null)
            {
                boxAndWhiskerPlot.Draw(width, height, 0, 0, 0, 0, 0, yAxisStart, yAxisEnd, color, colorBackground);
                return;
            }
            
            if (DisplayCount())
                boxAndWhiskerPlot.Draw(width, height, marker.countMin, marker.countLowerQuartile, marker.countMedian, marker.countUpperQuartile, marker.countMax, yAxisStart, yAxisEnd, color, colorBackground);
            else       
                boxAndWhiskerPlot.Draw(width, height, marker.msMin, marker.msLowerQuartile, marker.msMedian, marker.msUpperQuartile, marker.msMax, yAxisStart, yAxisEnd, color, colorBackground);
        }

        private void DrawBoxAndWhiskerPlotHorizontalForMarker(BoxAndWhiskerPlot boxAndWhiskerPlot, float width, float height, ProfileAnalysis analysis, MarkerData marker, float yAxisStart, float yAxisEnd, Color color, Color colorBackground)
        {
            boxAndWhiskerPlot.DrawHorizontal(width,height, marker.msMin, marker.msLowerQuartile, marker.msMedian, marker.msUpperQuartile, marker.msMax, yAxisStart, yAxisEnd, color, colorBackground);
        }

        void DrawFrameRatio(MarkerData marker)
        {
            var frameSummary = m_ProfileSingleView.analysis.GetFrameSummary();

            GUIStyle style = GUI.skin.label;
            float w = LayoutSize.WidthColumn0;
            float h = style.lineHeight;
            float ySpacing = 2;
            float barHeight = h - ySpacing;

            EditorGUILayout.BeginVertical(GUILayout.Width(w + LayoutSize.WidthColumn1 + LayoutSize.WidthColumn2));

            float barMax = frameSummary.msMean;
            float barValue = marker.msMean;
            string text = "Mean frame contribution";
            Units units = m_DisplayUnits.Units;
            if (DisplayCount()) 
            { 
                units = Units.Count;
                barMax = frameSummary.markerCountMaxMean;
                barValue = marker.countMean;
                text = "Mean count";
            }
            DisplayUnits displayUnits = new DisplayUnits(units);
            float barLength = Math.Min((w * barValue) / barMax, w);

            EditorGUILayout.LabelField(text);
            m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);

            m_Columns.Draw2("", "");
            EditorGUILayout.BeginHorizontal();

            // NOTE: This can effect the whole width of the region its inside
            // Not clear why
            if (m_2D.DrawStart(w, h, Draw2D.Origin.TopLeft, style))
            {
                m_2D.DrawFilledBox(0, ySpacing, barLength, barHeight, m_ColorBar);
                m_2D.DrawFilledBox(barLength, ySpacing, w - barLength, barHeight, m_ColorBarBackground);
                m_2D.DrawEnd();

                Rect rect = GUILayoutUtility.GetLastRect();
                string tooltip = string.Format("{0}", displayUnits.ToString(barValue, true, 5));
                GUI.Label(rect, new GUIContent("", tooltip));
            }

            EditorGUILayout.LabelField(ShowPercent((100 * barValue) / barMax), GUILayout.MaxWidth(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        string ShowPercent(float percent)
        {
            if (percent>=100f)
                return string.Format("{0:f0}%", percent);

            return string.Format("{0:f2}%", percent);
        }

        void DrawComparisonFrameRatio(MarkerData leftMarker, MarkerData rightMarker)
        {
            var leftFrameSummary = m_ProfileLeftView.analysis.GetFrameSummary();
            var rightFrameSummary = m_ProfileRightView.analysis.GetFrameSummary();

            GUIStyle style = GUI.skin.label;
            float w = LayoutSize.WidthColumn0;
            float h = style.lineHeight;
            float ySpacing = 2;
            float barHeight = (h - ySpacing) / 2;

            EditorGUILayout.BeginVertical(GUILayout.Width(w + LayoutSize.WidthColumn1 + LayoutSize.WidthColumn2));

            float leftBarValue = MarkerData.GetMsMean(leftMarker);
            float rightBarValue = MarkerData.GetMsMean(rightMarker);
            float leftBarMax = leftFrameSummary.msMean;
            float rightBarMax = rightFrameSummary.msMean;
            
            string text = "Mean frame contribution";
            Units units = m_DisplayUnits.Units;
            if (DisplayCount()) 
            { 
                units = Units.Count;
                leftBarValue = MarkerData.GetCountMean(leftMarker);
                rightBarValue = MarkerData.GetCountMean(rightMarker);
                leftBarMax = leftFrameSummary.markerCountMaxMean;
                rightBarMax = rightFrameSummary.markerCountMaxMean;
                text = "Mean count";
            }
            
            DisplayUnits displayUnits = new DisplayUnits(units);

            float leftBarLength = Math.Min((w * leftBarValue) / leftBarMax, w);
            float rightBarLength = Math.Min((w * rightBarValue) / rightBarMax, w);

            EditorGUILayout.LabelField(text);
            m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);
            m_Columns.Draw4("", "Left", "Right", "Diff");
            EditorGUILayout.BeginHorizontal();
            if (m_2D.DrawStart(w, h, Draw2D.Origin.TopLeft, style))
            {
                m_2D.DrawFilledBox(0, ySpacing, w, h - ySpacing, m_ColorBarBackground);

                m_2D.DrawFilledBox(0, ySpacing, leftBarLength, barHeight, m_ColorLeft);
                m_2D.DrawFilledBox(0, ySpacing + barHeight, rightBarLength, barHeight, m_ColorRight);
                m_2D.DrawEnd();

                Rect rect = GUILayoutUtility.GetLastRect();
                string tooltip = string.Format("Left: {0}\nRight: {1}", displayUnits.ToString(leftBarValue, true, 5), displayUnits.ToString(rightBarValue, true, 5));
                GUI.Label(rect, new GUIContent("", tooltip));
            }
            float leftPercentage = (100 * leftBarValue) / leftBarMax;
            float rightPercentage = (100 * rightBarValue) / rightBarMax;

            EditorGUILayout.LabelField(ShowPercent(leftPercentage), GUILayout.Width(LayoutSize.WidthColumn1));
            EditorGUILayout.LabelField(ShowPercent(rightPercentage), GUILayout.Width(LayoutSize.WidthColumn2));
            if (leftMarker!=null && rightMarker!=null)
                EditorGUILayout.LabelField(ShowPercent(rightPercentage - leftPercentage), GUILayout.Width(LayoutSize.WidthColumn3));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
          
        void DrawTopComparison(MarkerData leftMarker, MarkerData rightMarker)
        {
            GUIStyle style = GUI.skin.label;
            float w = LayoutSize.WidthColumn0;
            float h = style.lineHeight;
            float ySpacing = 2;
            float barHeight = (h - ySpacing) /2;

            EditorGUILayout.BeginVertical(GUILayout.Width(w + LayoutSize.WidthColumn1 + LayoutSize.WidthColumn2));
            
            float leftMax = MarkerData.GetMsMax(leftMarker);
            float rightMax = MarkerData.GetMsMax(rightMarker);

            Units units = m_DisplayUnits.Units;
            if (DisplayCount()) 
            { 
                units = Units.Count;
                leftMax = MarkerData.GetCountMax(leftMarker);
                rightMax = MarkerData.GetCountMax(rightMarker);
            }
            DisplayUnits displayUnits = new DisplayUnits(units);
            
            TopMarkerList topMarkerList = new TopMarkerList(m_2D, units,
                LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2,
                m_ColorBar, m_ColorBarBackground, DrawFrameIndexButton);
            m_TopNumber = topMarkerList.DrawTopNumber(m_TopNumber, m_TopStrings, m_TopValues);

            float barMax = Math.Max(leftMax, rightMax);

            int leftIndex = leftMarker!=null ? leftMarker.frames.Count - 1 : -1;
            int rightIndex = rightMarker!=null ? rightMarker.frames.Count - 1 : -1;
            FrameTime zeroFrameTime = new FrameTime(-1, 0.0f, 0);
            for (int i = 0; i < m_TopNumber; i++)
            {
                FrameTime leftFrameTime = leftIndex >= 0 ? leftMarker.frames[leftIndex] : zeroFrameTime;
                FrameTime rightFrameTime = rightIndex >= 0 ? rightMarker.frames[rightIndex] : zeroFrameTime;

                float leftBarValue = leftFrameTime.ms;
                float rightBarValue = rightFrameTime.ms;
                if (DisplayCount())
                {
                    leftBarValue = leftFrameTime.count;
                    rightBarValue = rightFrameTime.count;
                }

                float leftBarLength = Math.Min((w * leftBarValue) / barMax, w);
                float rightBarLength = Math.Min((w * rightBarValue) / barMax, w);

                EditorGUILayout.BeginHorizontal();
                if (m_2D.DrawStart(w, h, Draw2D.Origin.TopLeft, style))
                {
                    if (leftIndex >= 0 || rightIndex >= 0)
                    {
                        m_2D.DrawFilledBox(0, ySpacing, w, h - ySpacing, m_ColorBarBackground);

                        m_2D.DrawFilledBox(0, ySpacing, leftBarLength, barHeight, m_ColorLeft);
                        m_2D.DrawFilledBox(0, ySpacing + barHeight, rightBarLength, barHeight, m_ColorRight);
                    }
                    m_2D.DrawEnd();

                    Rect rect = GUILayoutUtility.GetLastRect();
                    string leftContent = leftIndex >= 0 ? string.Format("{0} on frame {1}", displayUnits.ToString(leftBarValue, true, 5), leftFrameTime.frameIndex) : "None";
                    string rightContent = rightIndex >= 0 ? string.Format("{0} on frame {1}", displayUnits.ToString(rightBarValue, true, 5), rightFrameTime.frameIndex) : "None";
                    GUI.Label(rect, new GUIContent("", string.Format("Left:\t{0}\nRight:\t{1}", leftContent, rightContent)));
                }

                EditorGUILayout.LabelField(leftIndex>=0 ? displayUnits.ToGUIContentWithTooltips(leftBarValue, false, leftFrameTime.frameIndex) : Styles.emptyString, GUILayout.Width(LayoutSize.WidthColumn1));
                EditorGUILayout.LabelField(rightIndex>=0 ? displayUnits.ToGUIContentWithTooltips(rightBarValue, false, rightFrameTime.frameIndex) : Styles.emptyString, GUILayout.Width(LayoutSize.WidthColumn2));
                if (leftIndex >= 0 && rightIndex>=0)
                    EditorGUILayout.LabelField(displayUnits.ToGUIContentWithTooltips(rightBarValue - leftBarValue), GUILayout.Width(LayoutSize.WidthColumn3));
                EditorGUILayout.EndHorizontal();

                leftIndex--;
                rightIndex--;
            }

            EditorGUILayout.EndVertical();
        }

        void DrawSelectedStats(MarkerData marker)
        {
            GUIStyle style = GUI.skin.label;

            m_Columns.Draw3("", GetDisplayUnits(), "Frame");
            Draw3LabelMsFrame(Styles.max, marker.msMax, marker.maxFrameIndex);
            Draw2LabelMs(Styles.upperQuartile, marker.msUpperQuartile);
            Draw3LabelMsFrame(Styles.median, marker.msMedian, marker.medianFrameIndex);
            Draw2LabelMs(Styles.mean, marker.msMean);
            Draw2LabelMs(Styles.lowerQuartile, marker.msLowerQuartile);
            Draw3LabelMsFrame(Styles.min, marker.msMin, marker.minFrameIndex);

            GUILayout.Space(style.lineHeight);

            Draw3LabelMsFrame(Styles.individualMax, marker.msMaxIndividual,
                marker.maxIndividualFrameIndex);
            Draw3LabelMsFrame(Styles.individualMin, marker.msMinIndividual,
                marker.minIndividualFrameIndex);
        }

        void DrawSelected()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(LayoutSize.WidthRHS));

            bool lastMarkerSummary = m_ShowMarkerSummary;
            m_ShowMarkerSummary = BoldFoldout(m_ShowMarkerSummary, Styles.markerSummary);
            var analytic = ProfileAnalyzerAnalytics.BeginAnalytic();
            if (m_ShowMarkerSummary)
            {
                if (IsAnalysisValid())
                {
                    List<MarkerData> markers = m_ProfileSingleView.analysis.GetMarkers();
                    if (markers != null)
                    {
                        int markerAt = m_SelectedMarker;
                        if (markerAt >= 0 && markerAt < markers.Count)
                        {
                            var marker = markers[markerAt];

                            EditorGUILayout.LabelField(marker.name,
                                GUILayout.MaxWidth(LayoutSize.WidthRHS -
                                                   (GUI.skin.box.padding.horizontal + GUI.skin.box.margin.horizontal)));
                            
                            DrawFrameRatio(marker);

                            m_Columns.SetColumnSizes(LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2, LayoutSize.WidthColumn3);

                            EditorGUILayout.BeginHorizontal();
                            m_Columns.Draw(0, "First frame");
                            m_Columns.Draw(1, "");
                            DrawFrameIndexButton(marker.firstFrameIndex);
                            EditorGUILayout.EndHorizontal();

                            GUIStyle style = GUI.skin.label;
                            
                            float min = marker.msMin;
                            float max = marker.msMax;
                            string fieldString = "marker time";
                            Units units = m_DisplayUnits.Units;
                            if (DisplayCount()) 
                            { 
                                min = marker.countMin;
                                max = marker.countMax;
                                fieldString = "count";
                                units = Units.Count;
                            }
                            
                            TopMarkerList topMarkerList = new TopMarkerList(m_2D, units,
                                LayoutSize.WidthColumn0, LayoutSize.WidthColumn1, LayoutSize.WidthColumn2,
                                m_ColorBar, m_ColorBarBackground, DrawFrameIndexButton);
                            m_TopNumber = topMarkerList.Draw(marker, m_TopNumber, m_TopStrings, m_TopValues);

                            GUILayout.Space(style.lineHeight);

                            float plotWidth = 40 + GUI.skin.box.padding.horizontal;
                            float plotHeight = 100;

                            EditorGUILayout.BeginHorizontal();
                            
                            Histogram histogram = new Histogram(m_2D, units);
                            DrawHistogramForMarker(histogram, marker);
                            
                            BoxAndWhiskerPlot boxAndWhiskerPlot = new BoxAndWhiskerPlot(m_2D, units);
                            DrawBoxAndWhiskerPlotForMarker(boxAndWhiskerPlot, plotWidth, plotHeight, m_ProfileSingleView.analysis, marker,
                                                           min, max, m_ColorStandardLine, m_ColorBoxAndWhiskerBoxColor);

                            boxAndWhiskerPlot.DrawText(m_Columns.GetColumnWidth(3), plotHeight, min, max, 
                                string.Format("Min {0} for selected frames", fieldString), 
                                string.Format("Max {0} for selected frames", fieldString));
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(style.lineHeight);

                            DrawSelectedStats(marker);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No marker data selected");
                }
            }

            if (m_ShowMarkerSummary != lastMarkerSummary)
            {
                ProfileAnalyzerAnalytics.SendUIVisibilityEvent(ProfileAnalyzerAnalytics.UIVisibility.Markers, analytic.GetDurationInSeconds(), m_ShowMarkerSummary);
            }

            EditorGUILayout.EndVertical();
        }
    }
}