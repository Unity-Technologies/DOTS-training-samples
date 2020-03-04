using System.Security.Cryptography;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using BoxCollider = UnityEngine.BoxCollider;
using Random = UnityEngine.Random;

public class ProjectileSpawnerSystem: JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;
        
        Entities.
            WithStructuralChanges().
            ForEach(
            (PhysicsCollider collider, LocalToWorld transform, ProjectileSpawnerComponentData spawnerData) =>
            {
                spawnerData.timeUntilSpawn -= dt;
                
                if (spawnerData.timeUntilSpawn <= 0.0f)
                {
                    Translation translation = new Translation
                    {
                        Value = transform.Position,
                    };
                    Rotation rotation = new Rotation
                    {
                        Value = transform.Rotation,
                    };
                    
                    var aabb = collider.Value.Value.CalculateAabb();
                    
                    Bounds bounds = new Bounds(translation.Value,aabb.Extents);
                    
                    spawnerData.timeUntilSpawn = spawnerData.spawnTime;
                    float life = Random.Range(spawnerData.lifetimeRange.x, spawnerData.lifetimeRange.y);
                    float3 vel = spawnerData.velocityDirection * Random.Range(spawnerData.velocityRange.x,spawnerData.velocityRange.y);
                    
                    Vector3 randomPos = new Vector3(Random.Range(bounds.min.x,bounds.max.x),
                        Random.Range(bounds.min.y,bounds.max.y),
                        Random.Range(bounds.min.z,bounds.max.z));
                    
                    Debug.Log(randomPos.ToString());

                    float3 initPos = randomPos;

                    Entity newProjectile = EntityManager.Instantiate(spawnerData.projectilePrefab);
                    ProjectileComponentData data = new ProjectileComponentData()
                    {
                        TimeLeft = life,
                        Velocity = vel,
                        initPosition = initPos
                    };
                    
                    Translation pos = new Translation
                    {    
                        Value = initPos,
                    };
                    
                    EntityManager.AddComponentData(newProjectile, pos);
                    EntityManager.AddComponentData(newProjectile, data);
                //
                }
                
            }).Run();

        return default;
    }
}
