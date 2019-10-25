using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{
    public class CurvePiece : HighwayPiece
    {
        public override float length(float lane)
        {
            return (curveRadiusLane0 + lane * Highway.LANE_SPACING) * Mathf.PI / 2;
        }

        public override float curveRadiusLane0 { get { return Highway.instance.CURVE_LANE0_RADIUS; } }
    }

}
