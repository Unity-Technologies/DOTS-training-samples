using Unity.Entities;
using UnityEngine;

[ExecuteAlways]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public class ObstacleDebugDrawSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref ObstacleSet obstacles) =>
        {
            var radius = obstacles.blob.Value.radius;
            var positions = obstacles.blob.Value.positions;

            for (int i = 0; i < positions.Length; ++i)
            {
                var position = positions[i];
                Gizmos.DrawWireSphere(position, radius);
            }
        });
    }
}