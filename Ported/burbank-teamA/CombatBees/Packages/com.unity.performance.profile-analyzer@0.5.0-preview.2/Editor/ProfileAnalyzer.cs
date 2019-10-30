using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using System;
using System.Text;
using UnityEngine.Profiling;

namespace UnityEditor.Performance.ProfileAnalyzer
{ 
    public class ProfileAnalyzer
    {
        private int m_Progress = 0;
        private ProfilerFrameDataIterator m_frameData;
        private List<string> m_threadNames = new List<string>();
        private ProfileAnalysis m_analysis;
        private ProgressBarDisplay m_progressBar;

        public ProfileAnalyzer(ProgressBarDisplay progressBar)
        {
            m_progressBar = progressBar;
        }

        public void QuickScan()
        {
            var frameData = new ProfilerFrameDataIterator();

            m_threadNames.Clear();
            int frameIndex = 0;
            int threadCount = frameData.GetThreadCount(0);
            frameData.SetRoot(frameIndex, 0);

            Dictionary<string, int> threadNameCount = new Dictionary<string, int>();
            for (int threadIndex = 0; threadIndex < threadCount; ++threadIndex)
            {
                frameData.SetRoot(frameIndex, threadIndex);

                var threadName = frameData.GetThreadName();
                var groupName = frameData.GetGroupName();
                threadName = ProfileData.GetThreadNameWithGroup(threadName, groupName);

                if (!threadNameCount.ContainsKey(threadName))
                    threadNameCount.Add(threadName, 1);
                else
                    threadNameCount[threadName] += 1;

                string threadNameWithIndex = ProfileData.ThreadNameWithIndex(threadNameCount[threadName], threadName);
                threadNameWithIndex = ProfileData.CorrectThreadName(threadNameWithIndex);

                m_threadNames.Add(threadNameWithIndex);
            }

            frameData.Dispose();
        }

        public List<string> GetThreadNames()
        {
            return m_threadNames;
        }

        public ProfileData PullFromProfiler(int firstFrameDisplayIndex, int lastFrameDisplayIndex)
        {
            Profiler.BeginSample("ProfileAnalyzer.PullFromProfiler");
            ProfilerFrameDataIterator frameData = new ProfilerFrameDataIterator();
            int firstFrameIndex = firstFrameDisplayIndex - 1;
            int lastFrameIndex = lastFrameDisplayIndex - 1;
            ProfileData profileData = GetData(frameData, firstFrameIndex, lastFrameIndex);
            frameData.Dispose();
            Profiler.EndSample();
            return profileData;
        }

        private void CalculateFrameTimeStats(ProfileData data, out float median, out float mean, out float standardDeviation)
        {
            List<float> frameTimes = new List<float>();
            for (int frameIndex = 0; frameIndex < data.GetFrameCount(); frameIndex++)
            {
                var frame = data.GetFrame(frameIndex);
                float msFrame = frame.msFrame;
                frameTimes.Add(msFrame); 
            }
            frameTimes.Sort();
            median = frameTimes[frameTimes.Count / 2];


            double total = 0.0f;
            foreach (float msFrame in frameTimes)
            {
                total += msFrame;
            }
            mean = (float)(total / (double)frameTimes.Count);


            if (frameTimes.Count <= 1)
            {
                standardDeviation = 0f;
            }
            else
            {
                total = 0.0f;
                foreach (float msFrame in frameTimes)
                {
                    float d = msFrame - mean;
                    total += (d * d);
                }
                total /= (frameTimes.Count - 1);
                standardDeviation = (float)Math.Sqrt(total);
            }
        }

        private ProfileData GetData(ProfilerFrameDataIterator frameData, int firstFrameIndex, int lastFrameIndex)
        {
            var data = new ProfileData();
            data.SetFrameIndexOffset(firstFrameIndex);

            Dictionary<string, int> threadNameCount = new Dictionary<string, int>();
            for (int frameIndex = firstFrameIndex; frameIndex <= lastFrameIndex; ++frameIndex)
            {
                m_progressBar.AdvanceProgressBar();

                int threadCount = frameData.GetThreadCount(frameIndex);
                frameData.SetRoot(frameIndex, 0);

                var msFrame = frameData.frameTimeMS;

                /*
                if (frameIndex == lastFrameIndex)
                {
                    // Check if last frame appears to be invalid data
                    float median;
                    float mean;
                    float standardDeviation;
                    CalculateFrameTimeStats(data, out median, out mean, out standardDeviation);
                    float execessiveDeviation = (3f * standardDeviation);
                    if (msFrame > (median + execessiveDeviation))
                    {
                        Debug.LogFormat("Dropping last frame as it is significantly larger than the median of the rest of the data set {0} > {1} (median {2} + 3 * standard deviation {3})", msFrame, median + execessiveDeviation, median, standardDeviation);
                        break;
                    }
                    if (msFrame < (median - execessiveDeviation))
                    {
                        Debug.LogFormat("Dropping last frame as it is significantly smaller than the median of the rest of the data set {0} < {1} (median {2} - 3 * standard deviation {3})", msFrame, median - execessiveDeviation, median, standardDeviation);
                        break;
                    }
                }
                */

                ProfileFrame frame = new ProfileFrame();
                frame.msStartTime = 1000.0 * frameData.GetFrameStartS(frameIndex);
                frame.msFrame = msFrame;
                data.Add(frame);

                threadNameCount.Clear();
                for (int threadIndex = 0; threadIndex < threadCount; ++threadIndex)
                {
                    frameData.SetRoot(frameIndex, threadIndex);

                    var threadName = frameData.GetThreadName();
                    if (threadName.Trim() == "")
                    {
                        Debug.Log(string.Format("Warning: Unnamed thread found on frame {0}. Corrupted data suspected, ignoring frame", frameIndex));
                        continue;
                    }

                    var groupName = frameData.GetGroupName();
                    threadName = ProfileData.GetThreadNameWithGroup(threadName, groupName);

                    ProfileThread thread = new ProfileThread();
                    frame.Add(thread);

                    int nameCount = 0;
                    threadNameCount.TryGetValue(threadName, out nameCount);
                    threadNameCount[threadName] = nameCount + 1;

                    data.AddThreadName(ProfileData.ThreadNameWithIndex(threadNameCount[threadName], threadName), thread);

                    const bool enterChildren = true;
                    // The markers are in depth first order and the depth is known
                    // So we can infer a parent child relationship
                    while (frameData.Next(enterChildren))
                    {
                        var markerData = ProfileMarker.Create(frameData);

                        data.AddMarkerName(frameData.name, markerData);
                        thread.Add(markerData);
                    }
                }
            }

            data.Finalise();

            return data;
        }

        private int GetClampedOffsetToFrame(ProfileData profileData, int frameIndex)
        {
            int frameOffset = profileData.DisplayFrameToOffset(frameIndex);
            if (frameOffset < 0)
            {
                Debug.Log(string.Format("Frame index {0} offset {1} < 0, clamping", frameIndex, frameOffset));
                frameOffset = 0;
            }
            if (frameOffset >= profileData.GetFrameCount())
            {
                Debug.Log(string.Format("Frame index {0} offset {1} >= frame count {2}, clamping", frameIndex, frameOffset, profileData.GetFrameCount()));
                frameOffset = profileData.GetFrameCount() - 1;
            }

            return frameOffset;
        }

        public static bool MatchThreadFilter(string threadNameWithIndex, List<string> threadFilters)
        {
            if (threadFilters == null || threadFilters.Count==0)
                return false;

            if (threadFilters.Contains(threadNameWithIndex))
                return true;

            return false;
        }

        public bool IsNullOrWhiteSpace(string s)
        {
            // return string.IsNullOrWhiteSpace(parentMarker);
            if (s == null || Regex.IsMatch(s,@"^[\s]*$"))
                return true;

            return false;
        }

        public ProfileAnalysis Analyze(ProfileData profileData, List<int> selectionIndices, List<string> threadFilters, int depthFilter, bool selfTimes = false, string parentMarker = null, float timeScaleMax = 0)
        {
            m_Progress = 0;
            if (profileData == null)
            {
                return null;
            }
            if (profileData.GetFrameCount()<=0)
            {
                return null;
            }

            int frameCount = selectionIndices.Count;
            if (frameCount <= 0)
            {
                return null;
            }

            bool processMarkers = (threadFilters != null);

            ProfileAnalysis analysis = new ProfileAnalysis();
            analysis.SetRange(selectionIndices[0], selectionIndices[selectionIndices.Count-1]);

            m_threadNames.Clear();

            int maxMarkerDepthFound = 0;
            var threads = new Dictionary<string, ThreadData>();
            var markers = new Dictionary<string, MarkerData>();
            var allMarkers = new Dictionary<string, int>();


            bool filteringByParentMarker = false;
            int parentMarkerIndex = -1;
            if (!IsNullOrWhiteSpace(parentMarker))
            {
                // Returns -1 if this marker doesn't exist in the data set
                parentMarkerIndex = profileData.GetMarkerIndex(parentMarker);
                filteringByParentMarker = true;
            }

            int at = 0;
            foreach (int frameIndex in selectionIndices) 
            {
                int frameOffset = profileData.DisplayFrameToOffset(frameIndex);
                var frameData = profileData.GetFrame(frameOffset);
                if (frameData == null)
                    continue;
                var msFrame = frameData.msFrame;

                analysis.UpdateSummary(frameIndex, msFrame);

                if (processMarkers)
                {
                    for (int threadIndex = 0; threadIndex < frameData.threads.Count; threadIndex++)
                    {
                        float msTimeOfMinDepthMarkers = 0.0f;
                        float msIdleTimeOfMinDepthMarkers = 0.0f;

                        var threadData = frameData.threads[threadIndex];
                        var threadNameWithIndex = profileData.GetThreadName(threadData);

                        ThreadData thread;
                        if (!threads.ContainsKey(threadNameWithIndex))
                        {
                            m_threadNames.Add(threadNameWithIndex);

                            thread = new ThreadData(threadNameWithIndex);

                            analysis.AddThread(thread);
                            threads[threadNameWithIndex] = thread;

                            // Update threadsInGroup for all thread records of the same group name
                            foreach (var threadAt in threads.Values)
                            {
                                if (threadAt == thread)
                                    continue;
                                
                                if (thread.threadGroupName == threadAt.threadGroupName)
                                {
                                    threadAt.threadsInGroup += 1;
                                    thread.threadsInGroup += 1;
                                }
                            }
                        }
                        else
                        {
                            thread = threads[threadNameWithIndex];
                        }

                        bool include = MatchThreadFilter(threadNameWithIndex, threadFilters);

                        int parentMarkerDepth = -1;
                        foreach (ProfileMarker markerData in threadData.markers)
                        {
                            string markerName = profileData.GetMarkerName(markerData);
                            if (!allMarkers.ContainsKey(markerName))
                                allMarkers.Add(markerName, 1);
                            // No longer counting how many times we see the marker (this saves 1/3 of the analysis time).

                            float ms = markerData.msMarkerTotal - (selfTimes ? markerData.msChildren : 0);
                            var markerDepth = markerData.depth;
                            if (markerDepth > maxMarkerDepthFound)
                                maxMarkerDepthFound = markerDepth;

                            if (markerDepth == 1)
                            {
                                if (markerName == "Idle")
                                    msIdleTimeOfMinDepthMarkers += ms;
                                else
                                    msTimeOfMinDepthMarkers += ms;
                            }

                            if (!include)
                                continue;

                            if (depthFilter >= 0 && markerDepth != depthFilter)
                                continue;

                            // If only looking for markers below the parent
                            if (filteringByParentMarker)
                            {
                                // If found the parent marker
                                if (markerData.nameIndex == parentMarkerIndex)
                                {
                                    // And we are not already below the parent higher in the depth tree
                                    if (parentMarkerDepth < 0)
                                    {
                                        // record the parent marker depth
                                        parentMarkerDepth = markerData.depth;
                                    }
                                }
                                else
                                {
                                    // If we are now above or beside the parent marker then we are done for this level
                                    if (markerData.depth <= parentMarkerDepth)
                                    {
                                        parentMarkerDepth = -1;
                                    }
                                }

                                if (parentMarkerDepth < 0)
                                    continue;
                            }

                            MarkerData marker;
                            if (markers.ContainsKey(markerName))
                            {
                                marker = markers[markerName];
                            }
                            else
                            {
                                marker = new MarkerData(markerName);
                                marker.firstFrameIndex = frameIndex;
                                marker.minDepth = markerDepth;
                                marker.maxDepth = markerDepth;
                                analysis.AddMarker(marker);
                                markers.Add(markerName, marker);
                            }

                            marker.count += 1;
                            marker.msTotal += ms;

                            // Individual marker time (not total over frame)
                            if (ms < marker.msMinIndividual)
                            {
                                marker.msMinIndividual = ms;
                                marker.minIndividualFrameIndex = frameIndex;
                            }
                            if (ms > marker.msMaxIndividual)
                            {
                                marker.msMaxIndividual = ms;
                                marker.maxIndividualFrameIndex = frameIndex;
                            }

                            // Record highest depth foun
                            if (markerDepth < marker.minDepth)
                                marker.minDepth = markerDepth;
                            if (markerDepth > marker.maxDepth)
                                marker.maxDepth = markerDepth;

                            FrameTime frameTime;
                            if (frameIndex != marker.lastFrame)
                            {
                                marker.presentOnFrameCount += 1;
                                frameTime = new FrameTime(frameIndex, ms, 1);
                                marker.frames.Add(frameTime);
                                marker.lastFrame = frameIndex;
                            }
                            else
                            {
                                frameTime = marker.frames[marker.frames.Count - 1];
                                frameTime = new FrameTime(frameTime.frameIndex, frameTime.ms + ms, frameTime.count + 1);
                                marker.frames[marker.frames.Count - 1] = frameTime;
                            }
                        }

                        if (include)
                            thread.frames.Add(new ThreadFrameTime(frameIndex, msTimeOfMinDepthMarkers, msIdleTimeOfMinDepthMarkers));
                    }
                }

                at++;
                m_Progress = (100 * at) / frameCount;
            }

            analysis.GetFrameSummary().totalMarkers = allMarkers.Count;
            analysis.Finalise(timeScaleMax, maxMarkerDepthFound);

            /*
            foreach (int frameIndex in selectionIndices) 
            {
                int frameOffset = profileData.DisplayFrameToOffset(frameIndex);
                               
                var frameData = profileData.GetFrame(frameOffset);
                foreach (var threadData in frameData.threads)
                { 
                    var threadNameWithIndex = profileData.GetThreadName(threadData);

                    if (filterThreads && threadFilter != threadNameWithIndex)
                        continue;

                    const bool enterChildren = true;
                    foreach (var markerData in threadData.markers)
                    {
                        var markerName = markerData.name;
                        var ms = markerData.msFrame;
                        var markerDepth = markerData.depth;
                        if (depthFilter>=0 && markerDepth != depthFilter)
                            continue;

                        MarkerData marker = markers[markerName];
                        bucketIndex = (range > 0) ? (int)(((marker.buckets.Length-1) * (ms - first)) / range) : 0;
                        if (bucketIndex<0 || bucketIndex > (marker.buckets.Length - 1))
                        {
                            // This can happen if a single marker range is longer than the frame start end (which could occur if running on a separate thread)
                            // Debug.Log(string.Format("Marker {0} : {1}ms exceeds range {2}-{3} on frame {4}", marker.name, ms, first, last, frameIndex));
                            if (bucketIndex > (marker.buckets.Length - 1))
                                bucketIndex = (marker.buckets.Length - 1);
                            else
                                bucketIndex = 0;
                        }
                        marker.individualBuckets[bucketIndex] += 1;
                    }
                }
            }
*/
            m_Progress = 100;
            return analysis;
        }

        public int GetProgress()
        {
            return m_Progress;
        }
    }
}