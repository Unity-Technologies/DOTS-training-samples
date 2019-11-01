using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial class CarSystem
{
    [BurstCompile]
    // TOOD: Proper grouping of the properties based on access type: Read-write or Read-only.
    private struct CarTransformJob : IJobForEachWithEntity<CarBasicState, CarReadOnlyProperties, CarColor, LocalToWorld>
    {
        public float HighwayLen;
        public Color baseColor;
        public Color fastColor;
        public Color slowColor;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<HighwayPieceProperties> pieces;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<LocalToWorld> xforms;

        public void Execute(Entity entity, int index, [ReadOnly] ref CarBasicState carBasicState, [ReadOnly] ref CarReadOnlyProperties crop, ref CarColor ccol, ref LocalToWorld localToWorld)
        {
            float localX, localZ, localRot;
            int hitPiece = HighwayMathUtils.RoadPosToRelativePos(ref pieces,
                                    HighwayLen,
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
            Color curColor = baseColor;
            if (carBasicState.Speed < crop.DefaultSpeed)
            {
                float lerpFac = carBasicState.Speed / crop.DefaultSpeed;
                curColor = baseColor * lerpFac + slowColor * (1 - lerpFac);
            }
            else if (carBasicState.Speed > crop.DefaultSpeed)
            {
                float lerpFac = (carBasicState.Speed - crop.DefaultSpeed) / (crop.MaxSpeed - crop.DefaultSpeed);
                curColor = fastColor * lerpFac + baseColor * (1 - lerpFac);
            }
            ccol.Color = new float4(curColor.r, curColor.g, curColor.b, curColor.a);
        }
    }
}