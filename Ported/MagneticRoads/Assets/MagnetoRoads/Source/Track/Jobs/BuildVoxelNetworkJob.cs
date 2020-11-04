using Magneto.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Magneto.Track.Jobs
{
    public struct BuildVoxelNetworkJob : IJob
    {
        [ReadOnly] public NativeList<IntersectionData> R_Intersections;
        [ReadOnly] public NativeStrideGridArray<int> R_IntersectionsGrid;
        [ReadOnly] public NativeArray<int3> R_LimitedCachedNeighbourIndexOffsets;
        [ReadOnly] public NativeStrideGridArray<bool> R_TrackVoxels;

        [WriteOnly] public NativeList<SplineData> W_OutSplines;

        public void Execute()
            //public void Execute(int index)
        {
            var count = R_Intersections.Length;
            for (var index = 0; index < count; index++)
            {
                // Get specific data
                var intersectionData = R_Intersections[index];
                var axesWithNeighbors = int3.zero;

                for (var j = 0; j < TrackManager.LIMITED_DIRECTIONS_LENGTH; j++)
                {
                    if (GetVoxel(intersectionData.Index + R_LimitedCachedNeighbourIndexOffsets[j], false))
                    {
                        axesWithNeighbors.x += Mathf.Abs(R_LimitedCachedNeighbourIndexOffsets[j].x);
                        axesWithNeighbors.y += Mathf.Abs(R_LimitedCachedNeighbourIndexOffsets[j].y);
                        axesWithNeighbors.z += Mathf.Abs(R_LimitedCachedNeighbourIndexOffsets[j].z);

                        var intersectionFirst = FindFirstIntersection(intersectionData.Index,
                            R_LimitedCachedNeighbourIndexOffsets[j], out var connectionDirection);

                        if (intersectionFirst == -1) continue;

                        var neighbourData = R_Intersections[intersectionFirst];

                        if (neighbourData.ListIndex != -1 && neighbourData.ListIndex != intersectionData.ListIndex)
                        {
                            W_OutSplines.Add(new SplineData
                            {
                                StartPosition = intersectionData.Position,
                                StartNormal = intersectionData.Normal,
                                StartTangent = R_LimitedCachedNeighbourIndexOffsets[j],
                                EndPosition = neighbourData.Position,
                                EndNormal = neighbourData.Normal,
                                EndTangent = connectionDirection
                            });
                        }
                    }
                }

                // TODO : SOmething?
                // // find this intersection's normal - it's the one axis
                // // along which we have no neighbors
                // for (int j=0;j<3;j++) {
                //     if (axesWithNeighbors[j]==0) {
                //         if (intersectionData.Normal.Equals(int3.zero)) {
                //             intersectionData.Normal = int3.zero;
                //             intersectionData.Normal[j] = -1+Random.Range(0,2)*2;
                //             //Debug.DrawRay(intersection.position,(Vector3)intersection.normal * .5f,Color.red,1000f);
                //         } else {
                //             Debug.LogError("a straight line has been marked as an intersection!");
                //         }
                //     }
                // }
            }
        }

        private bool GetVoxel(int3 position, bool outOfBoundsReturns = true)
        {
            if (position.x >= 0 && position.x < TrackManager.VOXEL_COUNT &&
                position.y >= 0 && position.y < TrackManager.VOXEL_COUNT &&
                position.z >= 0 && position.z < TrackManager.VOXEL_COUNT)
            {
                return R_TrackVoxels[position.x, position.y, position.z];
            }

            return outOfBoundsReturns;
        }

        // TODO: this will need to return the index
        private int FindFirstIntersection(int3 pos, int3 dir, out int3 otherDirection)
        {
            // step along our voxel paths (before splines have been spawned),
            // starting at one intersection, and stopping when we reach another intersection
            while (true)
            {
                pos += dir;
                if (R_IntersectionsGrid[pos.x, pos.y, pos.z] != -1)
                {
                    otherDirection = dir * -1;
                    return R_IntersectionsGrid[pos.x, pos.y, pos.z];
                }

                if (GetVoxel(pos + dir, false) == false)
                {
                    var foundTurn = false;
                    for (var i = 0; i < TrackManager.LIMITED_DIRECTIONS_LENGTH; i++)
                    {
                        if (!R_LimitedCachedNeighbourIndexOffsets[i].Equals(dir) &&
                            !R_LimitedCachedNeighbourIndexOffsets[i].Equals(dir * -1))
                        {
                            if (GetVoxel(pos + R_LimitedCachedNeighbourIndexOffsets[i], false))
                            {
                                dir = R_LimitedCachedNeighbourIndexOffsets[i];
                                foundTurn = true;
                                break;
                            }
                        }
                    }

                    if (foundTurn == false)
                    {
                        // dead end
                        otherDirection = int3.zero;
                        return -1;
                    }
                }
            }
        }

        // long HashIntersectionPair(IntersectionData a, IntersectionData b) {
        //     // pack two intersections' IDs into one int64
        //     int id1 = a.ListIndex;
        //     int id2 = b.ListIndex;
        //
        //     return ((long)Mathf.Min(id1,id2) << 32) + Mathf.Max(id1,id2);
        // }
    }
}