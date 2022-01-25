using UnityEngine;
using System.Linq;
using System.Threading;

namespace Onboarding.BezierPath
{
    public class BakeSplinesMultithreaded
    {
        // ----------------------------------------------------------------------------------------
        private static void CallActionOnAllContextsThreadPooled(PathBakingHelper.BakingContext[] contexts, System.Action<PathBakingHelper.BakingContext> action)
        {
            int bezierSegmentCount = contexts.Length;

            // Start threads
            int remainingJobs = bezierSegmentCount;
            for (int i = 0; i < bezierSegmentCount; ++i)
            {
                int index = i;
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (obj) =>
                    {
                        action(contexts[index]);
                        Interlocked.Decrement(ref remainingJobs);
                    }));
            }

            // Wait for all jobs to end
            while (remainingJobs != 0)
                Thread.Sleep(10);
        }

        // ----------------------------------------------------------------------------------------
        private static void CallActionOnAllContextsSingleThreaded(PathBakingHelper.BakingContext[] contexts, System.Action<PathBakingHelper.BakingContext> action)
        {
            int bezierSegmentCount = contexts.Length;
            for (int i = 0; i < bezierSegmentCount; ++i)
            {
                action(contexts[i]);
            }
        }

        // ----------------------------------------------------------------------------------------
        private static void PreBakingProcess(PathBaker baker)
        {
            baker.m_PathData.PathLength = 0;
            baker.m_PathData.BiggestError = 0;
        }

        // ----------------------------------------------------------------------------------------
        public static void BakingProcess(PathBaker baker, int bezierSegmentCount, bool disableThreadPools = false)
        {
            float startTime = Time.realtimeSinceStartup;

            // Bake in the Bezier control points
            baker.m_PathData.m_BezierControlPoints = baker.m_ControlPoints.Select(go => go.transform.position).ToArray();

            Debug.Log($"Starting Baking process for : {baker.name} ({bezierSegmentCount} Bezier segments)");

            PathBakingHelper.BakingContext[] contexts = PrepareAllBakingContexts(baker, bezierSegmentCount);

            {
                PreBakingProcess(baker);
                GenerateAllSamples(contexts, disableThreadPools);
                ComputeAllBezierSegmentStartDistances(contexts);
                PerformArcLengthReparameterization(contexts, disableThreadPools);
                PostBakingProcess(baker, contexts);
            }

            Debug.Log($"Finished Baking process in {Time.realtimeSinceStartup - startTime}");
        }

        // ----------------------------------------------------------------------------------------
        private static PathBakingHelper.BakingContext[] PrepareAllBakingContexts(PathBaker baker, int bezierSegmentCount)
        {
            PathBakingHelper.BakingContext[] contexts = new PathBakingHelper.BakingContext[bezierSegmentCount];
            for (int i = 0; i < bezierSegmentCount; ++i)
            {
                contexts[i] = PathBakingHelper.BakingContext.CreateContext(baker);
                contexts[i].Initialize(i);
            }

            return contexts;
        }

        

        // ----------------------------------------------------------------------------------------
        private static void GenerateAllSamples(PathBakingHelper.BakingContext[] contexts, bool disableThreadPools)
        {
            if (disableThreadPools)
                CallActionOnAllContextsSingleThreaded(contexts, PathBakingHelper.SampleDistances);
            else
                CallActionOnAllContextsThreadPooled(contexts, PathBakingHelper.SampleDistances);
        }

        // ----------------------------------------------------------------------------------------
        private static void ComputeAllBezierSegmentStartDistances(PathBakingHelper.BakingContext[] contexts)
        {
            float length = 0;
            for (int i = 0; i < contexts.Length; ++i)
            {
                contexts[i].startingDistance = length;
                length += contexts[i].splineLength;
            }
        }

        // ----------------------------------------------------------------------------------------
        private static void PerformArcLengthReparameterization(PathBakingHelper.BakingContext[] contexts, bool disableThreadPools)
        {
            if (disableThreadPools)
                CallActionOnAllContextsSingleThreaded(contexts, PathBakingHelper.FitSamplesWithBezier1DCurves);
            else
                CallActionOnAllContextsThreadPooled(contexts, PathBakingHelper.FitSamplesWithBezier1DCurves);
        }

        // ----------------------------------------------------------------------------------------
        private static void PostBakingProcess(PathBaker baker, PathBakingHelper.BakingContext[] contexts)
        {
            for (int i = 0; i < contexts.Length; ++i)
            {
                baker.m_PathData.PathLength += contexts[i].splineLength;
            }
            baker.m_PathData.m_DistanceToParametric = contexts.SelectMany(c => c.outputFitCurves).ToArray();
            baker.m_PathData.DataSizeApproximation = baker.m_PathData.m_BezierControlPoints.Length * 3 * 4;     // 3 floats
            baker.m_PathData.DataSizeApproximation += baker.m_PathData.m_DistanceToParametric.Length * 7 * 4;   // 7 floats
        }
    }
}