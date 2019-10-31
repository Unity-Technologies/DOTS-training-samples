using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public struct HighwayMathUtils
{
    public float laneLength(float laneNum)
    {
        return 0;
    }

    /*
    public static void RoadPosToRelativePos(float distInLane, float laneNum, out float x, out float z, out float rotation)
    {

        // keep distance in [0, length)
        distInLane -= Mathf.Floor(distInLane / length(lane)) * length(lane);

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        float pieceStartDistance = 0;
        float pieceEndDistance = 0;
        x = 0;
        z = 0;
        rotation = 0;

        //GetEntityQuery(ComponentType.ReadOnly<CarBasicState>());

        for (int i = 0; i< 8; i++)
        {
            HighwayPiece piece = pieces[i];
            pieceStartDistance = pieceEndDistance;
            pieceEndDistance += piece.length(lane);
            if (distance >= pieceEndDistance)
                continue;

            // inside piece i

            // position and rotation local to the piece
            float localX, localZ;
            if (i % 2 == 0)
            {
                // straight piece
                GetStraightPiecePosition(distance - pieceStartDistance, lane, out localX, out localZ, out rotation);
            } else {
                // curved piece
                GetCurvePiecePosition(distance - pieceStartDistance, lane, out localX, out localZ, out rotation);
            }
            // transform
            RotateAroundOrigin(localX, localZ, piece.startRotation, out x, out z);
            x += piece.startX;
            z += piece.startZ;
            rotation += piece.startRotation;
            break;                
        }   
    }
*/
}