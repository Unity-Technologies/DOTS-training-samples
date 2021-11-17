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

    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<HiveTeam0>();
        this.RequireSingletonForUpdate<HiveTeam1>();
    }

    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        float3 gravityVector = new float3(0, -2, 0);

        var team0Hive = GetSingletonEntity<HiveTeam0>();
        var team1Hive = GetSingletonEntity<HiveTeam1>();
        var team0HiveData = GetComponent<HiveTeam0>(team0Hive);
        var team1HiveData = GetComponent<HiveTeam1>(team1Hive);
        var team0HiveBounds = GetComponent<Bounds>(team0Hive);
        var team1HiveBounds = GetComponent<Bounds>(team1Hive);

        // Apply gravity
        Entities
            .WithAll<Gravity>()
            .ForEach((ref Velocity velocity, in Gravity gravity) =>
        {
            velocity.Value += gravityVector * time;
        }).Run();

        // Integrate forward
        Entities
            .WithNone<Decay>()
            .ForEach((ref Translation translation, in Velocity velocity) => 
        {
             translation.Value = translation.Value + velocity.Value * time;
        }).Schedule();

        // Bounce off walls
        var worldBounds = GetComponent<Bounds>(GetSingletonEntity<Globals>());
        var worldMin = worldBounds.Value.Min;
        var worldMax = worldBounds.Value.Max;
        Entities
            .ForEach((ref Translation translation, ref Bounds bounds, ref Velocity velocity) =>
        {
            bounds.Value.Center = translation.Value;
            var min = bounds.Value.Min;
            var max = bounds.Value.Max;

            float3 mirrorNormal = new float3(0);
            if (min.x > worldMax.x)
            {
                mirrorNormal += new float3(-1, 0, 0);
                bounds.Value.Center.x = worldMax.x - bounds.Value.Extents.x;
            }
            if (max.x < worldMin.x)
            {
                mirrorNormal += new float3(1, 0, 0);
                bounds.Value.Center.x = worldMin.x + bounds.Value.Extents.x;
            }

            if (min.y > worldMax.y)
            {
                mirrorNormal += new float3(0, -1, 0);
                bounds.Value.Center.y = worldMax.y - bounds.Value.Extents.y;
            }
            if (max.y < worldMin.y)
            {
                mirrorNormal += new float3(0, 1, 0);
                bounds.Value.Center.y = worldMin.y + bounds.Value.Extents.y;
            }

            if (min.z > worldMax.z)
            {
                mirrorNormal += new float3(0, 0, -1);
                bounds.Value.Center.z = worldMax.z - bounds.Value.Extents.z;
            }
            if (max.z < worldMin.z)
            {
                mirrorNormal += new float3(0, 0, 1);
                bounds.Value.Center.z = worldMin.z + bounds.Value.Extents.z;
            }

            if (math.any(mirrorNormal))
            {
                velocity.Value = math.reflect(velocity.Value, math.normalize(mirrorNormal));
            }
            translation.Value = bounds.Value.Center;
        }).Schedule();

        // When things fall on ground, turn off gravity and turn on decay
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

        // Handle falling food
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

                int teamID = -1;
                if (translation.Value.x < -12.5)
                    teamID = 0;
                if (translation.Value.x > 12.5)
                    teamID = 1;

                if (teamID == 0 || teamID == 1)
                {
                    EntityManager.AddComponentData<Decay>(entity, new Decay { RemainingTime = 1.0f });

                    Entity beePrefab = (teamID == 0) ? team0HiveData.BeePrefab : team1HiveData.BeePrefab;
                    var spawnEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(spawnEntity, new TeamID { Value = teamID });
                    EntityManager.AddComponentData(spawnEntity, new Spawner { Prefab = beePrefab, SpawnPosition = translation.Value, Count = random.NextInt(1, 3) });
                    EntityManager.DestroyEntity(entity);
                }
                else
                {
                    EntityManager.AddComponentData<Decay>(entity, new Decay { RemainingTime = Decay.Never });
                }
            }
        }).Run();


        // Handle carried food
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

        // Orient objects in their direction of motion when moving under their own power
        var up = new float3(0, 1, 0);
        Entities
            .WithNone<Gravity, Decay>()
            .ForEach((ref Velocity velocity, in Translation translation, in Goal goal) =>
        {
            velocity.Value = 4.0f * math.normalize(velocity.Value + math.normalize(goal.target - translation.Value) * 0.2f);
        }).Schedule();

        Entities
            .WithNone<Gravity, Decay>()
            .ForEach((ref Rotation rotation, in Velocity velocity) =>
        {
            rotation.Value = Quaternion.FromToRotation(Vector3.up, math.normalize(velocity.Value));
        }).Schedule();
    }
}
