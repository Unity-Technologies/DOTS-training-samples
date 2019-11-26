using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup((typeof(InitializationSystemGroup)))]
[UpdateAfter(typeof(AntSpawningSystem))]
[UpdateBefore(typeof(AntPostInitializationSystem))]
public class AntInitializationSystem : JobComponentSystem
{
    EntityQuery m_MapSettingsQuery;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        uint seed = 1 + (uint)UnityEngine.Time.frameCount;
        var mapSize = m_MapSettingsQuery.GetSingleton<MapSettingsComponent>().MapSize;
        return Entities.WithAll<UninitializedTagComponent>().ForEach((Entity entity, ref BrightnessComponent brightness, ref FacingAngleComponent facingAngle, ref PositionComponent position, ref RandomSteeringComponent random) =>
        {
            var rng = new Random(((uint)entity.Index + 1) * seed * 100151);
            facingAngle.Value = rng.NextFloat() * 2 * math.PI;
            brightness.Value = rng.NextFloat(0.75f, 1.25f);
            position.Value = .5f * mapSize + new float2(rng.NextFloat(-5, 5), rng.NextFloat(-5, 5));
            random.Rng = rng;
        }).Schedule(inputDeps);
    }
}
