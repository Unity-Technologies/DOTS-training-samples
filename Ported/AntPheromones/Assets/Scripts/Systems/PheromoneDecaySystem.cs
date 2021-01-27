using Unity.Entities;
using Unity.Mathematics;

public class PheromoneDecaySystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<PheromoneStrength>();
    }
    
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        var decayStrength = GetSingleton<Tuning>().PheromoneDecayStrength * time;


        Entity pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
        DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);
        
        Entities
            .WithAll<PheromoneStrength>()
            .ForEach((Entity entity) =>
            {
                for (int i = 0; i < pheromoneBuffer.Length; ++i)
                {
                    if (pheromoneBuffer[i].Value > 0)
                    {
                        var strength = (float)pheromoneBuffer[i].Value;
                        strength -= decayStrength;
                        strength = math.max(strength, 0);
                        pheromoneBuffer[i] = (byte)strength;
                    }   
                }
            }).Schedule();
    }
}
