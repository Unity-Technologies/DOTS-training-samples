using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputSystem : SystemBase 
{
    protected override void OnUpdate()
    {
        Entities.WithAll<Farmer>()
           .ForEach((Entity entity, ref Velocity speed) =>
           {
               speed.Value = new float3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
           }).Run();
    }
}