
﻿using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

namespace Systems
{
    public partial class InputSystem : SystemBase
    {
        CameraRef cameraRef;
        InputSettings inputs;
        private Vector3 velocity;
        private float3 velocityTornado;   

        protected override void OnUpdate()
        {
            cameraRef = this.GetSingleton<CameraRef>();
            inputs = GetSingleton<InputSettings>();
           
            var tornadoSettings = GetSingleton<TornadoParameters>();
            Vector3 movingQuantity= Vector3.zero;

            float3 tornadomovingQuantity = float3.zero;

            var dt = Time.DeltaTime;
            if (UnityInput.GetKey(UnityKeyCode.UpArrow)) movingQuantity.z += inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.DownArrow)) movingQuantity.z -= inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.RightArrow)) movingQuantity.x += inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.LeftArrow)) movingQuantity.x -= inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Space)) movingQuantity.y += inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.LeftControl)) movingQuantity.y -= inputs.cameraAcceleration * dt;


            if (UnityInput.GetKey(UnityKeyCode.Keypad6)) tornadomovingQuantity.x += inputs.tornadoAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Keypad4)) tornadomovingQuantity.x -= inputs.tornadoAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Keypad8)) tornadomovingQuantity.z += inputs.tornadoAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Keypad5)) tornadomovingQuantity.z -= inputs.tornadoAcceleration * dt;

            velocityTornado += tornadomovingQuantity * dt; 
            tornadoSettings.eyePosition += velocityTornado * dt;

            SetSingleton<TornadoParameters>(tornadoSettings);
           
            velocity += movingQuantity * dt;
            
            cameraRef.Camera.transform.position += velocity;
            var tornadoPos = new Vector3(tornadoSettings.eyePosition.x, 0f, tornadoSettings.eyePosition.z);
            cameraRef.Camera.transform.LookAt(tornadoPos + Vector3.up * 15f);

            velocity *= inputs.friction;
            velocityTornado *= inputs.friction;

            if (Input.GetKeyDown(KeyCode.R))
            {
                
                World.GetExistingSystem<GenerationSystem>().Reset();
            }

        }
    }
}