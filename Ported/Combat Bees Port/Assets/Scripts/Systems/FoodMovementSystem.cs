using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial class FoodMovementSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        float3 gravity = new float3(0f, -9.81f, 0f);

        
        Entities
            .WithAll<Food>()
            .ForEach((ref Translation translation, ref Food food) =>
            {
                CheckBounds(ref translation.Value);
                /*if (HasComponent<Bee>(food.target))
                {
                    /*var targetPos = GetComponent<Translation>(food.target).Value;
                    translation.Value = targetPos;#1#
                    translation.Value = food.targetPos;
                }
                else translation.Value += gravity * dt;*/

                if (!food.targetPos.Equals(float3.zero))
                {
                    translation.Value = food.targetPos;
                }
                else translation.Value += gravity;
                
                CheckBounds(ref translation.Value);
                

            }).Run();
        
        void CheckBounds(ref float3 position)
        {
            if (position.y < -10) position.y = -10;
            if (position.y > 10) position.y = 10;
        }
        

    }
}
