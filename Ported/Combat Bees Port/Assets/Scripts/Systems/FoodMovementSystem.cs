using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial class FoodMovementSystem : SystemBase
{
    ComponentDataFromEntity<NotCollected> _notCollected;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        _notCollected = GetComponentDataFromEntity<NotCollected>();
        
        RequireForUpdate<Base>();
    }
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        _notCollected.Update(this);
        
        var notCollected = _notCollected;
        var dt = Time.DeltaTime;
        float3 gravity = new float3(0f, -9.81f, 0f);
        var baseInfo = SystemAPI.GetSingleton<Base>();
        
        Entities
            .WithAll<Food>()
            .ForEach((ref Translation translation, ref Food food, ref Entity foodEntity) =>
            {
                CheckBounds(ref translation.Value);
                if (Exists(food.target))
                {
                    if (GetComponent<Bee>(food.target).state == BeeState.Hauling)
                    {
                        var targetPos = GetComponent<LocalToWorld>(food.target).Position;
                        food.targetPos = targetPos;
                        translation.Value = food.targetPos;
                    }
                    else
                    {
                        translation.Value += gravity * dt;
                        food.target = Entity.Null;
                    }
                }
                else
                {
                    if (translation.Value.x >= baseInfo.blueBase.GetBaseBorderX() || translation.Value.x <= baseInfo.yellowBase.GetBaseBorderX() || translation.Value.y > -9)
                    {
                        notCollected.SetComponentEnabled(foodEntity, false);
                    }
                    else
                    {
                        notCollected.SetComponentEnabled(foodEntity, true);
                    }

                    if (translation.Value.y >= -10)
                    {
                        translation.Value += gravity * dt;
                    }
                }
            }).Run();
        
        void CheckBounds(ref float3 position)
        {
            if (position.y < -10) position.y = -10;
            if (position.y > 10) position.y = 10;
        }

    }
}
