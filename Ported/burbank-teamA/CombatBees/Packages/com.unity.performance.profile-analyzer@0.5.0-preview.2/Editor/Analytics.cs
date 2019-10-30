using System;
using System.Collections.Generic;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    class ProfileAnalyzerAnalytics
    {
        const int k_MaxEventsPerHour = 100;
        const int k_MaxEventItems = 1000;
        const string k_VendorKey = "unity.profileanalyzer";
        const string k_EventTopicName = "usability";

        static bool s_EnableAnalytics = false;

        public static void EnableAnalytics()
        {
#if UNITY_2018_1_OR_NEWER
            AnalyticsResult result = EditorAnalytics.RegisterEventWithLimit(k_EventTopicName, k_MaxEventsPerHour, k_MaxEventItems, k_VendorKey);
            if (result == AnalyticsResult.Ok)
                s_EnableAnalytics = true;
#endif
        }

        public enum UIButton
        {
            Pull,
            OpenProfiler,
            CloseProfiler,
            JumpToFrame,
            ExportSingleFrames,
            ExportComparisonFrames,
        };

        public enum UIUsageMode
        {
            Single,
            Comparison,
        };

        public enum UIVisibility
        {
            FrameTimeContextMenu,
            Filters,
            TopTen,
            Frames,
            Threads,
            Markers,
        };

        public enum UIResizeView
        {
            Single,
            Comparison,
        };


        [Serializable]
        struct ProfileAnalyzerUIButtonEventParameters
        {
            public string name;

            public ProfileAnalyzerUIButtonEventParameters(string name)
            {
                this.name = name;
            }
        }

        // camelCase since these events get serialized to Json and naming convention in analytics is camelCase
        [Serializable]
        struct ProfileAnalyzerUIButtonEvent
        {
            public ProfileAnalyzerUIButtonEvent(string name, float durationInTicks)
            {
                subtype = "profileAnalyzerUIButton";

                // ts is auto added so no need to include it here
                //ts = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                this.duration = durationInTicks;

                parameters = new ProfileAnalyzerUIButtonEventParameters(name);
            }

            public string subtype;
            //public int ts;
            public float duration;  // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            public ProfileAnalyzerUIButtonEventParameters parameters;
        }

        [Serializable]
        struct ProfileAnalyzerUIUsageEventParameters
        {
            public string name;

            public ProfileAnalyzerUIUsageEventParameters(string name)
            {
                this.name = name;
            }
        }

        [Serializable]
        struct ProfileAnalyzerUIUsageEvent
        {
            public ProfileAnalyzerUIUsageEvent(string name, float durationInTicks)
            {
                subtype = "profileAnalyzerModeUsage";

                this.duration = durationInTicks;

                parameters = new ProfileAnalyzerUIUsageEventParameters(name);
            }

            public string subtype;
            public float duration;  // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            public ProfileAnalyzerUIUsageEventParameters parameters;
        }

        [Serializable]
        struct ProfileAnalyzerUIVisibilityEventParameters
        {
            public string name;
            public bool show;

            public ProfileAnalyzerUIVisibilityEventParameters(string name, bool show)
            {
                this.name = name;
                this.show = show;
            }
        }

        [Serializable]
        struct ProfileAnalyzerUIVisibilityEvent
        {
            public ProfileAnalyzerUIVisibilityEvent(string name, float durationInTicks, bool show)
            {
                subtype = "profileAnalyzerUIVisibility";

                this.duration = durationInTicks;

                parameters = new ProfileAnalyzerUIVisibilityEventParameters(name, show);
            }

            public string subtype;
            public float duration;  // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            public ProfileAnalyzerUIVisibilityEventParameters parameters;
        }

        [Serializable]
        struct ProfileAnalyzerUIResizeEventParameters
        {
            public string name;
            public float width;
            public float height;
            public float screenWidth;
            public float screenHeight;
            public bool docked;

            public ProfileAnalyzerUIResizeEventParameters(string name, float width, float height, float screenWidth, float screenHeight, bool isDocked)
            {
                this.name = name;
                this.width = width;
                this.height = height;
                this.screenWidth = screenWidth;
                this.screenHeight = screenHeight;
                docked = isDocked;
            }
        }

        [Serializable]
        struct ProfileAnalyzerUIResizeEvent
        {
            public ProfileAnalyzerUIResizeEvent(string name, float durationInTicks, float width, float height, float screenWidth, float screenHeight, bool isDocked)
            {
                subtype = "profileAnalyzerUIResize";

                this.duration = durationInTicks;

                parameters = new ProfileAnalyzerUIResizeEventParameters(name, width, height, screenWidth, screenHeight, isDocked);
            }

            public string subtype;
            public float duration;  // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            public ProfileAnalyzerUIResizeEventParameters parameters;
        }

        static float SecondsToTicks(float durationInSeconds)
        {
            return durationInSeconds * 10000;
        }

        public static bool SendUIButtonEvent(UIButton uiButton, float durationInSeconds)
        {
            if (!s_EnableAnalytics)
                return false;

#if UNITY_2018_1_OR_NEWER
            // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            float durationInTicks = SecondsToTicks(durationInSeconds);

            ProfileAnalyzerUIButtonEvent uiButtonEvent;
            switch (uiButton)
            {
                case UIButton.Pull:
                    uiButtonEvent = new ProfileAnalyzerUIButtonEvent("profilerAnalyzerGrab", durationInTicks);
                    break;
                case UIButton.OpenProfiler:
                    uiButtonEvent = new ProfileAnalyzerUIButtonEvent("profilerAnalyzerOpenProfiler", durationInTicks);
                    break;
                case UIButton.CloseProfiler:
                    uiButtonEvent = new ProfileAnalyzerUIButtonEvent("profilerAnalyzerCloseProfiler", durationInTicks);
                    break;
                case UIButton.JumpToFrame:
                    uiButtonEvent = new ProfileAnalyzerUIButtonEvent("profilerAnalyzerJumpToFrame", durationInTicks);
                    break;
                case UIButton.ExportSingleFrames:
                    uiButtonEvent = new ProfileAnalyzerUIButtonEvent("profilerAnalyzerExportSingleFrames", durationInTicks);
                    break;
                case UIButton.ExportComparisonFrames:
                    uiButtonEvent = new ProfileAnalyzerUIButtonEvent("profilerAnalyzerExportComparisonFrames", durationInTicks);
                    break;
                default:
                    Debug.LogFormat("SendUIButtonEvent: Unsupported button type : {0}", uiButton);
                    return false;
            }


            AnalyticsResult result = EditorAnalytics.SendEventWithLimit(k_EventTopicName, uiButtonEvent);
            if (result != AnalyticsResult.Ok)
                return false;

            return true;
#else
            return false;
#endif
        }

        public static bool SendUIUsageModeEvent(UIUsageMode uiUsageMode, float durationInSeconds)
        {
            if (!s_EnableAnalytics)
                return false;

#if UNITY_2018_1_OR_NEWER
            // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            float durationInTicks = SecondsToTicks(durationInSeconds);

            ProfileAnalyzerUIUsageEvent uiUsageEvent;
            switch (uiUsageMode)
            {
                case UIUsageMode.Single:
                    uiUsageEvent = new ProfileAnalyzerUIUsageEvent("profileAnalyzerSingle", durationInTicks);
                    break;
                case UIUsageMode.Comparison:
                    uiUsageEvent = new ProfileAnalyzerUIUsageEvent("profileAnalyzerCompare", durationInTicks);
                    break;
                default:
                    Debug.LogFormat("SendUsageEvent: Unsupported usage mode : {0}", uiUsageMode);
                    return false;
            }


            AnalyticsResult result = EditorAnalytics.SendEventWithLimit(k_EventTopicName, uiUsageEvent);
            if (result != AnalyticsResult.Ok)
                return false;

            return true;
#else
            return false;
#endif
        }

        public static bool SendUIVisibilityEvent(UIVisibility uiVisibility, float durationInSeconds, bool show)
        {
            if (!s_EnableAnalytics)
                return false;

#if UNITY_2018_1_OR_NEWER
            // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            float durationInTicks = SecondsToTicks(durationInSeconds);

            ProfileAnalyzerUIVisibilityEvent uiUsageEvent;
            switch (uiVisibility)
            {
                case UIVisibility.FrameTimeContextMenu:
                    uiUsageEvent = new ProfileAnalyzerUIVisibilityEvent("profilerAnalyzerFrameTimeContextMenu", durationInTicks, show);
                    break;
                case UIVisibility.Filters:
                    uiUsageEvent = new ProfileAnalyzerUIVisibilityEvent("profilerAnalyzerFilters", durationInTicks, show);
                    break;
                case UIVisibility.TopTen:
                    uiUsageEvent = new ProfileAnalyzerUIVisibilityEvent("profilerAnalyzerTopTen", durationInTicks, show);
                    break;
                case UIVisibility.Frames:
                    uiUsageEvent = new ProfileAnalyzerUIVisibilityEvent("profilerAnalyzerFrames", durationInTicks, show);
                    break;
                case UIVisibility.Threads:
                    uiUsageEvent = new ProfileAnalyzerUIVisibilityEvent("profilerAnalyzerThreads", durationInTicks, show);
                    break;
                case UIVisibility.Markers:
                    uiUsageEvent = new ProfileAnalyzerUIVisibilityEvent("profilerAnalyzerMarkers", durationInTicks, show);
                    break;
                default:
                    Debug.LogFormat("SendUIVisibilityEvent: Unsupported visibililty item : {0}", uiVisibility);
                    return false;
            }

            AnalyticsResult result = EditorAnalytics.SendEventWithLimit(k_EventTopicName, uiUsageEvent);
            if (result != AnalyticsResult.Ok)
                return false;

            return true;
#else
            return false;
#endif
        }

        public static bool SendUIResizeEvent(UIResizeView uiResizeView, float durationInSeconds, float width, float height, bool isDocked)
        {
            if (!s_EnableAnalytics)
                return false;

#if UNITY_2018_1_OR_NEWER
            // Duration is in "ticks" 100 nanosecond intervals. I.e. 0.1 microseconds
            float durationInTicks = SecondsToTicks(durationInSeconds);

            ProfileAnalyzerUIResizeEvent uiResizeEvent;
            switch (uiResizeView)
            {
                case UIResizeView.Single:
                    // Screen.width, Screen.height is game view size
                    uiResizeEvent = new ProfileAnalyzerUIResizeEvent("profileAnalyzerSingle", durationInTicks, width, height, Screen.currentResolution.width, Screen.currentResolution.height, isDocked);
                    break;
                case UIResizeView.Comparison:
                    uiResizeEvent = new ProfileAnalyzerUIResizeEvent("profileAnalyzerCompare", durationInTicks, width, height, Screen.currentResolution.width, Screen.currentResolution.height, isDocked);
                    break;
                default:
                    Debug.LogFormat("SendUIResizeEvent: Unsupported view : {0}", uiResizeView);
                    return false;
            }

            AnalyticsResult result = EditorAnalytics.SendEventWithLimit(k_EventTopicName, uiResizeEvent);
            if (result != AnalyticsResult.Ok)
                return false;

            return true;
#else
            return false;
#endif
        }


        public class Analytic
        {
            double m_StartTime;
            float m_DurationInSeconds;

            public Analytic()
            {
                m_StartTime = EditorApplication.timeSinceStartup;
                m_DurationInSeconds = 0;
            }

            public void End()
            {
                m_DurationInSeconds = (float)(EditorApplication.timeSinceStartup - m_StartTime);
            }

            public float GetDurationInSeconds()
            {
                return m_DurationInSeconds;
            }
        }

        static public Analytic BeginAnalytic()
        {
            return new Analytic();
        }

        static public void SendUIButtonEvent(UIButton uiButton, Analytic instance)
        {
            instance.End();
            SendUIButtonEvent(uiButton, instance.GetDurationInSeconds());
        }

        static public void SendUIUsageModeEvent(UIUsageMode uiUsageMode, Analytic instance)
        {
            instance.End();
            SendUIUsageModeEvent(uiUsageMode, instance.GetDurationInSeconds());
        }
    }
}
