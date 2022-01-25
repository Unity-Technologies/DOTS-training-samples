using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Onboarding.BezierPath
{
    [BurstCompile(FloatMode = FloatMode.Fast)]
    public struct ComputeFastBezierEquationsJob : IJobParallelFor
    {
        [ReadOnly] 
        public NativeArray<Vector3> controlPoints;
        
        // output

        public NativeArray<BezierBakingJob.FastBezierEquation> fastBezierComponents;

        public void Execute(int i)
        {
            int offsetIn = i * 3;

            Vector3 p0 = controlPoints[offsetIn + 0];
            Vector3 p1 = controlPoints[offsetIn + 1];
            Vector3 p2 = controlPoints[offsetIn + 2];
            Vector3 p3 = controlPoints[offsetIn + 3];

            // Develop the Bezier equation so interpolation is faster later on
            //   P0(1-t)³ + 3P1t(1-t)² + 3P2t²(1-t) + P3t³
            // = P0(1-3t+3t²-t³) + 3P1t(t²-2t+1) + 3P2(t²-t³) + P3t³
            // = P0-3P0t+3P0t²-P0t³ + 3P1t³-6P1t²+3P1t + 3P2t²-3P2t³ + P3t³
            // = P0 + t(-3P0+3P1) + t²(3P0-6P1+3P2) + t³(-P0+3P1-3P2+P3)
            int offsetOut = i * 4;
            fastBezierComponents[i] = new BezierBakingJob.FastBezierEquation
            {
                polynomialA = p0,
                polynomialB = -3 * p0 + 3 * p1,
                polynomialC = 3 * p0 - 6 * p1 + 3 * p2,
                polynomialD = -p0 + 3 * p1 - 3 * p2 + p3
            };
        }
    }
}
