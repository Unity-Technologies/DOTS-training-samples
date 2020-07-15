using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BeeSpawnerSystem : SystemBase
{
    private EntityQuery m_baseQuery;
    Unity.Mathematics.Random m_Random = new Unity.Mathematics.Random(0x5716318);

    protected override void OnCreate()
    {
        m_baseQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<FieldInfo>()
            },
            Any = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<TeamTwo>()
            }
        });
    }

    protected override void OnUpdate()
    {
        //var currentScore = GetSingleton<BeeManager.Instance>();
        var mainField = m_baseQuery.ToComponentDataArray<FieldInfo>(Unity.Collections.Allocator.Temp);

        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in BeeSpawner spawner, in LocalToWorld ltw) =>
            {
                for (int i = 0; i < mainField.Length; i++)
                {
                    float xCenter = mainField[i].Bounds.Center.x;
                    float xExtents = mainField[i].Bounds.Extents.x;
                    float yCenter = mainField[i].Bounds.Center.y;
                    float yExtents = mainField[i].Bounds.Extents.y;
                    float zCenter = mainField[i].Bounds.Center.z;
                    float zExtents = mainField[i].Bounds.Extents.z;
                    for (int x = 0; x < spawner.BeeCount; ++x)
                    {
                        var xVal = m_Random.NextFloat(xCenter - xExtents, xCenter + xExtents);
                        var yVal = m_Random.NextFloat(yCenter - yExtents, yCenter + yExtents);
                        var zVal = m_Random.NextFloat(zCenter - zExtents, zCenter + zExtents);
                        var instance = EntityManager.Instantiate(spawner.Prefab);
                        EntityManager.SetComponentData(instance,
                            new Translation
                            {
                                Value = new float3(xVal, yVal, zVal)
                            });

                        if (i == 0)
                        {
                            EntityManager.AddComponentData(instance, new TeamOne());
                        }
                        else
                        {
                            EntityManager.AddComponentData(instance, new TeamTwo());
                        }

                        EntityManager.SetComponentData(instance,
                            new BeeColor
                            {
                                Value = new float4(mainField[i].TeamColor.r, mainField[i].TeamColor.g, mainField[i].TeamColor.b, mainField[i].TeamColor.a)
                            });
                    }
                }

                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}
