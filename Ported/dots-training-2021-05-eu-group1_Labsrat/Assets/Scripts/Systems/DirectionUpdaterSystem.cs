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
public class DirectionUpdaterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            Entities.ForEach((ref Direction direction, ref Rotation rotation, in Translation translation) =>
            {
                if (    (direction.Value == Cardinals.West && translation.Value.x >= gameConfig.BoardDimensions.x - 1)
                    ||  (direction.Value == Cardinals.East && translation.Value.x < 0)
                    ||  (direction.Value == Cardinals.North && translation.Value.z >= gameConfig.BoardDimensions.y - 1)
                    ||  (direction.Value == Cardinals.South && translation.Value.z < 0)
                    )
                {
                    //Rotate Right
                    direction.Value = Direction.RotateRight(direction.Value);
                    
                }

                rotation.Value = math.slerp(rotation.Value, quaternion.RotateY(Direction.GetAngle(direction.Value)), 0.1f);

            }).ScheduleParallel();
        }
    }
}
