using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    public partial class BeamSpawner : SystemBase
    {
        protected override void OnCreate()
        {
            Debug.Log("BeamSpawner.OnCreate");
        }

        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity, in BuildingSpawnerData spawner) =>
            {
                Debug.Log("BeamSpawner.OnUpdate");

                var random = new Random(1234);
                var pointPosList = new NativeArray<float3>(20000, Allocator.Temp);
                var pointCount = 0;
                for (int i = 0; i < 35; i++)
                {
                    var height = random.NextInt(4, 12);
                    var pos = new float3(random.NextFloat(-45f, 45f), 0f, random.NextFloat(-45f, 45f));
                    float spacing = 2f;
                    for (int j = 0; j < height; j++)
                    {
                        var point = new float3();
                        point.x = pos.x + spacing;
                        point.y = j * spacing;
                        point.z = pos.z - spacing;
                        pointPosList[pointCount++] = point;

                        point = new float3();
                        point.x = pos.x - spacing;
                        point.y = j * spacing;
                        point.z = pos.z - spacing;
                        pointPosList[pointCount++] = point;

                        point = new float3();
                        point.x = pos.x + 0f;
                        point.y = j * spacing;
                        point.z = pos.z + spacing;
                        pointPosList[pointCount++] = point;
                    }
                }

                // We only want to call this once. 
                Enabled = false;
                // Alternative to being called once...
                // EntityManager.DestroyEntity(entity);


                // No ground Details



            }).Run();
        }
    }
}