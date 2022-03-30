using Components;
using Unity.Entities;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace Systems
{
    public partial class CameraSystem : SystemBase
    {
        private Vector2 _viewAngles;
        private Vector2 _smoothViewAngles;
        private float _viewDist;
        private float _smoothViewDist;

        private float _sensitivity = 5000;
        private float _zoomSensitivity = 0.2f;
        private float _stiffness = 12;
        
        protected override void OnCreate()
        {
            _viewDist = 70f;
            _smoothViewDist = _viewDist;
            _sensitivity = 5000;
            _zoomSensitivity = 0.2f;
            _stiffness = 12;
        }

        protected override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1))
            {
                _viewAngles.x += Input.GetAxis("Mouse X") * _sensitivity / Screen.height;
                _viewAngles.y -= Input.GetAxis("Mouse Y") * _sensitivity / Screen.height;

                _viewAngles.y = Mathf.Clamp(_viewAngles.y, -89f, 89f);
            }

            _viewDist -= Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity * _viewDist;
            _viewDist = Mathf.Clamp(_viewDist, 5f, 80f);

            _smoothViewAngles = Vector2.Lerp(_smoothViewAngles, _viewAngles, _stiffness * Time.DeltaTime);
            _smoothViewDist = Mathf.Lerp(_smoothViewDist, _viewDist, _stiffness * Time.DeltaTime);

            var cameraTransform = this.GetSingleton<GameObjectRefs>().Camera.transform;
            cameraTransform.rotation = Quaternion.Euler(_smoothViewAngles.y, _smoothViewAngles.x, 0f);
            cameraTransform.position = -cameraTransform.forward * _smoothViewDist;
        }
    }
}