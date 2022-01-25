using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Onboarding.BezierPath
{
    [BurstCompile(FloatMode = FloatMode.Fast)]
    public struct CompressArcLengthParamCurveJob : IJob
    {
        [ReadOnly]
        public NativeArray<BezierBakingJob.FastBezierEquation> fastBezierComponents;
        [ReadOnly]
        public NativeArray<BezierBakingJob.Sample> samples;
        [ReadOnly]
        public int segmentIndex;
        [ReadOnly]
        public float startDistance;
        [ReadOnly]
        public float endDistance;
        [ReadOnly]
        public float length;
        [ReadOnly]
        public float errorThreshold;

        public NativeArray<ApproximatedCurveSegment> parameterizationCurves;
        public NativeArray<int> paramCurvesCounts;
        public NativeArray<float> paramCurvesErrors;

        public float biggestError;
        public int curvesCount;

        public void Execute()
        {
            ApproximatedCurveSegment tempCurve = new ApproximatedCurveSegment();
            tempCurve.bezierIndex = segmentIndex;

            int startIndex = 0;
            for (int i = 0; i < samples.Length; ++i)
            {
                int endIndex = i;

                if (endIndex - startIndex + 1 < 4)
                    continue;

                tempCurve.start = startDistance + length * samples[startIndex].arcLength;
                tempCurve.end = startDistance + length * samples[endIndex].arcLength;
                LeastSquareFit_FastBezier(startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);

                float curveBiggestError = 0;
                curveBiggestError = ComputeBiggestError(startIndex, endIndex, ref tempCurve);

                if (curveBiggestError > errorThreshold)
                {
                    int remainingPointsAfterCut = samples.Length - (i - 1);
                    if (remainingPointsAfterCut > 0 && remainingPointsAfterCut < 4)
                    {
                        // if we split at this position, we won't have enough for another curve before we run out of samples
                        // we can grab some points from the previous curve though!
                        int currentCurveTotalPoints = i - startIndex;
                        int requiredPoints = 4 - remainingPointsAfterCut;
                        if (currentCurveTotalPoints - requiredPoints < 4)
                        {
                            // No luck, we have to live with that error, and merge the current curve with the remainder of the points
                            biggestError = Math.Max(curveBiggestError, biggestError);
                            endIndex = samples.Length - 1;
                            tempCurve.start = startDistance + length * samples[startIndex].arcLength;
                            tempCurve.end = startDistance + length * samples[endIndex].arcLength;
                            LeastSquareFit_FastBezier(startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
                            startIndex = endIndex;
                            // no adding to the curves list here, as it's done at the end of the loop
                            break;
                        }
                        else
                        {
                            // We are going to generate another curve anyway, to finishing fitting the sample array
                            // so we better make use of that to get more quality out of it... 
                            // Curve one
                            endIndex = i - 1 - currentCurveTotalPoints / 2;
                            tempCurve.start = startDistance + length * samples[startIndex].arcLength;
                            tempCurve.end = startDistance + length * samples[endIndex].arcLength;
                            LeastSquareFit_FastBezier(startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
                            parameterizationCurves[curvesCount++] = tempCurve;
                            startIndex = endIndex;

                            // Curve two, up to the end of the data set
                            endIndex = samples.Length - 1;
                            tempCurve.start = startDistance + length * samples[startIndex].arcLength;
                            tempCurve.end = startDistance + length * samples[endIndex].arcLength;
                            LeastSquareFit_FastBezier(startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
                            startIndex = endIndex;
                            // no adding to the curves list here, as it's done at the end of the loop
                            break;
                        }
                    }
                    else
                    {
                        // Cut the curve at where we were last loop
                        endIndex = i - 1;
                        tempCurve.start = startDistance + length * samples[startIndex].arcLength;
                        tempCurve.end = startDistance + length * samples[endIndex].arcLength;
                        LeastSquareFit_FastBezier(startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
                        parameterizationCurves[curvesCount++] = tempCurve;
                        startIndex = endIndex;
                    }
                }
                else
                {
                    biggestError = Math.Max(curveBiggestError, biggestError);
                }
            }

            // There is always a curve in the pipe, make sure we commit it in the list
            parameterizationCurves[curvesCount++] = tempCurve;
            paramCurvesErrors[segmentIndex] = biggestError;
            paramCurvesCounts[segmentIndex] = curvesCount;
        }

        public void AllocateSamples()
        {
            const int MAX_CURVES = 64;
            parameterizationCurves = new NativeArray<ApproximatedCurveSegment>(MAX_CURVES, Allocator.TempJob);
            curvesCount = 0;
            biggestError = 0;
        }

        /// <summary>
        /// Non Linear Least square fitting of a dataset with a bezier spline
        /// WARNING: THE OUTPUT is not the 4 control points of a regular bezier equation
        /// but instead 4 constants to be used with the DEVELOPED version of the bezier equation !!!!
        /// f(t) = (dt³ + ct² + bt + a)
        /// 
        /// In our use case, the curve initial guess for P0, and P1 is perfect, as we know
        /// the curve passes by these points
        /// Also, we know that curve is continuous and the function will converge.
        /// Thus, this function is not intended for general purpose curve fitting, it assumes all the above, 
        /// as well as at least 4 points are passed in
        /// 
        /// (P1 & P2) are obtained by partially differentiating the sum of square distance
        /// between each sample 'i' and the parametric curve with respect to P1 and P2 at the corresponding 't'
        /// and then solving for the two unknowns P1 and  P2.
        /// 
        /// </summary>
        // --------------------------------------------------------------------------------------------
        public void LeastSquareFit_FastBezier(int start, int end, out float a, out float b, out float c, out float d)
        {

            double distStart = samples[start].arcLength;
            double distRange = samples[end].arcLength - distStart;

            // P0 and P1 are set, since we already know where they land
            double P0asDouble = samples[start].t;
            double P3asDouble = samples[end].t;

            // 
            double SumOfB1Squared = 0, SumOfB2Squared = 0, C1 = 0, C2 = 0, SumOfB1TimesB2 = 0;
            for (int i = start; i <= end; ++i)
            {
                double t = (samples[i].arcLength - distStart) / distRange;
                double t2 = t * t;
                double t3 = t2 * t;
                double t4 = t3 * t;
                double _1_t = 1 - t;
                double _1_t2 = _1_t * _1_t;
                double _1_t3 = _1_t2 * _1_t;
                double _1_t4 = _1_t3 * _1_t;

                // Bezier : 
                //      f(t) = P0(1-t)³ + 3P1t(1-t)² + 3P2t²(1-t) + P3t³
                // so
                //      f(t) = P0*B0 + P1*B1 + P2*B2 + P3*B3
                // with :
                //      B0 = (1-t)³ 
                //      B1 = 3t(1-t)²
                //      B2 = 3t²(1-t)
                //      B3 = t³

                // Update sum of B1 et B2 for the P1 and P2
                SumOfB1Squared += 9 * t2 * _1_t4; // += B1² => 9t²(1-t)^4
                SumOfB2Squared += 9 * t4 * _1_t2; // += B2² => 9t^4(1-t)²
                SumOfB1TimesB2 += 9 * t3 * _1_t3; // += B1xB2 => 9t³(1-t)³

                double partialResidual = (samples[i].t - _1_t3 * P0asDouble - t3 * P3asDouble);
                // C1 = C1 + B1*( sample[i] - B0*P0 - B3*P3 );
                C1 += 3 * t * _1_t2 * partialResidual;

                // C2 = C2 + B2*( sample[i] - B0*P0 - B3*P3 );
                C2 += 3 * t2 * _1_t * partialResidual;
            }

            // the easy ones we already know
            a = (float)P0asDouble;
            d = (float)P3asDouble;

            // Solve for the other 2
            double denom = (SumOfB1Squared * SumOfB2Squared - SumOfB1TimesB2 * SumOfB1TimesB2);
            b = (float)((SumOfB2Squared * C1 - SumOfB1TimesB2 * C2) / denom);
            c = (float)((SumOfB1Squared * C2 - SumOfB1TimesB2 * C1) / denom);

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // NOT PART OF THE BASIC LEAST SQUARE ALGORITHM
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Develop the Bezier equation, so it's faster at runtime to consume

            //   a(1-t)³ + 3bt(1-t)² + 3ct²(1-t) + dt³
            // = a(1-3t+3t²-t³) + 3bt(t²-2t+1) + 3c(t²-t³) + dt³
            // = a-3at+3at²-at³ + 3bt³-6bt²+3bt + 3ct²-3ct³ + dt³
            // = a + t(-3a+3b) + t²(3a-6b+3c) + t³(-a+3b-3c+d)
            float dP1 = -3 * a + 3 * b;
            float dP2 = 3 * a - 6 * b + 3 * c;
            float dP3 = -a + 3 * b - 3 * c + d;

            b = dP1;
            c = dP2;
            d = dP3;
        }

        private float ComputeBiggestError(int startIndex, int endIndex, ref ApproximatedCurveSegment tempCurve)
        {
            float biggestError = 0;
            float startDistance = samples[startIndex].arcLength;
            float endDistance = samples[endIndex].arcLength;
            float range = endDistance - startDistance;
            var segmentEquation = fastBezierComponents[segmentIndex];
            for (int j = startIndex; j <= endIndex; ++j)
            {
                float t = (samples[j].arcLength - startDistance) / range;
                float val = PathController.InterpolateBezier1D(tempCurve, t);

                segmentEquation.InterpolatePositionFast(val, out var erroneousPos);

                float dx = samples[j].position.x - erroneousPos.x;
                float dy = samples[j].position.y - erroneousPos.y;
                float dz = samples[j].position.z - erroneousPos.z;

                float error = dx * dx + dy * dy + dz * dz;
                if (biggestError < error)
                    biggestError = error;
            }

            return Mathf.Sqrt(biggestError);
        }
    }
}
