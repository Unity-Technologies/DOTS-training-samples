using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BotActionSystemEmptyBucket : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GameConfigComponent>();
        RequireSingletonForUpdate<HeatMapElement>();
    }

    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfigComponent>();
        var heatMapEntity = GetSingletonEntity<HeatMapElement>();
        var heatMap = GetBuffer<HeatMapElement>(heatMapEntity);
        var grid = gameConfig.SimulationSize;

        Entities
            .ForEach((ref BotActionEmptyBucket action, ref TargetLocationComponent targetLocation, ref HeatMapIndex targetFire, ref CarriedBucket carriedBucket,
                    in Translation trans) =>
                {
                    if (targetFire.index < 0 || targetFire.index >= heatMap.Length)
                        return;

                    if (carriedBucket.bucket == Entity.Null)
                        return;

                    if (!GetComponent<BucketFullComponent>(carriedBucket.bucket).full)
                        return;

                    Debug.Log("Thrower with a full bucket emptying it on fire cell " + targetFire.index);
                    var targetFireColumn = targetFire.index % grid;
                    var targetFireRow = targetFire.index / grid;
                    
                    for (var col = math.max(0,targetFireColumn - 1) ; col < math.min(targetFireColumn + 2, grid); ++col)
                    {
                        for (var row = math.max(0, targetFireRow - 1); row < math.min(targetFireRow + 2, grid); ++row)
                        {
                            var currentFireIndex = row * grid + col;
                            Debug.Log("Thrower with a full bucket emptying it on neighbour fire cell " + currentFireIndex);
                            heatMap[currentFireIndex] = new HeatMapElement() { temperature = 0 };
                        }
                    }

                    // Now mark the bucket as empty
                    SetComponent(carriedBucket.bucket, new BucketFullComponent(){ full = false });
                    SetComponent(carriedBucket.bucket, new BucketVolumeComponent() { Volume = 0F });
                    action.ActionDone = true;
                }
            ).Schedule();
    }
}