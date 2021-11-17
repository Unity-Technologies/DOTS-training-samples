using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class BeeSpawner : SystemBase
{
    public int totalBees = 10;
    
    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<Globals>();
    }

    // May run before scene is loaded
    protected override void OnUpdate()
    {
        var globals = GetSingletonEntity<Globals>();
        var globalsComponent = GetComponent<Globals>(globals);

        var random = new Random(1234);
        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, in Spawner spawnComponent, in TeamID teamID) => 
        {
            for (int i = 0; i < spawnComponent.Count; ++i)
            {
                var spawnedBee = EntityManager.Instantiate(globalsComponent.BeePrefab);

                var vel = math.normalize(random.NextFloat3Direction());
                EntityManager.SetComponentData<Velocity>(spawnedBee, new Velocity { Value = vel });
                EntityManager.SetComponentData<Translation>(spawnedBee, new Translation { Value = spawnComponent.SpawnPosition });
                EntityManager.SetComponentData<Bee>(spawnedBee, new Bee { Mode = Bee.ModeCategory.Searching });
                EntityManager.SetComponentData<Goal>(spawnedBee, new Goal { target = new float3(0) });
                EntityManager.SetComponentData<TeamID>(spawnedBee, new TeamID { Value = teamID.Value });

                if (teamID.Value == 0)
                {
                    EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(spawnedBee, new URPMaterialPropertyBaseColor { Value = new float4(0, 0, 1, 1)});
                }
                else
                {
                    EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(spawnedBee, new URPMaterialPropertyBaseColor { Value = new float4(1, 1, 0, 1)});
                }
            }

            EntityManager.DestroyEntity(entity);
        }).Run();
    }
}
