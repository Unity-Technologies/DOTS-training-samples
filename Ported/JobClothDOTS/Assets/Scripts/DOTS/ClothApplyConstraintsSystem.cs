using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class ClothApplyConstraintsSystem : SystemBase
{
    private List<ClothMesh> data;

    protected override void OnCreate()
    {
        data = new List<ClothMesh>();
    }

    protected override void OnUpdate()
    {
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

                float length = math.distance(p2, p1);
                float extra = (length - edge.Length) * .5f;
                float3 dir = math.normalize(p2 - p1);
                
                if (mass1 == 0 && mass2 == 0)
                {
                    vertiesData[edge.IndexPair.x] += extra * dir;
                    vertiesData[edge.IndexPair.y] -= extra * dir;
                }
                else if (mass1 == 0 && mass2 == 1)
                {
                    vertiesData[edge.IndexPair.x] += extra * dir * 2f;
                }
                else if (mass1 == 1 && mass2 == 0)
                {
                    vertiesData[edge.IndexPair.y] -= extra * dir * 2f;
                }
            }).Schedule();
        }
    }
}