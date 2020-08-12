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
            var hit = (float3) ray.GetPoint(enter);

            var deltaTime = Time.DeltaTime;
            var mouseDown = UnityEngine.Input.GetMouseButton(0);
            Entities.ForEach((ref Color color, in LocalToWorld ltw) =>
            {
                var dist = mouseDown ? math.distancesq(ltw.Position, hit) / 40 : 1;
                var clamped = math.clamp(1 - dist, 0, 1);
                color.Value.x = math.max(clamped, color.Value.x - deltaTime / 30);
            }).ScheduleParallel();
        }
    }
