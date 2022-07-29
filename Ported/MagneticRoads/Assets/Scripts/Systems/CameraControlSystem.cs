using Aspects;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct CameraControlSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }
        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            // float3 targetPosition;
            // float3 cameraOffset;
            // int index = 0;
            //
            // foreach (var carAspect in SystemAPI.Query<CarAspect>())
            // {
            //     targetPosition = carAspect.Position;
            //     
            //     // Camera.main.transform.LookAt(targetPosition);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.UpArrow))
            // {
            //     index++;
            // }
        }
    }
}
