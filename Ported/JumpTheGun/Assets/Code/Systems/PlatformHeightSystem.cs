
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class PlatformHeightSystem : SystemBase
{
    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(MinMaxHeight));
        RequireForUpdate(query);

        GetEntityQuery(typeof(OffsetList));
    }
    
    protected override void OnUpdate()
    {
        var boardEntity = GetSingletonEntity<Board>();
        
        float2 minMaxHeight = GetComponent<MinMaxHeight>(boardEntity).Value;
        float hitStrength = GetComponent<HitStrength>(boardEntity).Value;
        var boardSize = GetComponent<BoardSize>(boardEntity).Value;
        var offsets = GetBuffer<OffsetList>(boardEntity).AsNativeArray();
       
        Entities
            .WithAll<Platform>()
            .WithNativeDisableParallelForRestriction(offsets)
            .ForEach((ref LocalToWorld xform, ref URPMaterialPropertyBaseColor baseColor, ref WasHit hit, in BoardPosition boardPosition) =>
            {
                if (hit.Count <= 0)
                {
                    return;
                }

                //Debug.Log("boardPosition: " + boardPosition.Value.x + " " + boardPosition.Value.y);
                
                var index = CoordUtils.ToIndex(boardPosition.Value, boardSize.x, boardSize.y); 
                var offset = math.max(offsets[index].Value - hitStrength * hit.Count, minMaxHeight.x);

                offsets[index] = new OffsetList {Value = offset};
                
                xform.Value = math.mul(
                    math.mul(
                        float4x4.Translate(new float3(boardPosition.Value.x, -0.5f, boardPosition.Value.y)),
                        float4x4.Scale(1f, offset, 1f)),
                    float4x4.Translate(new float3(0f, 0.5f, 0f)));
                
                baseColor.Value = Colorize.Platform(offset, minMaxHeight.x, minMaxHeight.y);

                // Reset hit count
                hit.Count = 0;
            })
            .ScheduleParallel();
    }
}
