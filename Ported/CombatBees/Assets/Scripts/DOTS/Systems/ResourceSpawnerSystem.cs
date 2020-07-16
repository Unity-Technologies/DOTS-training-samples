using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class ResourceSpawnerSystem : SystemBase
{
    private EntityQuery m_MainFieldQuery;

    protected override void OnCreate()
    {
        m_MainFieldQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<FieldInfo>()
            }, 
            None = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<TeamTwo>()
            }
        });
    }

    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
        .ForEach((Entity entity, in Spawner spawner, in LocalToWorld ltw) =>
        {
            var mainField = m_MainFieldQuery.ToComponentDataArray<FieldInfo>(Unity.Collections.Allocator.Temp).FirstOrDefault();
            var center = mainField.Bounds.Center;

            for (int x = 0; x < spawner.Count; ++x)
            {
                var instance = EntityManager.Instantiate(spawner.Prefab);
                SetComponent(instance, new Translation
                {
                    Value = center + new float3(math.sin(x), 0, math.cos(x)) * x * 0.1f
                });
                EntityManager.AddComponentData(instance, new ResourceNew());
            }

            EntityManager.DestroyEntity(entity);
        }).Run();
    }
}
