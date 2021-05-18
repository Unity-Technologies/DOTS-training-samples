using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            var time = Time.DeltaTime;

            Entities.
                ForEach((ref Translation translation, in Cat cat, in Direction direction) =>
                {
                    var offset = direction.getDirection() * time * gameConfig.CatSpeed;
                    translation.Value.x += offset.x;
                    translation.Value.z += offset.y;
                }).ScheduleParallel();

            Entities.
                ForEach((ref Translation translation, in Mouse mouse, in Direction direction) =>
                {
                    var offset = direction.getDirection() * time * gameConfig.MouseSpeed; 
                    translation.Value.x += offset.x;
                    translation.Value.z += offset.y;
                }).ScheduleParallel();
        }


    }

}
