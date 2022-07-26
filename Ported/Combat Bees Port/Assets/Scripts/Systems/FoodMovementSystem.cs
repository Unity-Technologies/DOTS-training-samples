using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial class FoodMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        float3 gravity = new float3(0f, -9.81f, 0f);

        Entities
            .WithAll<Food>()
            .ForEach((ref Translation translation, in Food food) =>
            {
                if (HasComponent<LocalToWorld>(food.target))
                {
                    var targetPos = food.targetPos;
                    translation.Value = targetPos;
                }
                translation.Value += gravity * dt;
                CheckBounds(ref translation.Value);
                

            }).ScheduleParallel();
        
        void CheckBounds(ref float3 position)
        {
            if (position.y < -10) position.y = -10;
            if (position.y > 10) position.y = 10;
        }

    }
}
