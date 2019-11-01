using System;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct HighwayMathUtils
{
    public static void RotateAroundOrigin(float x, float z, float rotation, out float xOut, out float zOut)
    {
        float sin = Mathf.Sin(-rotation);
        float cos = Mathf.Cos(-rotation);

        xOut = x * cos - z * sin;
        zOut = x * sin + z * cos;
    }

    public static int RoadPosToRelativePos(ref NativeArray<HighwayPieceProperties> pieces, float highWayLen, float distInLane, float laneNum, out float x, out float z, out float rotation)
    {
        // Convert to lane0 because all piece lengths are calculated for lane0.
        distInLane = Utilities.ConvertPositionToLane(distInLane, laneNum, 0, highWayLen);

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        float pieceStartDistance = 0;
        float pieceEndDistance = 0;
        x = 0;
        z = 0;
        rotation = 0;

        int pieceIdx = 0;

        for (int i = 0; i < 8; i++)
        {
            pieceStartDistance = pieceEndDistance;
            pieceEndDistance += pieces[i].length;
            if (distInLane >= pieceEndDistance)
                continue;

            // inside piece i
            pieceIdx = i;
            float localX, localZ;

            if (pieces[i].isStraight)
            {
                localX = HighwayConstants.LANE_SPACING * ((HighwayConstants.NUM_LANES - 1) / 2f - laneNum);
                localZ = distInLane - pieceStartDistance;
                rotation = 0;
            }
            else
            {
                float radius = Utilities.CurvePieceRadius(laneNum);
                float angle = (distInLane - pieceStartDistance) / HighwayConstants.CURVE_LANE0_RADIUS;
                localX = HighwayConstants.MID_RADIUS - Mathf.Cos(angle) * radius;
                localZ = Mathf.Sin(angle) * radius;
                rotation = angle;
            }

            RotateAroundOrigin(localX, localZ, pieces[i].startRotation, out x, out z);
            rotation += pieces[i].startRotation;
            break;                
        }
        return pieceIdx;
    }

}