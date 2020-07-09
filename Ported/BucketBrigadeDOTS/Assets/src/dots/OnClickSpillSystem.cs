using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class OnClickSpillSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null)
            return;
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            new UnityEngine.Plane(UnityEngine.Vector3.up, 0).Raycast(ray, out var enter);
            var hit = (float3)ray.GetPoint(enter);
            var deltaTime = Time.DeltaTime;
            var newEntity = EntityManager.CreateEntity();
            WaterSpill spill;
            spill.SpillPosition = hit.xz;
            EntityManager.AddComponent<WaterSpill>(newEntity);
            EntityManager.SetComponentData<WaterSpill>(newEntity, spill);
        }
    }
}
