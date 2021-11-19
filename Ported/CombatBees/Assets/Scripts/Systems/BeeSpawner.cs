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

        var sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();

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
            .ForEach((Entity hiveEntity, in AABB aabb, in TeamID teamID) =>
            {
                for (int i = 0; i < totalBees; ++i)
                {
                    var entity = EntityManager.Instantiate(globalData.BeePrefab);
                    SetBees(entity, ecb, teamID);

                    // Move bee to hive location
                    ecb.SetComponent<Translation>(entity, new Translation { Value = aabb.center });
                }
            }).Run();
        sys.AddJobHandleForProducer(Dependency);

        this.Enabled = false;
    }

    public static void SetBees(Entity entity, EntityCommandBuffer myecb, TeamID teamID)
    {
        var random = new Random(1234);
        var vel = math.normalize(random.NextFloat3Direction()) * 10.0f;
        myecb.AddComponent(entity, new Velocity { Value = vel });
        myecb.AddComponent(entity, new Bee());
        myecb.AddComponent(entity, new BeeIdleMode());
        myecb.AddComponent(entity, teamID);
        myecb.AddComponent(entity, new TargetedBy { Value = Entity.Null });
        myecb.AddComponent(entity, new Flutter());
        myecb.AddComponent(entity, new NonUniformScale { Value = new float3(1.0f) });

        // Set bee color based off the teamID
        URPMaterialPropertyBaseColor color = new URPMaterialPropertyBaseColor();
        if (teamID.Value == 0) // We are Yellow Bees
        {
            color.Value.x = 1;
            color.Value.y = 1;
            color.Value.z = 0;
        }
        else // we are Blue Bees
        {
            color.Value.x = 0;
            color.Value.y = 0;
            color.Value.z = 1;
        }
        color.Value.w = 1;
        myecb.SetComponent<URPMaterialPropertyBaseColor>(entity, color);
    }
}
