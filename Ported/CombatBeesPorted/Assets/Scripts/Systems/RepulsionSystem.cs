using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class RepulsionSystem: SystemBase
{
    private EntityQueryDesc allBeesQueryDesc; 
    protected override void OnCreate()
    {
        allBeesQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Bee)} //, ComponentType.ReadOnly<WorldRenderBounds>()
        };

    }

    protected override void OnUpdate()
    {

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        EntityQuery allBeesQuery = GetEntityQuery(allBeesQueryDesc);        
        const float maxRepulsionDistance = 5.0f;

        NativeMultiHashMap<int,float3> partitions =  new NativeMultiHashMap<int, float3>(allBeesQuery.CalculateEntityCount(), Allocator.TempJob);
        Entities
            .WithNone<Attacking>()
            .WithAll<Bee>()
            .ForEach((in Translation translation) =>
            {
                var partIndex = translation.Value / maxRepulsionDistance / 2.0f;
                int index =  (int)partIndex.x;// + ((int)partIndex.y)*1024;// + ((int)partIndex.z)*1024*1024;
                partitions.Add(index, translation.Value);
            
            }).Run();

        Entities
            .WithNone<Attacking>()
            .WithAll<Bee>()
            .WithDisposeOnCompletion(partitions)
            .ForEach((ref Force beeForce, in Translation translation) =>
            {
                float3 sumForce = float3.zero;
                int count = 1;
                var partIndex = translation.Value / maxRepulsionDistance / 2.0f;
                
                for( int index=((int)partIndex.x)-1; index<=((int)partIndex.x)+1; index++) {
                    var iterator = partitions.GetValuesForKey(index);
                    while( iterator.MoveNext()) {
                        var anotherBeePos = iterator.Current;

                        //if( math.all(translation.Value == anotherBeePos)) continue;
                        
                        float3 vec = translation.Value - anotherBeePos;

                        float distance = math.length(vec);                            
                        if(distance < maxRepulsionDistance && distance != 0) {
                            float amout = 1.0f - distance / maxRepulsionDistance;
                            sumForce +=  math.normalize(vec) * amout;
                            count += 1;
                        }
                    }
                }             

                beeForce.Value += sumForce *3 / count;
            }).Run();
    }
}
