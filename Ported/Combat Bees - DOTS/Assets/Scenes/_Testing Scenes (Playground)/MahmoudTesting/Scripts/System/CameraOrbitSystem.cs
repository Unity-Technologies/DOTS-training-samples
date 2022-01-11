using System.Runtime.InteropServices;
using Combatbees.Testing.Mahmoud;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;

namespace Combatbees.Testing.Mahmoud
{
    public partial class CameraOrbitSystem : SystemBase
    {
        float sensitivity=100;
        float zoomSensitivity=20f;
        float stiffness=12f;
        Vector2 viewAngles;
        Vector2 smoothViewAngles;
        float viewDist;
        float smoothViewDist;
        private Camera _camera;

        protected override void OnCreate()
        {
            viewDist = -80f;
            smoothViewDist = viewDist;
            RequireSingletonForUpdate<SingeltonHybridSpawner>();
        }

        protected override void OnUpdate()
        {
            _camera = this.GetSingleton<GameObjectRef>().Camera;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                viewAngles.X += Input.GetAxis("Mouse X") * sensitivity / Screen.height;
                viewAngles.Y -= Input.GetAxis("Mouse Y") * sensitivity / Screen.height;

                viewAngles.Y = Mathf.Clamp(viewAngles.Y, -30f, 30f);
            }

            
            viewDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity ;
            viewDist = Mathf.Clamp(viewDist, 0f, 80f);
            
            smoothViewAngles = Vector2.Lerp(smoothViewAngles, viewAngles, stiffness * Time.DeltaTime);
            smoothViewDist = Mathf.Lerp(smoothViewDist, viewDist, stiffness * Time.DeltaTime);

            _camera.transform.rotation = quaternion.Euler(viewAngles.Y, viewAngles.X, 0f);
            _camera.transform.position =- _camera.transform.forward * smoothViewDist;
        }
    }
}
