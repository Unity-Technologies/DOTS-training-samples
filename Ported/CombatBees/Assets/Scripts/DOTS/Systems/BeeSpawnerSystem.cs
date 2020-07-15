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
                    for (int x = 0; x < spawner.BeeCount; ++x)
                    {
                        //var xVal = m_Random.NextInt(mainField[i].Bounds.Extents.)
                        var instance = EntityManager.Instantiate(spawner.Prefab);
                        EntityManager.SetComponentData(instance,
                            new Translation
                            {
                                Value = new float3(mainField[i].Bounds.Center.x, math.sin(x) * x, math.cos(x) * x)
                            });

                        if (i == 0)
                        {
                            EntityManager.AddComponentData(instance, new TeamOne() { });
                        }
                        else
                        {
                            EntityManager.AddComponentData(instance, new TeamTwo() { });
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
