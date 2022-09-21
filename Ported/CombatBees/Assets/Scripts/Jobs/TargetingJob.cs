using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

[WithNone(typeof(TargetId))]
partial struct TargetingJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    
    public Random random;
    public float aggression;

    public NativeArray<Entity> enemies;
    public NativeArray<Entity> resources;

    void Execute(Entity bee)
    {
        if (random.NextFloat() < aggression && enemies.Length > 0)
        {
            // attacking
            ECB.AddComponent<TargetId>(bee, new TargetId() { Value = enemies[random.NextInt(0, enemies.Length)] });
        }
        else if (resources.Length > 0)
        {
            // gathering
            ECB.AddComponent<TargetId>(bee, new TargetId() { Value = resources[random.NextInt(0, resources.Length)] });
        }
    }
}