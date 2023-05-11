using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(MouseHit))]
public partial struct InputSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MouseHit>();
    }

    public void OnUpdate(ref SystemState state) {
        var hit = SystemAPI.GetSingleton<MouseHit>().Value;

        if (Input.GetMouseButtonDown(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (new Plane(Vector3.up, 0f).Raycast(ray, out var enter)) {
                hit = ray.GetPoint(enter);
                SystemAPI.SetSingleton(new MouseHit { Value = hit, ChangedThisFrame = true });
            }
        } else {
            SystemAPI.SetSingleton(new MouseHit { Value = hit, ChangedThisFrame = false });
        }
    }
}