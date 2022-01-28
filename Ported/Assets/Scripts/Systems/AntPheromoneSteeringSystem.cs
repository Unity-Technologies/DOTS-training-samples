using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(SteeringSystem))]
public partial class AntPheromoneSteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var pheromoneBuffer = GetBuffer<Pheromone>(GetSingletonEntity<Grid2D>());
        var grid = GetSingleton<Grid2D>();
        var configuration = GetSingleton<Configuration>();
        var castDistance = 3.0f / configuration.MapSize;
        
        Entities
            .WithAll<AntTag>()
            .WithReadOnly(pheromoneBuffer)
            .ForEach((ref PheromoneSteering steering, in Translation antTranslation, in Rotation antRotation) =>
            {
                var antPosition = antTranslation.Value.xy + 0.5f;
                var antOrientation = math.mul(antRotation.Value, new float3(0, 0, 1));
                steering.Value = float2.zero;
            
                // Sample left and right to find pheromones
                var sideOrientation = new float2(-antOrientation.y, antOrientation.x);
                var sideVector = sideOrientation * castDistance;
                var xIdx = (int)math.floor((antPosition.x + sideVector.x) * grid.rowLength);
                var yIdx = (int)math.floor((antPosition.y + sideVector.y) * grid.columnLength);
                if (xIdx >= 0 && xIdx < grid.rowLength && yIdx >= 0 && yIdx < grid.columnLength)
                {
                    var pheromone = pheromoneBuffer[xIdx + yIdx * grid.rowLength].Value;
                    steering.Value += pheromone * sideOrientation;
                }

                sideVector = -sideVector;
                xIdx = (int)math.floor((antPosition.x + sideVector.x) * grid.rowLength);
                yIdx = (int)math.floor((antPosition.y + sideVector.y) * grid.columnLength);
                if (xIdx >= 0 && xIdx < grid.rowLength && yIdx >= 0 && yIdx < grid.columnLength)
                {
                    var pheromone = pheromoneBuffer[xIdx + yIdx * grid.rowLength].Value;
                    steering.Value += pheromone * -sideOrientation;
                }
            }).ScheduleParallel();
    }
}
