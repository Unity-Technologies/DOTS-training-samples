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
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();

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
            .ForEach((Entity hiveEntity, int entityInQueryIndex, in AABB aabb, in TeamID teamID) =>
            {
                var random = Random.CreateFromIndex((uint)entityInQueryIndex);
                for (int i = 0; i < totalBees; ++i)
                {
                    var entity = ecb.Instantiate(entityInQueryIndex, globalData.BeePrefab);
                    var vel = SetBees(entity, entityInQueryIndex, ecb, teamID, ref random);

                    // Move bee to hive location
                    ecb.SetComponent<Translation>(entityInQueryIndex, entity, new Translation
                        { Value = aabb.center + vel*0.25f });
                }
            }).Run();
        sys.AddJobHandleForProducer(Dependency);

        this.Enabled = false;
    }

    public static float3 SetBees(Entity entity, int entityIndex, EntityCommandBuffer.ParallelWriter myecb, TeamID teamID, ref Random random)
    {
        var vel = math.normalize(random.NextFloat3Direction()) * random.NextFloat(1, 10);
        myecb.AddComponent(entityIndex, entity, new Velocity { Value = vel });
        myecb.AddComponent(entityIndex, entity, teamID);

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
        myecb.SetComponent<URPMaterialPropertyBaseColor>(entityIndex, entity, color);

        return vel;
    }
}
