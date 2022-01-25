using UnityEngine;
using System.Linq;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;

namespace Onboarding.BezierPath
{
    public class BakeSplinesJobified
    {
        // ----------------------------------------------------------------------------------------
        public static void BakingProcess(PathBaker baker, int bezierSegmentCount)
        {
            float startTime = Time.realtimeSinceStartup;

            // Bake in the Bezier control points
            baker.m_PathData.m_BezierControlPoints = baker.m_ControlPoints.Select(go => go.transform.position).ToArray();

            Debug.Log($"Starting Baking process for : {baker.name} ({bezierSegmentCount} Bezier segments)");

            // First allocate memory arrays to read / compute in the jobs
            NativeArray<Vector3> controlPoints = new NativeArray<Vector3>(baker.m_PathData.m_BezierControlPoints, Allocator.TempJob);
            NativeArray<BezierBakingJob.FastBezierEquation> fastBezierComponents = new NativeArray<BezierBakingJob.FastBezierEquation>(bezierSegmentCount, Allocator.TempJob);
            NativeArray<float> segmentsLengths = new NativeArray<float>(bezierSegmentCount, Allocator.TempJob);
            NativeArray<float> totalLength = new NativeArray<float>(1, Allocator.TempJob);
            NativeArray<int> samplecounts = new NativeArray<int>(bezierSegmentCount, Allocator.TempJob);
            NativeArray<int> paramCurvesCounts = new NativeArray<int>(bezierSegmentCount, Allocator.TempJob);
            NativeArray<float> paramCurvesErrors = new NativeArray<float>(bezierSegmentCount, Allocator.TempJob);

            // Create the jobs dependency chain
            // --------------------------------

            // convert bezier control points list into developped polynomial form
            ComputeFastBezierEquationsJob ComputeFastBezierEquationsJob = new ComputeFastBezierEquationsJob { 
                controlPoints = controlPoints, 
                fastBezierComponents = fastBezierComponents };
            var ComputeFastBezierEquationsJobHandle = ComputeFastBezierEquationsJob.Schedule(bezierSegmentCount, 1);

            // compute all spline segments lengths
            ComputeSegmentLengthJob computeSegmentLengthsJob = new ComputeSegmentLengthJob { 
                fastBezierComponents = fastBezierComponents,
                samplingDistance = baker.m_SamplingDistance,
                samplecounts = samplecounts,
                lengths = segmentsLengths };
            var computeSegmentLengthsJobHandle = computeSegmentLengthsJob.Schedule(bezierSegmentCount, 1, ComputeFastBezierEquationsJobHandle);

            // compute global length of the spline
            ComputeTotalLengthJob computeTotalLengthJob = new ComputeTotalLengthJob { 
                segmentsLengths = segmentsLengths, 
                totalLength = totalLength };
            var computeTotalLengthJobHandle = computeTotalLengthJob.Schedule(computeSegmentLengthsJobHandle);

            // Synch point as we need the lengths computed properly to allocate sample counts
            computeTotalLengthJobHandle.Complete();
            Debug.Log($"Finished Computing Lengths at t={Time.realtimeSinceStartup - startTime}. Length {totalLength[0]}");

            // Create uniform samples along each bezier spline 
            ComputeSegmentSamplesJob[] samplingJobs = new ComputeSegmentSamplesJob[bezierSegmentCount];
            NativeArray<JobHandle> samplingJobHandles = new NativeArray<JobHandle>(bezierSegmentCount, Allocator.TempJob);
            for (int i = 0; i < samplingJobs.Length; ++i)
            {
                samplingJobs[i] = new ComputeSegmentSamplesJob
                {
                    segmentsLengths = segmentsLengths,
                    fastBezierComponents = fastBezierComponents,
                    segmentIndex = i,
                    samplecounts = samplecounts,
                    samplingDistance = baker.m_SamplingDistance,
                };
                samplingJobs[i].AllocateSamples();
                samplingJobHandles[i] = samplingJobs[i].Schedule();
            }

            // Perform compression of the arc parameterization curves via 1d Bezier curves
            CompressArcLengthParamCurveJob[] computeArcLengthParamJobs = new CompressArcLengthParamCurveJob[bezierSegmentCount];
            NativeArray<JobHandle> computeArcLengthParamJobHandles = new NativeArray<JobHandle>(bezierSegmentCount, Allocator.TempJob);
            {
                // while going through al segments, we will compute the start / end distance, from the origin of the spline, for each
                // segment
                float currentDistance = 0;
                for (int i = 0; i < computeArcLengthParamJobs.Length; ++i)
                {
                    float endDistance = currentDistance + segmentsLengths[i];
                    computeArcLengthParamJobs[i] = new CompressArcLengthParamCurveJob
                    {
                        samples = samplingJobs[i].samples,
                        errorThreshold = baker.m_ErrorThreshold,
                        segmentIndex = i,
                        startDistance = currentDistance,
                        endDistance = endDistance,
                        length = segmentsLengths[i],
                        fastBezierComponents = fastBezierComponents,
                        paramCurvesCounts = paramCurvesCounts,
                        paramCurvesErrors = paramCurvesErrors
                    };
                    computeArcLengthParamJobs[i].AllocateSamples();
                    computeArcLengthParamJobHandles[i] = computeArcLengthParamJobs[i].Schedule(samplingJobHandles[i]);

                    currentDistance = endDistance;
                }
            }

            JobHandle allParameterizationJobsHandle = JobHandle.CombineDependencies(computeArcLengthParamJobHandles);

            // This is the root Job calling everything
            BezierBakingJob bakingJob = new BezierBakingJob();
            JobHandle BakingJobHandle = bakingJob.Schedule(allParameterizationJobsHandle);
            BakingJobHandle.Complete();


            // Collect the result
            List<ApproximatedCurveSegment> allSegments = new List<ApproximatedCurveSegment>();
            foreach (var job in computeArcLengthParamJobs)
            {
                int count = paramCurvesCounts[job.segmentIndex];
                for (int i = 0; i < count; ++i)
                {
                    allSegments.Add(job.parameterizationCurves[i]);
                    if (job.parameterizationCurves[i].end == job.endDistance)
                        break;
                }
            }
            baker.m_PathData.PathLength = 0;
            baker.m_PathData.BiggestError = 0;

            foreach (var job in computeArcLengthParamJobs)
            {
                baker.m_PathData.BiggestError = Mathf.Max(baker.m_PathData.BiggestError, job.paramCurvesErrors[job.segmentIndex]);
                baker.m_PathData.PathLength += job.length;
            }


            baker.m_PathData.m_DistanceToParametric = allSegments.ToArray();
            baker.m_PathData.DataSizeApproximation = baker.m_PathData.m_BezierControlPoints.Length * 3 * 4;     // 3 floats
            baker.m_PathData.DataSizeApproximation += baker.m_PathData.m_DistanceToParametric.Length * 7 * 4;   // 7 floats

            Debug.Log($"Finished Baking process in {Time.realtimeSinceStartup - startTime}. Length {totalLength[0]}");
            Debug.Log($"Generated curves : {allSegments.Count}");
            
            // Need to dispose all native arrays so we don't get a memory leak
            controlPoints.Dispose();
            segmentsLengths.Dispose();
            totalLength.Dispose();
            fastBezierComponents.Dispose();
            samplingJobHandles.Dispose();
            computeArcLengthParamJobHandles.Dispose();
            samplecounts.Dispose();
            paramCurvesCounts.Dispose();
            paramCurvesErrors.Dispose();

            foreach (var job in samplingJobs)
                job.samples.Dispose();

            foreach (var job in computeArcLengthParamJobs)
                job.parameterizationCurves.Dispose();
        }
    }
}