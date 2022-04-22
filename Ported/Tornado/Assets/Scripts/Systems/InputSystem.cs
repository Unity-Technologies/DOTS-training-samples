
using Assets.Scripts;
using Components;
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
        private Vector3 velocityTornado;   

        protected override void OnUpdate()
        {
            cameraRef = this.GetSingleton<CameraRef>();
            inputs = GetSingleton<InputSettings>();
           
            var tornadoSettings = GetSingleton<TornadoParameters>();
            Vector3 movingQuantity= Vector3.zero;

            Vector3 tornadomovingQuantity = Vector3.zero;
            var tCamera = cameraRef.Camera.transform;
            var forward2D = Vector3.Cross(tCamera.right, Vector3.up);
            var right2D = Vector3.Cross(Vector3.up ,forward2D );

            var dt = Time.DeltaTime;
            if (UnityInput.GetKey(UnityKeyCode.UpArrow)) movingQuantity += forward2D * inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.DownArrow)) movingQuantity -= forward2D *inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.RightArrow)) movingQuantity += right2D * inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.LeftArrow)) movingQuantity -= right2D * inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Space)) movingQuantity += Vector3.up * inputs.cameraAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.LeftControl)) movingQuantity -= Vector3.up * inputs.cameraAcceleration * dt;


            if (UnityInput.GetKey(UnityKeyCode.Keypad6)) tornadomovingQuantity += right2D * inputs.tornadoAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Keypad4)) tornadomovingQuantity -= right2D * inputs.tornadoAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Keypad8)) tornadomovingQuantity += forward2D * inputs.tornadoAcceleration * dt;
            if (UnityInput.GetKey(UnityKeyCode.Keypad5)) tornadomovingQuantity -= forward2D *inputs.tornadoAcceleration * dt;

            velocityTornado +=  tornadomovingQuantity;       
  

            tornadoSettings.eyePosition += new float3(velocityTornado.x, velocityTornado.y, velocityTornado.z);

            SetSingleton<TornadoParameters>(tornadoSettings);
           
            velocity += movingQuantity;
            
            cameraRef.Camera.transform.position += velocity;
            var tornadoPos = new Vector3(tornadoSettings.eyePosition.x, 0f, tornadoSettings.eyePosition.z);
            cameraRef.Camera.transform.LookAt(tornadoPos + Vector3.up * 15f);

            velocity *= inputs.friction;
            velocityTornado *= inputs.friction;

            ComputeShaderManager.Instance.tornadoPosition = tornadoPos;
            if (Input.GetKeyDown(KeyCode.R))
            {
                
                World.GetExistingSystem<GenerationSystem>().Reset();
            }

        }
    }
}