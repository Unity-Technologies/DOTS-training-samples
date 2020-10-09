using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TransformSystem))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Translation translation, in Direction direction, in Position position, in EntitySpeed entitySpeed) =>
        {
            switch (direction.Value)
            {
                case EntityDirection.Up:
                    translation.Value += new float3(0, 0, 1) * entitySpeed.speed * deltaTime;
                    break;
                case EntityDirection.Down:
                    translation.Value += new float3(0, 0, -1) * entitySpeed.speed * deltaTime;
                    break;
                case EntityDirection.Right:
                    translation.Value += new float3(1, 0, 0) * entitySpeed.speed * deltaTime;
                    break;
                case EntityDirection.Left:
                    translation.Value += new float3(-1, 0, 0) * entitySpeed.speed * deltaTime;
                    break;
            }
        }).Schedule();
    }
}
