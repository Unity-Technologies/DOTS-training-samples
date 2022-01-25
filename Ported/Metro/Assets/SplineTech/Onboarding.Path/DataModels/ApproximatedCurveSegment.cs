using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding.BezierPath
{
    [System.Serializable]
    public struct ApproximatedCurveSegment
    {
        public float p0, p1, p2, p3;        // the 4 control points of the 1D bezier
        public float start;                 // validity range within the approximated curve
        public float end;                   
        public int bezierIndex;             // corresponding bezier curve in the global path
    }
}