using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public partial class MoveSystem : SystemBase
{
    public Unity.Mathematics.Random random = new Unity.Mathematics.Random(11111);
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        float3 gravityVector = new float3(0, -2, 0);

        Entities
            .WithAll<Gravity>()
            .ForEach((ref Velocity velocity, in Gravity gravity) =>
        {
            velocity.Value += gravityVector * time;
        }).Run();

        Entities
            .WithNone<Decay>()
            .ForEach((ref Translation translation, in Velocity velocity) => 
        {
             translation.Value = translation.Value + velocity.Value * time;
        }).Schedule();

        Entities
            .WithStructuralChanges()
            .WithAll<Gravity>()
            .WithNone<Decay, Food>()
            .ForEach((Entity entity, ref Translation translation) =>
        {
            if (translation.Value.y < -5)
            {
                translation.Value.y = -5;
                EntityManager.RemoveComponent<Gravity>(entity);
                EntityManager.AddComponentData<Decay>(entity, new Decay { RemainingTime = Decay.TotalTime });
            }
        }).Run();

        Entities
            .WithStructuralChanges()
            .WithAll<Food>()
            .WithNone<Decay>()
            .ForEach((Entity entity, ref Translation translation) =>
        {
            if (translation.Value.y < -5)
            {
                translation.Value.y = -5;
                EntityManager.RemoveComponent<Gravity>(entity);

                if (translation.Value.x < -12.5 || translation.Value.x > 12.5)
                {
                    EntityManager.AddComponentData<Decay>(entity, new Decay { RemainingTime = 1.0f });

                    var spawnEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(spawnEntity, new TeamID { Value = translation.Value.x < -12.5 ? 0 : 1 });
                    EntityManager.AddComponentData(spawnEntity, new Spawner { SpawnPosition = translation.Value, Count = random.NextInt(1, 3) });

                    EntityManager.DestroyEntity(entity);
                }
                else
                {
                    EntityManager.AddComponentData<Decay>(entity, new Decay { RemainingTime = Decay.Never });
                }
            }
        }).Run();


        var entityPositions = GetComponentDataFromEntity<Translation>(false);
        Entities
            .ForEach((ref Translation translation, in Food food) =>
        {
            if (food.CarriedBy != Entity.Null)
            {
                translation.Value = entityPositions[food.CarriedBy].Value;
                translation.Value.z -= 1;
            }
        }).Run();
    }
}
