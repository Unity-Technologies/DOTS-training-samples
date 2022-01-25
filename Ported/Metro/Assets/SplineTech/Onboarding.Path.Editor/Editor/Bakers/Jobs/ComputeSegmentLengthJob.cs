using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Onboarding.BezierPath
{
    [BurstCompile(FloatMode = FloatMode.Fast)]
    public struct ComputeSegmentLengthJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<BezierBakingJob.FastBezierEquation> fastBezierComponents;

        public NativeArray<float> lengths;
        public NativeArray<int> samplecounts;

        [ReadOnly]
        public float samplingDistance;

        public void Execute(int index)
        {
            float last_t = 0;
            float t = 0;
            float samplingDistanceSquared = samplingDistance * samplingDistance;
            double totalDistance = 0;

            int sampleCount = 0;
            fastBezierComponents[index].InterpolatePositionFast(0, out var lastPos);
            sampleCount++;

            while (t != 1)
            {
                // try to go past target
                while (t < 1)
                {
                    fastBezierComponents[index].InterpolatePositionFast(t, out var sample);
                    float distance = (sample - lastPos).sqrMagnitude;
                    if (distance > samplingDistanceSquared)
                        break;
                    last_t = t;
                    t += 0.001f;
                    if (t > 1) t = 1;
                }

                if (t == 1)
                {
                    fastBezierComponents[index].InterpolatePositionFast(1, out var sample);
                    totalDistance += (sample - lastPos).magnitude;
                    break;
                }

                // Dichotomy 
                float tmax = t;
                float tmin = last_t;
                do
                {
                    t = tmin + (tmax - tmin) * 0.5f;
                    fastBezierComponents[index].InterpolatePositionFast(t, out var sample);

                    float distance = (sample - lastPos).sqrMagnitude;
                    if (Mathf.Abs(distance - samplingDistanceSquared) < 0.00001f || (tmax - tmin < 0.00001f))
                    {
                        // close enough

                        // Count one more point
                        sampleCount++;

                        lastPos = sample;
                        totalDistance += Mathf.Sqrt(distance);
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

            // Add one more point for the last point on the spline, which is not sampled with the
            // code above
            sampleCount++;

            // Store results
            samplecounts[index] = sampleCount;
            lengths[index] = (float)totalDistance;
        }
    }
}
