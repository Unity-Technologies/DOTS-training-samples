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
        NativeArray<Entity> allBees = allBeesQuery.ToEntityArray(Allocator.Temp);
        const float maxRepulsionDistance = 10.0f;

        NativeMultiHashMap<int,float3> partitions =  new NativeMultiHashMap<int, float3>(allBees.Length, Allocator.TempJob);
        Entities
            .WithNone<Attacking>()
            .WithAll<Bee>()
            .ForEach((in Translation translation) =>
            {
                var partIndex = translation.Value / maxRepulsionDistance / 2.0f;
                int index =  (int)partIndex.x + ((int)partIndex.y)*1024;
                partitions.Add(index, translation.Value);
            
            }).Run();

        /*NativeArray<int> allKeys = partitions.GetUniqueKeyArray(Allocator.Temp).Item1;
        foreach( var key in allKeys)
            Debug.Log(key);*/

        Entities
            .WithNone<Attacking>()
            .WithoutBurst()
            .ForEach((Entity entity, ref Force beeForce, in Bee bee, in Translation translation) =>
            {
                float3 sumForce = float3.zero;
                int count = 1;
                var partIndex = translation.Value / maxRepulsionDistance / 2.0f;
                
                for( int indexX=((int)partIndex.x)-1; indexX<=((int)partIndex.x)+1; indexX++) {
                    for( int indexY=((int)partIndex.y)-1; indexY<=((int)partIndex.y)+1; indexY++) {

                        int index =  indexX + indexY*1024;
                        var iterator = partitions.GetValuesForKey(index);
                        while( iterator.MoveNext()) {
                            var anotherBeePos = iterator.Current;

                            if( math.all(translation.Value == anotherBeePos)) continue;
                            
                            float3 vec = translation.Value - anotherBeePos;

                            float distance = math.length(vec);                            
                            if(distance < maxRepulsionDistance) {
                                float amout = 1.0f - distance / maxRepulsionDistance;
                                sumForce +=  math.normalize(vec) * amout;
                                count += 1;
                                //Debug.DrawLine(translation.Value,anotherBeePos);   
                            }
                        }
                    }   
                }             

                beeForce.Value += sumForce *3 / count;
            }).Run();
        
        partitions.Dispose();
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
