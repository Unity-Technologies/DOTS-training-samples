using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ApplyGravitySystem : SystemBase
{
    [BurstCompile]
    struct AppyGravityJob : IJobParallelFor
    {
        public NativeArray<float3> vertiesData;
        public NativeArray<float> massData;
        
        public void Execute(int i)
        {
            if (massData[i] == 0f) {
                float3 vert = vertiesData[i];
                vert -= ClothConstants.gravity.y;
                vertiesData[i] = vert;
            }
        }
    }
    
    protected override void OnUpdate()
    {
        //ugly code
        List<ClothMesh> data = new List<ClothMesh>();
        EntityManager.GetAllUniqueSharedComponentData(data);

        foreach (var clothMesh in data)
        {
            NativeArray<float3> vertiesData = clothMesh.vertexPosition;
            NativeArray<float> massData = clothMesh.vertexMass;

            Entities.WithSharedComponentFilter(clothMesh).ForEach((Entity entity, in ClothEdge edge) =>
            {
                float3 p1 = vertiesData[edge.IndexPair.x];
                float3 p2 = vertiesData[edge.IndexPair.y];
                float mass1 = massData[edge.IndexPair.x];
                float mass2 = massData[edge.IndexPair.y];

                float length = math.distance(p2,p1);
                float extra = (length - edge.Length) * .5f;
                float3 dir = math.normalize(p2 - p1);

                // ecb.AddDynamicBuffer(new ForceData(extra, dir));
                if (mass1 == 0 && mass2 == 0) {
                    vertiesData[edge.IndexPair.x] += extra * dir;
                    vertiesData[edge.IndexPair.y] -= extra * dir;
                } else if (mass1 == 0 && mass2 == 1) {
                    vertiesData[edge.IndexPair.x] += extra * dir * 2f;
                } else if (mass1 == 1 && mass2 == 0) {
                    vertiesData[edge.IndexPair.y] -= extra * dir * 2f;
                }
            }).Schedule();

            new AppyGravityJob
            {
                vertiesData = vertiesData,
                massData = massData
            }.Schedule(vertiesData.Length, 64);
            
            JobHandle.ScheduleBatchedJobs();
        }
    }
}
