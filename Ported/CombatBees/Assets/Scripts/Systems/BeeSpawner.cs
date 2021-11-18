using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class BeeSpawner : SystemBase
{
    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<GlobalData>();
    }
   
    // May run before scene is loaded
    protected override void OnUpdate()
    {
        var globalDataEntity = GetSingletonEntity<GlobalData>();
        var globalData = GetComponent<GlobalData>(globalDataEntity);
        var beeDefinitions = GetBuffer<TeamDefinition>(globalDataEntity);

        var totalBees = globalData.BeeCount;

        var random = new Random(1234);

        Entities
            .WithAll<HiveTag>()
            .ForEach((Entity hiveEntity, in TeamID teamID) =>
            {
                var def = beeDefinitions[teamID.Value];
                def.hive = hiveEntity;
                beeDefinitions[teamID.Value] = def;
            }).Run();

        Entities
            .WithAll<HiveTag>()
            .WithStructuralChanges()
            .ForEach((Entity hiveEntity, in AABB aabb, in TeamID teamID, in URPMaterialPropertyBaseColor urpColor) =>
            {
                for (int i = 0; i < totalBees; ++i)
                {
                    var entity = EntityManager.Instantiate(globalData.BeePrefab);

                    var vel = math.normalize(random.NextFloat3Direction())*10.0f;
                    // Optimize by Setting the velocity instead of adding.
                    EntityManager.AddComponentData(entity, new Velocity { Value = vel });
                    EntityManager.AddComponentData(entity, new Bee());
                    EntityManager.AddComponentData(entity, new BeeIdleMode());
                    EntityManager.AddComponentData(entity, teamID);
                    EntityManager.AddComponentData(entity, new TargetedBy { Value = Entity.Null });
                    EntityManager.AddComponentData(entity, new Flutter());
                    EntityManager.AddComponentData(entity, new NonUniformScale { Value = new float3(1.0f) });

                    // Move bee to hive location
                    EntityManager.SetComponentData<Translation>(entity, new Translation { Value = aabb.center });
                    // Set bee color based off the hive
                    URPMaterialPropertyBaseColor color = urpColor;
                    color.Value.w = 1;
                    EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(entity, color);

                }
            }).Run();

        this.Enabled = false;
    }
}
