using Unity.Collections;
using UnityEngine;

namespace Onboarding.BezierPath
{
    [CreateAssetMenu( menuName = "Gamove/Create PathData", fileName = "PathData")]
    [System.Serializable]
    public class PathData : ScriptableObject
    {
        public Vector3[] m_BezierControlPoints;
        public ApproximatedCurveSegment[] m_DistanceToParametric;
        public float PathLength;
        public float BiggestError;
        public int DataSizeApproximation;
    }
}