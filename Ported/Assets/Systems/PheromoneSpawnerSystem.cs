using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class PheromoneSpawnerSystem : SystemBase
{
    public float pheromoneIntervalTime = 0.1f;
    private double lastIntervalTime = 0.0f;
    
    protected override void OnUpdate()
    {
        if (Time.ElapsedTime > lastIntervalTime + pheromoneIntervalTime)
        {
            lastIntervalTime = Time.ElapsedTime;
            //Entities
            //    .WithAll<Ant>()
        }
    }
}