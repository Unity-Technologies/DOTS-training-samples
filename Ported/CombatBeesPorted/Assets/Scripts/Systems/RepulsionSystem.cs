using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


struct AverageBeePos {
    public float3 position;
    public int count;
};

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
        const float maxRepulsionRadius = 4.0f;
        const float posPartitionK = maxRepulsionRadius * 2.0f / 3.0f;

        NativeHashMap<int,AverageBeePos> partitions = new NativeHashMap<int, AverageBeePos>((int)math.sqrt(allBeesQuery.CalculateEntityCount()),Allocator.TempJob);
        Entities
            .WithNone<Attacking>()
            .WithAll<Bee>()
            .ForEach((in Translation translation) =>
            {
                var partIndex = translation.Value * posPartitionK;
                int index =  (int)partIndex.x + ((int)partIndex.y)*1024 + ((int)partIndex.z)*1024*1024;
                var avgBeePos = new AverageBeePos();
                if(partitions.TryGetValue(index, out avgBeePos)) {
                    avgBeePos.count += 1;
                    avgBeePos.position += translation.Value;
                    partitions[index] = avgBeePos;
                } else {
                    partitions[index] = new AverageBeePos() {count=1, position=translation.Value};
                }
            }).Run();

        var keys = partitions.GetKeyArray(Allocator.TempJob);
        foreach(int key in keys) {
            var v = partitions[key];
            v.position /= v.count;
            partitions[key] = v;
        }
        keys.Dispose();
        
        Entities
            .WithNone<Attacking>()
            .WithAll<Bee>()
            .WithDisposeOnCompletion(partitions)
            .ForEach((ref Force beeForce, in Translation translation) =>
            {
                float3 sumForce = float3.zero;
                int count = 1;
                var partIndex = translation.Value * posPartitionK;
                
                var avgBeePos = new AverageBeePos();
                for( int indexX=((int)partIndex.x)-1; indexX<=((int)partIndex.x)+1; indexX++) {
                    for( int indexY=((int)partIndex.y)-1; indexY<=((int)partIndex.y)+1; indexY++) {
                        for( int indexZ=((int)partIndex.z)-1; indexZ<=((int)partIndex.z)+1; indexZ++) {
                            int index = indexX + indexY*1024 + indexZ*1024*1024;
                            if(partitions.TryGetValue(index,out avgBeePos)) {                            
                                var anotherBeePos = avgBeePos.position;// / avgBeePos.count;

                                float3 vec = translation.Value - anotherBeePos;

                                float distance = math.length(vec);                            
                                if(distance != 0) {
                                    float amout = 1.0f - distance / (maxRepulsionRadius*2);
                                    sumForce +=  math.normalize(vec) * amout * avgBeePos.count;
                                    count += avgBeePos.count;
                                }                        
                            }    
                        }
                    }
                }             

                beeForce.Value += sumForce * 3 / count;
            }).Run();
    }
    /*
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
                int index =  (int)partIndex.x + ((int)partIndex.y)*1024;
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
                
                for( int indexX=((int)partIndex.x)-1; indexX<=((int)partIndex.x)+1; indexX++) {
                    for( int indexY=((int)partIndex.y)-1; indexY<=((int)partIndex.y)+1; indexY++) {
                        var iterator = partitions.GetValuesForKey(indexX+indexY*1024);
                        while( iterator.MoveNext()) {
                            var anotherBeePos = iterator.Current;

                            float3 vec = translation.Value - anotherBeePos;

                            float distance = math.length(vec);                            
                            if(distance < maxRepulsionDistance && distance != 0) {
                                float amout = 1.0f - distance / maxRepulsionDistance;
                                sumForce +=  math.normalize(vec) * amout;
                                count += 1;
                            }                        
                        }    
                    }
                }             

                beeForce.Value += sumForce *3 / count;
            }).Run();
    }*/
    
}
