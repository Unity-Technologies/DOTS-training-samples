using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Onboarding.BezierPath
{
    [BurstCompile(FloatMode = FloatMode.Fast)]
    public struct ComputeSegmentSamplesJob : IJob
    {
        [ReadOnly]
        public NativeArray<float> segmentsLengths;
        [ReadOnly]
        public NativeArray<int> samplecounts;
        [ReadOnly] 
        public NativeArray<BezierBakingJob.FastBezierEquation> fastBezierComponents;
        [ReadOnly]
        public float samplingDistance;
        [ReadOnly]
        public int segmentIndex;

        public NativeArray<BezierBakingJob.Sample> samples;

        public void Execute()
        {
            float last_t = 0;
            float t = 0;
            float samplingDistanceSquared = samplingDistance * samplingDistance;
            double totalDistance = 0;

            int sampleCount = 0;
            fastBezierComponents[segmentIndex].InterpolatePositionFast(0, out var lastPos);
            samples[sampleCount] = new BezierBakingJob.Sample { arcLength = 0, t = 0, position = lastPos };
            sampleCount++;

            while (t != 1)
            {
                // try to go past target
                while (t < 1)
                {
                    fastBezierComponents[segmentIndex].InterpolatePositionFast(t, out var sample);
                    float distance = (sample - lastPos).sqrMagnitude;
                    if (distance > samplingDistanceSquared)
                        break;
                    last_t = t;
                    t += 0.001f;
                    if (t > 1) t = 1;
                }

                if (t == 1)
                {
                    fastBezierComponents[segmentIndex].InterpolatePositionFast(1, out var sample);
                    totalDistance += (sample - lastPos).magnitude;
                    break;
                }

                // Dichotomy 
                float tmax = t;
                float tmin = last_t;
                do
                {
                    t = tmin + (tmax - tmin) * 0.5f;
                    fastBezierComponents[segmentIndex].InterpolatePositionFast(t, out var sample);

                    float distance = (sample - lastPos).sqrMagnitude;
                    if (Mathf.Abs(distance - samplingDistanceSquared) < 0.00001f || (tmax - tmin < 0.00001f))
                    {
                        // close enough
                        totalDistance += Mathf.Sqrt(distance);
                        samples[sampleCount] = new BezierBakingJob.Sample
                        {
                            position = sample,
                            t = t,
                            arcLength = (float)totalDistance
                        };
                        sampleCount++;

                        lastPos = sample;
                        
                        break;
                    }
                    else if (distance > samplingDistanceSquared)
                    {
                        // at 't' we are too far. Reduce the range to [min,t]
                        tmax = t;
                    }
                    else if (distance < samplingDistanceSquared)
                    {
                        // at 't' we are too short. Reduce the range to [t, max]
                        tmin = t;
                    }
                } while (true);
            }

            // Normalize all values
            for (int i = 0; i < sampleCount; ++i)
            {
                samples[i] = new BezierBakingJob.Sample
                {
                    position = samples[i].position,
                    t = samples[i].t,
                    arcLength = samples[i].arcLength / (float)totalDistance
                };
            }
            
            // Add last test sample at 1,1
            fastBezierComponents[segmentIndex].InterpolatePositionFast(1, out var lastsample);
            samples[sampleCount] = new BezierBakingJob.Sample
            {
                position = lastsample,
                t = 1,
                arcLength = 1
            };

            sampleCount++;

            Debug.Assert(sampleCount == samples.Length);
        }

        public void AllocateSamples()
        {
            samples = new NativeArray<BezierBakingJob.Sample>(samplecounts[segmentIndex], Allocator.TempJob);
        }
    }
}
