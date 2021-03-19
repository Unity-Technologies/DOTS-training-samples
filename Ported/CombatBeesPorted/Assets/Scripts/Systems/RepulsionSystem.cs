using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

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

        endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    EndSimulationEntityCommandBufferSystem endSim;    

    protected override void OnUpdate()
    {
        var commandBuffer = endSim.CreateCommandBuffer();

        EntityQuery allBeesQuery = GetEntityQuery(allBeesQueryDesc);        
        const float maxRepulsionRadius = 4.0f;
        const float posPartitionK = maxRepulsionRadius * 2.0f / 3.0f;

        int beesCount = allBeesQuery.CalculateEntityCount();        
        UnsafeHashMap<int,AverageBeePos> partitions = new UnsafeHashMap<int, AverageBeePos>((int)math.sqrt(beesCount),Allocator.TempJob);

        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));     
        float keepPercent = math.max(2000,math.sqrt(beesCount) / beesCount);
        Entities
            .WithNone<Attacking>()
            .WithAll<Bee>()
            .ForEach((in Translation translation) =>
            {
                if(random.NextFloat(1.0f) <keepPercent) {
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
                }
            }).Schedule();

        Job.WithCode( () => {
            var keys = partitions.GetKeyArray(Allocator.Temp);
            foreach(int key in keys) {
                var v = partitions[key];
                v.position /= v.count;
                partitions[key] = v;
            }
            keys.Dispose();
        }).Schedule();
        
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
            }).Schedule();

        endSim.AddJobHandleForProducer(Dependency);
    }
    
}
