using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ResetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Random(6541);
        var center = new Translation{ Value = new float3(64, 64, 0) };
        var minRange = new float2(-1,-1);
        var maxRange = new float2(1,1);

        if (Input.GetKeyUp(KeyCode.R))
        {
            // Reset Ants
            Entities
                .WithAll<Ant>()
                .ForEach((ref Heading heading, ref Translation translation) =>
                {
                    heading.heading = math.normalize(random.NextFloat2(minRange, maxRange));
                    translation.Value = center.Value;
                }).Run();
        }
        
        
        
        // Reset Obstacles

        // Reset Food

        // Reset Pheremones
        
    }
}
