using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityMath = Unity.Mathematics;

[UpdateAfter(typeof(DropPheromone))]
public partial class DecayPheromone : SystemBase
{
    protected override void OnUpdate()
    {
        var configuration = GetSingleton<Configuration>();
        var grid = GetSingleton<Grid2D>();
        var pheromone = GetBuffer<Pheromone>(GetSingletonEntity<Grid2D>());
        var deltaTime = Time.DeltaTime;
        Job.WithCode(() =>
        {
            for (int i = 0; i < pheromone.Length; i++)
            {
                var value = UnityMath.math.clamp(pheromone[i].Value - configuration.TrailDecay * deltaTime * pheromone[i].Value, 0f, 1f);
                pheromone[i] = new Pheromone() { Value = value };
            }
        }).Run();
    }
}