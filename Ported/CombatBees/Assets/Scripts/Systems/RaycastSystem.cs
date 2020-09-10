using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RaycastSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null)
            return;

        var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        new UnityEngine.Plane(UnityEngine.Vector3.up, 0).Raycast(ray, out var enter);
        UnityEngine.Physics.Raycast(ray, out var hit);
        // var hit = (float3)ray.GetPoint(enter);

        var deltaTime = Time.DeltaTime;
        var mouseDown = UnityEngine.Input.GetMouseButton(0);

        var userInput = GetSingleton<UserInput>();
        var mouseHit = new Translation {Value = hit.point};
        if (mouseDown)
        {
            var instance = EntityManager.Instantiate(userInput.ResourcePrefab);    
            EntityManager.SetComponentData(instance, mouseHit);
        }

        var cursor = GetSingletonEntity<UserInput>();
        EntityManager.SetComponentData(cursor, mouseHit);
    }
}