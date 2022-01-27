using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityMath = Unity.Mathematics;

[UpdateAfter(typeof(AntMoveSystem))]
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

                var xIdx = (int)UnityMath.math.floor(UnityMath.math.clamp(0.5f + translation.Value.x, 0, 1) * grid.rowLength);
                var yIdx = (int)UnityMath.math.floor(UnityMath.math.clamp(0.5f + translation.Value.y, 0, 1) * grid.columnLength);
                var idx = UnityMath.math.min(xIdx + yIdx * grid.rowLength, pheromone.Length -1);
                var excitement = (loadout.Value > 0 ? 1f : .3f) * antVelocity.Speed / configuration.AntMaxSpeed;
                var value = pheromone[idx].Value + UnityMath.math.clamp(configuration.TrailAddSpeed * deltaTime * excitement * (1f - pheromone[idx].Value), 0f, 1f);
                pheromone[idx] = new Pheromone() { Value = value };
            }).Run();
    }
}