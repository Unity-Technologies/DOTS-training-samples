using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityMath = Unity.Mathematics;

[UpdateAfter(typeof(SpawnerSystem))]
public partial class DropPheromone : SystemBase
{
    protected override void OnUpdate()
    {
        var configuration = GetSingleton<Configuration>();
        var grid = GetSingleton<Grid2D>();
        var pheromone = GetBuffer<Pheromone>(GetSingletonEntity<Grid2D>());
        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<AntTag>()
            .WithAll<Velocity>()
            .WithAll<Translation>()
            .WithAll<Loadout>()
            .ForEach((Entity antEntity) => {
                var antVelocity = GetComponent<Velocity>(antEntity);
                var translation = GetComponent<Translation>(antEntity);
                var loadout = GetComponent<Loadout>(antEntity);

                if (translation.Value.x > 0.5f || translation.Value.x < -0.5f) return;
                if (translation.Value.y > 0.5f || translation.Value.y < -0.5f) return;

                var xIdx = (int)UnityMath.math.floor((0.5f + translation.Value.x) * grid.rowLength);
                var yIdx = (int)UnityMath.math.floor((0.5f + translation.Value.y) * grid.columnLength);
                var idx = xIdx + yIdx * grid.rowLength;
                var excitement = (loadout.Value > 0 ? 1f : .3f) * antVelocity.Speed / configuration.AntMaxSpeed;
                var value = UnityMath.math.clamp(configuration.TrailAddSpeed * deltaTime * excitement * (1f - pheromone[idx].Value), 0f, 1f);
                pheromone[idx] = new Pheromone() { Value = value };
            }).Run();
    }
}