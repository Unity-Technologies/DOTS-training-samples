using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class BrigadeUpdateSystem : SystemBase
{
    private float updateSpeed = 2.0f;
    private float nextUpdate = 0;
    private Random random;
    protected override void OnCreate()
    {
        random = new Random(1);
    }

    protected override void OnUpdate()
    {

        // move the bots towards their targets
        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<TargetPosition>()
            .ForEach((ref Translation translation, in TargetPosition target) =>
            {
                translation.Value =
                    translation.Value + math.normalize(target.Value - translation.Value) * 1 * deltaTime;
            }).Schedule();

        // do an occasional target update
        nextUpdate -= Time.DeltaTime;
        if (nextUpdate > 0)
        {
            return;
        }
        nextUpdate = updateSpeed;

        // build the brigade lookup
        // this should really be done once
        var BrigadeDataLookup = new NativeHashMap<Entity, Brigade>(16, Allocator.TempJob);
        Entities
            .WithAll<Brigade>()
            .ForEach((Entity e, in Brigade brigade) => { BrigadeDataLookup.Add(e, brigade); })
            .Schedule();
        
        // just randomly updating the targets
        // will need to be replaced by the search
        Entities.WithAll<Brigade>().ForEach((ref Brigade brigade) =>
        {
            brigade.fireTarget = brigade.random.NextFloat3() * 10.0f;
            brigade.fireTarget.y = 0;
            brigade.waterTarget = brigade.random.NextFloat3() * 10.0f;
            brigade.waterTarget.y = 0;
        }).Schedule();
        
        Entities
            .WithAll<TargetPosition>()
            .ForEach((Entity e, ref TargetPosition target, in BrigadeGroup group, in EmptyPasserInfo passerInfo) =>
            {
                var brigadeData = BrigadeDataLookup[@group.Value];
                target.Value = BrigadeInitializationSystem.GetChainPosition(passerInfo.ChainPosition, passerInfo.ChainLength, brigadeData.waterTarget, brigadeData.fireTarget);
            }).Schedule();
        
        Entities
            .WithAll<TargetPosition>()
            .WithDisposeOnCompletion(BrigadeDataLookup)
            .ForEach((Entity e, ref TargetPosition target, in BrigadeGroup group, in FullPasserInfo passerInfo) =>
            {
                var brigadeData = BrigadeDataLookup[@group.Value];
                target.Value = BrigadeInitializationSystem.GetChainPosition(passerInfo.ChainPosition, passerInfo.ChainLength, brigadeData.fireTarget, brigadeData.waterTarget);
            }).Schedule();
    }
}