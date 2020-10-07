using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FakeTargetAttributionSystem : SystemBase
{
    Random m_Rand;
    
    protected override void OnCreate()
    {
        m_Rand = new Random(0x123456);   
    }

    protected override void OnUpdate()
    {
        // Entities
        //     .WithStructuralChanges()
        //     .WithAll<FarmerTag>()
        //     .WithNone<TargetEntity>()
        //     .ForEach((Entity entity) =>
        //     {
        //         Debug.Log("Adding target");
        //         Entity target = EntityManager.CreateEntity();
        //         float2 targetPos = m_Rand.NextFloat2(10);
        //         EntityManager.AddComponentData(target, new Position(){Value = targetPos});
        //         EntityManager.AddComponentData(entity, new TargetEntity(){target = target, targetPosition = targetPos});
        //     }).Run();
    }
}
