using System;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct HighwayMathUtils
{
    public static float laneLength(float laneMidLength, float laneNum)
    {
        return laneMidLength - HighwayConstants.LANE_SPACING * (HighwayConstants.NUM_LANES / 2 - laneNum);
    }

    public static void RotateAroundOrigin(float x, float z, float rotation, out float xOut, out float zOut)
    {
        float sin = Mathf.Sin(-rotation);
        float cos = Mathf.Cos(-rotation);

        xOut = x * cos - z * sin;
        zOut = x * sin + z * cos;
    }

    public static int RoadPosToRelativePos(ref NativeArray<HighwayPieceProperties> pieces, float distInLane, float laneNum, out float x, out float z, out float rotation)
    {
        // keep distance in [0, length)
        distInLane -= Mathf.Floor(distInLane / laneLength(360, laneNum)) * laneLength(360, laneNum);

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        float pieceStartDistance = 0;
        float pieceEndDistance = 0;
        x = 0;
        z = 0;
        rotation = 0;

        int pieceIdx = 0;

        for (int i = 0; i< 8; i++)
        {
            pieceStartDistance = pieceEndDistance;
            pieceEndDistance += pieces[i].length;
            if ((distInLane >= pieceEndDistance) || (distInLane < pieceStartDistance))
                continue;

            // inside piece i
            pieceIdx = i;
            float localX, localZ;

            if (pieces[i].isStraight)
            {
                localX = HighwayConstants.LANE_SPACING * ((HighwayConstants.NUM_LANES - 1) / 2f - laneNum);
                localZ = distInLane - pieceStartDistance;
                rotation = pieces[i].startRotation;
            }
            else
            {
                float radius = HighwayConstants.CURVE_LANE0_RADIUS + laneNum * HighwayConstants.LANE_SPACING;
                float angle = (distInLane - pieceStartDistance) / radius;
                localX = HighwayConstants.MID_RADIUS - Mathf.Cos(angle) * radius;
                localZ = Mathf.Sin(angle) * radius;
                rotation = angle;
            }

            RotateAroundOrigin(localX, localZ, pieces[i].startRotation, out x, out z);
            rotation += pieces[i].startRotation;

            /*
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
            */
            break;                
        }
        return pieceIdx;
    }

}