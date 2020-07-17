using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class SpawnResourceSystem : SystemBase
{
    //private EntityQuery m_MainFieldQuery;
    protected override void OnCreate()
    {
        //m_MainFieldQuery = GetEntityQuery(new EntityQueryDesc
        //{
        //    All = new[]
        //    {
        //        ComponentType.ReadOnly<Spawner>()
        //    }
        //});
        // m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    // Start is called before the first frame update
    protected override void OnUpdate()
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null)
            return;

        var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        new UnityEngine.Plane(UnityEngine.Vector3.up, 0).Raycast(ray, out var enter);
        var hit = (float3)ray.GetPoint(enter);
        //var mainField = m_MainFieldQuery.ToComponentDataArrayAsync<Spawner>(Unity.Collections.Allocator.TempJob, out var mainFieldHandle);
        var mouseDown = UnityEngine.Input.GetMouseButton(0);
        if (mouseDown)
        {
            Entities
                .WithStructuralChanges()
                .ForEach((int entityInQueryIndex, Entity entity, in SpawnerNew spawner, in LocalToWorld ltw) =>
                {
                    var instance = EntityManager.Instantiate(spawner.Prefab);
                    //SetComponent(instance, new Translation
                    //{
                    //    Value = ltw.Position + new float3(hit.x, hit.y + 1, hit.z)
                    //});
                }).Run();
        }

    }
}
