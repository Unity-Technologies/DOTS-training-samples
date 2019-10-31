using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

partial class CarSystem
{
    [BurstCompile]
    // TOOD: Proper grouping of the properties based on access type: Read-write or Read-only.
    private struct CarTransformJob : IJobForEachWithEntity<CarBasicState, LocalToWorld>
    {
        public float HighwayLen;
        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<HighwayPieceProperties> pieces;
        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<LocalToWorld> xforms;

        public void Execute(Entity entity, int index, [ReadOnly] ref CarBasicState carBasicState, ref LocalToWorld localToWorld)
        {
            float localX, localZ, localRot;
            int hitPiece = HighwayMathUtils.RoadPosToRelativePos(ref pieces,
                                    carBasicState.Position,
                                    carBasicState.Lane,
                                    out localX,
                                    out localZ,
                                    out localRot);

            float3 localPos = new float3(localX, localToWorld.Position.y, localZ);
            float3 piecePos = xforms[hitPiece].Position;
            localToWorld.Value = float4x4.TRS(localPos + piecePos,
                                        quaternion.RotateY(localRot),
                                        new float3(1, 1, 1));
        }
    }
}