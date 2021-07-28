using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WaterSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
    }
    
    protected override void OnUpdate()
    {
        var gameConfig = this.GetSingleton<GameConfigComponent>();
        var refillRate = gameConfig.WaterRefillRate;
        var maxScale = gameConfig.WaterMaxScale;
        
        Entities
            .WithAll<WaterTagComponent>()
            .ForEach((ref WaterVolumeComponent volume, ref NonUniformScale scale, in WaterCapacityComponent capacity) =>
            {
                if (volume.Volume < capacity.Capacity)
                {
                    volume.Volume += refillRate;
                }

                var scaleRatio = volume.Volume / capacity.Capacity;
                scale.Value.y = scaleRatio * maxScale;
            }).Schedule();
    }
}
