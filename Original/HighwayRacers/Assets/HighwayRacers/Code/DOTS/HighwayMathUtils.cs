using System;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct HighwayMathUtils
{
    public static float laneLength(float lane0Length, float laneNum)
    {
        return lane0Length + (4 * HighwayConstants.LANE_SPACING * laneNum * Mathf.PI * 0.5f);
    }

    public static void RotateAroundOrigin(float x, float z, float rotation, out float xOut, out float zOut)
    {
        float sin = Mathf.Sin(-rotation);
        float cos = Mathf.Cos(-rotation);

        xOut = x * cos - z * sin;
        zOut = x * sin + z * cos;
    }

    public static int RoadPosToRelativePos(ref NativeArray<HighwayPieceProperties> pieces, float highWayLen, float distInLane, float laneNum, out float x, out float z, out float rotation)
    {
        // keep distance in [0, length)
        distInLane -= Mathf.Floor(distInLane / laneLength(highWayLen, laneNum)) * laneLength(highWayLen, laneNum);

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
            if ((distInLane >= pieceEndDistance) && (i < 7))
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
                float radius = pieces[i].length / (Mathf.PI * 0.5f) + laneNum * HighwayConstants.LANE_SPACING;
                float angle = (distInLane - pieceStartDistance) / radius;
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