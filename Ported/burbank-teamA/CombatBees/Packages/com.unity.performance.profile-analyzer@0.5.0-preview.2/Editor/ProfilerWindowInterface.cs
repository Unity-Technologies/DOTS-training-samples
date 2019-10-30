using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class ProfilerWindowInterface
    {
        private Type m_ProfilerWindowType;
        private EditorWindow m_ProfilerWindow;
        private FieldInfo m_CurrentFrameFieldInfo;
        private FieldInfo m_TimeLineGUIFieldInfo;
        private FieldInfo m_SelectedEntryFieldInfo;
        private FieldInfo m_SelectedNameFieldInfo;
        private FieldInfo m_SelectedTimeFieldInfo;
        private FieldInfo m_SelectedDurationFieldInfo;
        private FieldInfo m_SelectedInstanceIdFieldInfo;
        private FieldInfo m_SelectedInstanceCountFieldInfo;
        private FieldInfo m_SelectedFrameIdFieldInfo;
        private FieldInfo m_SelectedThreadIdFieldInfo;
        private FieldInfo m_SelectedNativeIndexFieldInfo;

        private MethodInfo m_GetProfilerModuleInfo;
        private Type m_CPUProfilerModuleType;

        public ProfilerWindowInterface()
        {
            Assembly assem = typeof(Editor).Assembly;
            m_ProfilerWindowType = assem.GetType("UnityEditor.ProfilerWindow");
            m_CurrentFrameFieldInfo = m_ProfilerWindowType.GetField("m_CurrentFrame", BindingFlags.NonPublic | BindingFlags.Instance);

            m_TimeLineGUIFieldInfo = m_ProfilerWindowType.GetField("m_CPUTimelineGUI", BindingFlags.NonPublic | BindingFlags.Instance);
            if (m_TimeLineGUIFieldInfo == null)
            {
                // m_CPUTimelineGUI isn't present in 2019.3.0a8 onward
                m_GetProfilerModuleInfo = m_ProfilerWindowType.GetMethod("GetProfilerModule", BindingFlags.NonPublic | BindingFlags.Instance);
                if (m_GetProfilerModuleInfo == null)
                {
                    Debug.Log("Unable to initialise link to Profiler Timeline, no GetProfilerModule found");
                }

                m_CPUProfilerModuleType = assem.GetType("UnityEditorInternal.Profiling.CPUProfilerModule");
                m_TimeLineGUIFieldInfo = m_CPUProfilerModuleType.GetField("m_TimelineGUI", BindingFlags.NonPublic | BindingFlags.Instance);
                if (m_TimeLineGUIFieldInfo == null)
                {
                    Debug.Log("Unable to initialise link to Profiler Timeline");
                }
            }

            if (m_TimeLineGUIFieldInfo != null)
                m_SelectedEntryFieldInfo = m_TimeLineGUIFieldInfo.FieldType.GetField("m_SelectedEntry", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (m_SelectedEntryFieldInfo != null)
            {
                m_SelectedNameFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                m_SelectedTimeFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("time", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                m_SelectedDurationFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("duration", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                m_SelectedInstanceIdFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("instanceId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                m_SelectedInstanceCountFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("instanceCount", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                m_SelectedFrameIdFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("frameId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                m_SelectedThreadIdFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("threadId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                m_SelectedNativeIndexFieldInfo = m_SelectedEntryFieldInfo.FieldType.GetField("nativeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

        }

        /*
        public EditorWindow GetProfileWindow()
        {
            return m_profilerWindow;
        }
        */

        public bool IsReady()
        {
            if (m_ProfilerWindow != null)
                return true;

            return false;
        }

        public bool IsProfilerWindowOpen()
        {
            Profiler.BeginSample("IsProfilerWindowOpen");
            UnityEngine.Object[] windows = Resources.FindObjectsOfTypeAll(m_ProfilerWindowType);
            bool result = (windows != null && windows.Length > 0) ? true : false;
            Profiler.EndSample();

            return result;
        }

        public void OpenProfilerOrUseExisting()
        {
            m_ProfilerWindow = EditorWindow.GetWindow(m_ProfilerWindowType);
        }

        public bool GetFrameRangeFromProfiler(out int first, out int last)
        {
            if (m_ProfilerWindow)
            //if (ProfilerDriver.enabled)
            {
                first = 1 + ProfilerDriver.firstFrameIndex;
                last = 1 + ProfilerDriver.lastFrameIndex;
                // Clip to the visible frames in the profile which indents 1 in from end
                if (first < last)
                    last--;
                return true;
            }

            first = 1;
            last = 1;
            return false;
        }

        public void CloseProfiler()
        {
            if (m_ProfilerWindow)
                m_ProfilerWindow.Close();
        }

        public object GetTimeLineGUI()
        {
            object timeLineGUI = null;

            if (m_CPUProfilerModuleType != null)
            {
                object[] parametersArray = new object[] { ProfilerArea.CPU };
                var getCPUProfilerModuleInfo = m_GetProfilerModuleInfo.MakeGenericMethod(m_CPUProfilerModuleType);
                var cpuModule = getCPUProfilerModuleInfo.Invoke(m_ProfilerWindow, parametersArray);

                timeLineGUI = m_TimeLineGUIFieldInfo.GetValue(cpuModule);
            }
            else if (m_TimeLineGUIFieldInfo != null)
            {
                timeLineGUI = m_TimeLineGUIFieldInfo.GetValue(m_ProfilerWindow);
            }

            return timeLineGUI;
        }

        public string GetProfilerWindowMarkerName()
        {
            if (m_ProfilerWindow!=null)
            {
                var timeLineGUI = GetTimeLineGUI();
                if (timeLineGUI != null && m_SelectedEntryFieldInfo != null)
                {
                    var selectedEntry = m_SelectedEntryFieldInfo.GetValue(timeLineGUI);
                    if (selectedEntry != null && m_SelectedNameFieldInfo != null)
                    {
                        return m_SelectedNameFieldInfo.GetValue(selectedEntry).ToString();
                    }
                }
            }

            return null;
        }

        public float GetFrameTime(int frameIndex)
        {
            ProfilerFrameDataIterator frameData = new ProfilerFrameDataIterator();

            frameData.SetRoot(frameIndex, 0);
            float ms = frameData.frameTimeMS;
            frameData.Dispose();

            return ms;
        }

        private bool GetMarkerInfo(string markerName, int frameIndex, List<string> threadFilters, out int outThreadIndex, out float time, out float duration, out int instanceId)
        {
            ProfilerFrameDataIterator frameData = new ProfilerFrameDataIterator();

            outThreadIndex = 0;
            time = 0.0f;
            duration = 0.0f;
            instanceId = 0;
            bool found = false;

            int threadCount = frameData.GetThreadCount(frameIndex);
            Dictionary<string, int> threadNameCount = new Dictionary<string, int>();
            for (int threadIndex = 0; threadIndex < threadCount; ++threadIndex)
            {
                frameData.SetRoot(frameIndex, threadIndex);

                var threadName = frameData.GetThreadName();
                // Name here could be "Worker Thread 1"

                var groupName = frameData.GetGroupName();
                threadName = ProfileData.GetThreadNameWithGroup(threadName, groupName);

                int nameCount = 0;
                threadNameCount.TryGetValue(threadName, out nameCount);
                threadNameCount[threadName] = nameCount + 1;

                var threadNameWithIndex = ProfileData.ThreadNameWithIndex(threadNameCount[threadName], threadName);

                // To compare on the filter we need to remove the postfix on the thread name
                // "3:Worker Thread 0" -> "1:Worker Thread"
                // The index of the thread (0) is used +1 as a prefix 
                // The preceding number (3) is the count of number of threads with this name
                // Unfortunately multiple threads can have the same name
                threadNameWithIndex = ProfileData.CorrectThreadName(threadNameWithIndex);

                if (threadFilters.Contains(threadNameWithIndex))
                {
                    const bool enterChildren = true;
                    while (frameData.Next(enterChildren))
                    {
                        if (frameData.name == markerName)
                        {
                            time = frameData.startTimeMS;
                            duration = frameData.durationMS;
                            instanceId = frameData.instanceId;
                            outThreadIndex = threadIndex;
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                    break;
            }

            frameData.Dispose();
            return found;
        }

        public void SetProfilerWindowMarkerName(string markerName, List<string> threadFilters)
        {
            if (m_ProfilerWindow == null)
                return;

            var timeLineGUI = GetTimeLineGUI();
            if (timeLineGUI==null)
                return;

            if (m_SelectedEntryFieldInfo != null)
            {
                var selectedEntry = m_SelectedEntryFieldInfo.GetValue(timeLineGUI);
                if (selectedEntry != null)
                {
                    // Read profiler data direct from profile to find time/duration
                    int currentFrameIndex = (int)m_CurrentFrameFieldInfo.GetValue(m_ProfilerWindow);
                    float time;
                    float duration;
                    int instanceId;
                    int threadIndex;
                    if (GetMarkerInfo(markerName, currentFrameIndex, threadFilters, out threadIndex, out time, out duration, out instanceId))
                    {
                        /*
                        Debug.Log(string.Format("Setting profiler to {0} on {1} at frame {2} at {3}ms for {4}ms ({5})", 
                                                markerName, currentFrameIndex, threadFilter, time, duration, instanceId));
                         */
                        
                        if (m_SelectedNameFieldInfo != null)
                            m_SelectedNameFieldInfo.SetValue(selectedEntry, markerName);
                        if (m_SelectedTimeFieldInfo != null)
                            m_SelectedTimeFieldInfo.SetValue(selectedEntry, time);
                        if (m_SelectedDurationFieldInfo != null)
                            m_SelectedDurationFieldInfo.SetValue(selectedEntry, duration);
                        if (m_SelectedInstanceIdFieldInfo != null)
                            m_SelectedInstanceIdFieldInfo.SetValue(selectedEntry, instanceId);
                        if (m_SelectedFrameIdFieldInfo != null)
                            m_SelectedFrameIdFieldInfo.SetValue(selectedEntry, currentFrameIndex);
                        if (m_SelectedThreadIdFieldInfo != null)
                            m_SelectedThreadIdFieldInfo.SetValue(selectedEntry, threadIndex);
                        
                        // TODO : Update to fill in the total and number of instances.
                        // For now we force Instance count to 1 to avoid the incorrect info showing.
                        if (m_SelectedInstanceCountFieldInfo != null)
                            m_SelectedInstanceCountFieldInfo.SetValue(selectedEntry, 1);

                        // Set other values to non negative values so selection appears
                        if (m_SelectedNativeIndexFieldInfo != null)
                            m_SelectedNativeIndexFieldInfo.SetValue(selectedEntry, currentFrameIndex);

                        m_ProfilerWindow.Repaint();
                    }
                }
            }
        }

        public bool JumpToFrame(int index)
        {
            //if (!ProfilerDriver.enabled)
            //    return;

            if (!m_ProfilerWindow)
                return false;
            
            m_CurrentFrameFieldInfo.SetValue(m_ProfilerWindow, index - 1);
            m_ProfilerWindow.Repaint();
            return true;
        }
    }
}
