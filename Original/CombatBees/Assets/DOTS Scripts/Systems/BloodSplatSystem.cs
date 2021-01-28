using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class BloodSplatSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        float floorHeight = GetSingleton<SpawnZones>().LevelBounds.Min.y;
        
        Entities
            .WithAll<BloodTag>()
            .WithNone<GroundedTime>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity e, in PhysicsData data, in Translation translation) =>
            {
                if (translation.Value.y <= floorHeight + 0.001f)
                {
                    EntityManager.AddComponentData(e, new GroundedTime
                    {
                        Time = time,
                    });
                }
            }).Run();
            
        Entities
            .WithAll<BloodTag>()
            .WithoutBurst()
            .ForEach((ref URPPropertyLifetime smoothness, in GroundedTime groundedTime) =>
            {
                float newVal = 1.0f - ((float)(time - groundedTime.Time));
                smoothness.Value = math.clamp(newVal, 0, 1.0f);
            }).Run();
    }
}