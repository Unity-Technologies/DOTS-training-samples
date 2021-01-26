using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacersOldCode
{

    public class CurvePiece : HighwayPiece
    {

        public override float length(float lane)
        {
            return Highway.curvePieceLength(lane);
        }

    }

}