using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Onboarding.BezierPath
{
    [BurstCompile]
    public struct BezierBakingJob : IJob
    {
        public struct FastBezierEquation 
        {
            public Vector3 polynomialA;
            public Vector3 polynomialB;
            public Vector3 polynomialC;
            public Vector3 polynomialD;

            public void InterpolatePositionFast(float t, out Vector3 position)
            {
                position.x = polynomialA.x + t * (polynomialB.x + t * (polynomialC.x + polynomialD.x * t));
                position.y = polynomialA.y + t * (polynomialB.y + t * (polynomialC.y + polynomialD.y * t));
                position.z = polynomialA.z + t * (polynomialB.z + t * (polynomialC.z + polynomialD.z * t));
            }
        }

        public struct Sample
        {
            public float t;
            public float arcLength; // in percentage of the bezier segment length
            public Vector3 position;
        }

        public void Execute()
        {
        }
    }
}
