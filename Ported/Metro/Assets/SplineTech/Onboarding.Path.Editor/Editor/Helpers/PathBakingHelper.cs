using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Onboarding.BezierPath
{
    public class PathBakingHelper
    {
        public class BakingContext : IDisposable
        {
            public PathBaker                        baker;
            public PathData                         pathData            { get { return baker.m_PathData; } }
            public int                              bezierIndex;
            public List<ApproximatedCurveSegment>   outputFitCurves;
            public Vector2[]                        samples;
            public Vector3[]                        worldSpaceSamples;
            public float                            splineLength;
            public Vector3                          fastBezierP0;
            public Vector3                          fastBezierP1;
            public Vector3                          fastBezierP2;
            public Vector3                          fastBezierP3;
            public float                            startingDistance;

            // ----------------------------------------------------------------------------------------
            internal void Initialize(int _bezierIndex)
            {
                bezierIndex = _bezierIndex;
                splineLength = 0;
                samples = null;

                // Develop the Bezier equation
                int offset = _bezierIndex * 3;
                Vector3 p0 = pathData.m_BezierControlPoints[offset + 0];
                Vector3 p1 = pathData.m_BezierControlPoints[offset + 1];
                Vector3 p2 = pathData.m_BezierControlPoints[offset + 2];
                Vector3 p3 = pathData.m_BezierControlPoints[offset + 3];

                //   P0(1-t)³ + 3P1t(1-t)² + 3P2t²(1-t) + P3t³
                // = P0(1-3t+3t²-t³) + 3P1t(t²-2t+1) + 3P2(t²-t³) + P3t³
                // = P0-3P0t+3P0t²-P0t³ + 3P1t³-6P1t²+3P1t + 3P2t²-3P2t³ + P3t³
                // = P0 + t(-3P0+3P1) + t²(3P0-6P1+3P2) + t³(-P0+3P1-3P2+P3)
                fastBezierP0 = p0;
                fastBezierP1 = -3 * p0 + 3 * p1;
                fastBezierP2 = 3 * p0 - 6 * p1 + 3 * p2;
                fastBezierP3 = -p0 + 3 * p1 - 3 * p2 + p3;
            }

            public void FastBezierSample(float t, ref Vector3 point)
            {
                point.x = fastBezierP0.x + t * (fastBezierP1.x + t * (fastBezierP2.x + fastBezierP3.x * t));
                point.y = fastBezierP0.y + t * (fastBezierP1.y + t * (fastBezierP2.y + fastBezierP3.y * t));
                point.z = fastBezierP0.z + t * (fastBezierP1.z + t * (fastBezierP2.z + fastBezierP3.z * t));
            }

            // ----------------------------------------------------------------------------------------
            private BakingContext() {}

            // ----------------------------------------------------------------------------------------
            internal static BakingContext CreateContext(PathBaker _baker)
            {
                return new BakingContext {  baker = _baker,
                                            outputFitCurves = new List<ApproximatedCurveSegment>() };
            }

            // ----------------------------------------------------------------------------------------
            public void Dispose()
            {
                baker = null;
                bezierIndex = 0;
                outputFitCurves = null;
                samples = null;
                splineLength = 0;
            }
        }

        // ----------------------------------------------------------------------------------------
        public static void SampleDistances(BakingContext context)
        {
            //int bezierIndex, float precision, PathData data, out Vector2[] tForArcDistance, out float totalLength{
            float precision = context.baker.m_SamplingDistance;

            List<Vector2> tempSamples = new List<Vector2>();
            List<Vector3> tempWorldSamples = new List<Vector3>();
            float last_t = 0;
            float t = 0;
            float precisionSquared = precision * precision;
            double totalDistance = 0;
            tempSamples.Add(Vector2.zero);
            PathController.InterpolatePosition(context.pathData, context.bezierIndex, 0, out var lastPos);
            tempWorldSamples.Add(lastPos);
            while (t != 1)
            {
                // try to go too far
                while (t < 1)
                {
                    PathController.InterpolatePosition(context.pathData, context.bezierIndex, t, out var sample);
                    float distance = (sample - lastPos).sqrMagnitude;
                    if (distance > precisionSquared)
                        break;
                    last_t = t;
                    t += 0.001f;
                    if (t > 1) t = 1;
                }

                if (t == 1)
                {
                    PathController.InterpolatePosition(context.pathData, context.bezierIndex, 1, out var sample);
                    totalDistance += (sample - lastPos).magnitude;
                    break;
                }

                // Dichotomy 
                float tmax = t;
                float tmin = last_t;
                do
                {
                    t = tmin + (tmax - tmin) * 0.5f;
                    PathController.InterpolatePosition(context.pathData, context.bezierIndex, t, out var sample);
                    float distance = (sample - lastPos).sqrMagnitude;
                    if (Mathf.Abs(distance - precisionSquared) < 0.00001f || (tmax - tmin < 0.00001f))
                    {
                        // close enough
                        totalDistance += Mathf.Sqrt(distance);
                        tempSamples.Add(new Vector2((float)totalDistance, t));
                        tempWorldSamples.Add(sample);
                        lastPos = sample;
                        break;
                    }
                    else if (distance > precisionSquared)
                    {
                        // at 't' we are too far. Reduce the range to [min,t]
                        tmax = t;
                    }
                    else if (distance < precisionSquared)
                    {
                        // at 't' we are too short. Reduce the range to [t, max]
                        tmin = t;
                    }
                } while (true);
            }
            context.splineLength = (float)totalDistance;

            // Normalize all values
            for (int i = 0; i < tempSamples.Count; ++i)
                tempSamples[i] = new Vector2(tempSamples[i].x / context.splineLength, tempSamples[i].y);

            // Add last test sample at 1,1
            tempSamples.Add(new Vector2(1, 1));

            // Add last world space point, for t=1
            PathController.InterpolatePosition(context.pathData, context.bezierIndex, 1, out var lastsample);
            tempWorldSamples.Add(lastsample);

            // Fill in context
            context.samples = tempSamples.ToArray();
            context.worldSpaceSamples = tempWorldSamples.ToArray();
            Debug.Log($"segment #{context.bezierIndex} has {context.samples.Length} entries, and a length of {context.splineLength}");

            BezierCurveFitting.LeastSquareFit_FastBezier(context.samples, 0, context.samples.Length-1, out var p0, out var p1, out var p2, out var p3);
            ApproximatedCurveSegment tempCurve = new ApproximatedCurveSegment();
            tempCurve.p0 = p0;
            tempCurve.p1 = p1;
            tempCurve.p2 = p2;
            tempCurve.p3 = p3;
            tempCurve.start = 0;
            tempCurve.end = 0;
            var biggestError = ComputeBiggestError(context, 0, context.samples.Length - 1, ref tempCurve);
            Debug.Log($"Global Fit {p0} {p1} {p2} {p3}");

        }

        // ----------------------------------------------------------------------------------------
        public static void FitSamplesWithBezier1DCurves(BakingContext context)
        {
            var data = context.baker.m_PathData;

            ApproximatedCurveSegment tempCurve = new ApproximatedCurveSegment();
            tempCurve.bezierIndex = context.bezierIndex;

            int startIndex = 0;
            for (int i = 0; i < context.samples.Length; ++i)
            {
                int endIndex = i;

                if (endIndex - startIndex + 1 < 4)
                    continue;

                tempCurve.start = context.startingDistance + context.splineLength * context.samples[startIndex].x;
                tempCurve.end = context.startingDistance + context.splineLength * context.samples[endIndex].x;
                BezierCurveFitting.LeastSquareFit_FastBezier(context.samples, startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);

                float biggestError = 0;
                biggestError = ComputeBiggestError(context, startIndex, endIndex, ref tempCurve );

                if (biggestError > context.baker.m_ErrorThreshold)
                {
                    int remainingPointsAfterCut = context.samples.Length - (i - 1);
                    if (remainingPointsAfterCut > 0 && remainingPointsAfterCut < 4)
                    {
                        // if we split at this position, we won't have enough for another curve before we run out of samples
                        // we can grab some points from the previous curve though!
                        int currentCurveTotalPoints = i - startIndex;
                        int requiredPoints = 4 - remainingPointsAfterCut;
                        if (currentCurveTotalPoints - requiredPoints < 4)
                        {
                            // No luck, we have to live with that error, and merge the current curve with the remainder of the points
                            context.baker.m_PathData.BiggestError = Math.Max(biggestError, context.baker.m_PathData.BiggestError);
                            endIndex = context.samples.Length - 1;
                            tempCurve.start = context.startingDistance + context.splineLength * context.samples[startIndex].x;
                            tempCurve.end = context.startingDistance + context.splineLength * context.samples[endIndex].x;
                            BezierCurveFitting.LeastSquareFit_FastBezier(context.samples, startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
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
                            tempCurve.start = context.startingDistance + context.splineLength * context.samples[startIndex].x;
                            tempCurve.end = context.startingDistance + context.splineLength * context.samples[endIndex].x;
                            BezierCurveFitting.LeastSquareFit_FastBezier(context.samples, startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
                            context.outputFitCurves.Add(tempCurve);
                            startIndex = endIndex;

                            // Curve two, up to the end of the data set
                            endIndex = context.samples.Length - 1;
                            tempCurve.start = context.startingDistance + context.splineLength * context.samples[startIndex].x;
                            tempCurve.end = context.startingDistance + context.splineLength * context.samples[endIndex].x;
                            BezierCurveFitting.LeastSquareFit_FastBezier(context.samples, startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
                            startIndex = endIndex;
                            // no adding to the curves list here, as it's done at the end of the loop
                            break;
                        }
                    }
                    else
                    {
                        // Cut the curve at where we were last loop
                        endIndex = i - 1;
                        tempCurve.start = context.startingDistance + context.splineLength * context.samples[startIndex].x;
                        tempCurve.end = context.startingDistance + context.splineLength * context.samples[endIndex].x;
                        BezierCurveFitting.LeastSquareFit_FastBezier(context.samples, startIndex, endIndex, out tempCurve.p0, out tempCurve.p1, out tempCurve.p2, out tempCurve.p3);
                        context.outputFitCurves.Add(tempCurve);
                        startIndex = endIndex;
                    }
                }
                else
                {
                    context.baker.m_PathData.BiggestError = Math.Max(biggestError, context.baker.m_PathData.BiggestError);
                }
            }

            // There is always a curve in the pipe, make sure we commit it in the list
            context.outputFitCurves.Add(tempCurve);
        }

        // ----------------------------------------------------------------------------------------
        private static float ComputeBiggestError(BakingContext context, int startIndex, int endIndex, ref ApproximatedCurveSegment tempCurve)
        {
            float biggestError = 0;
            float startDistance = context.samples[startIndex].x;
            float endDistance = context.samples[endIndex].x;
            float range = endDistance - startDistance;
            Vector3 erroneousPos = Vector3.zero;
            for (int j = startIndex; j <= endIndex; ++j)
            {
                float t = (context.samples[j].x - startDistance) / range;
                float val = PathController.InterpolateBezier1D(tempCurve, t);

                context.FastBezierSample(val, ref erroneousPos);
                //PathController.InterpolatePosition(context.baker.m_PathData, context.bezierIndex, val, out erroneousPos);

                float dx = context.worldSpaceSamples[j].x - erroneousPos.x;
                float dy = context.worldSpaceSamples[j].y - erroneousPos.y;
                float dz = context.worldSpaceSamples[j].z - erroneousPos.z;

                float error = dx* dx + dy* dy + dz* dz;
                if (biggestError < error)
                    biggestError = error;
            }

            return Mathf.Sqrt(biggestError);
        }
    }
}
