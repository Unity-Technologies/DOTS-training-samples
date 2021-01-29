using Unity.Entities;

[UpdateInGroup( typeof(InitializationSystemGroup))]
public class PheromoneInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<PheromoneInit>();
        RequireSingletonForUpdate<Tuning>();
    }

    protected override void OnUpdate()
    {
        var pheromoneInitEntity = GetSingletonEntity<PheromoneInit>();
        var tuning = GetSingleton<Tuning>();

        var pheromoneEntity = EntityManager.CreateEntity(typeof(PheromoneStrength));
        var pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

        pheromoneBuffer.Capacity = tuning.PheromoneBuffer * tuning.PheromoneBuffer;
        pheromoneBuffer.Length = tuning.PheromoneBuffer * tuning.PheromoneBuffer;
        
        for (int i = 0; i < pheromoneBuffer.Length; i++)
        {
            pheromoneBuffer[i] = 0;
        }
        
        EntityManager.DestroyEntity(pheromoneInitEntity);
    }
}
