using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Metro
{
    // This system should run after the transform system has been updated, otherwise the camera
    // will lag one frame behind the tank.
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct CameraSystem : ISystem
    {
        Entity _target;
        private Vector3 _offset;
        Random _randomStation;
        Random _randomTrain;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<StationConfig>();

            _randomStation = Random.CreateFromIndex(1234);
            _randomTrain = Random.CreateFromIndex(7348);
        }

        // Because this OnUpdate accesses managed objects, it cannot be Burst compiled.
        public void OnUpdate(ref SystemState state)
        {
            if (_target == Entity.Null || Input.GetKeyDown(KeyCode.S))
            {
                RandomStation(ref state);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                RandomTrain(ref state);
            }

            var cameraTransform = CameraSingleton.Instance.transform;
            var targetTransform = SystemAPI.GetComponent<LocalToWorld>(_target);
            
            cameraTransform.position = targetTransform.Position;
            cameraTransform.position -= 20f * (Vector3)targetTransform.Forward; // move the camera back from the target
            cameraTransform.position += 25f * (Vector3)targetTransform.Right;
            cameraTransform.position += 25f * Vector3.up;
            cameraTransform.LookAt(targetTransform.Position);
        }

        private void RandomStation(ref SystemState state)
        {
            var stationQuery = SystemAPI.QueryBuilder().WithAll<StationIDComponent>().Build();
            var entities = stationQuery.ToEntityArray(Allocator.Temp);
            if (entities.Length == 0)
            {
                return;
            }

            _target = entities[_randomStation.NextInt(entities.Length)];
        }

        private void RandomTrain(ref SystemState state)
        {
            var trainQuery = SystemAPI.QueryBuilder().WithAll<Train>().Build();
            var entities = trainQuery.ToEntityArray(Allocator.Temp);
            if (entities.Length == 0)
            {
                return;
            }

            _target = entities[_randomTrain.NextInt(entities.Length)];
        }
    }
}
