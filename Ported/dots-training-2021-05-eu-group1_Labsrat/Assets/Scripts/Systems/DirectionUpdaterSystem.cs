using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
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
        NativeArray<Entity> cells = World.GetOrCreateSystem<BoardSpawner>().cells;
        NativeArray<Cardinals> walls = World.GetOrCreateSystem<BoardSpawner>().walls;

        if (TryGetSingleton(out GameConfig gameConfig))
        {
            if (cells.Length == 0)
                return;

            var forcedDirectionData = GetComponentDataFromEntity<ForcedDirection>(true);

            Entities
                .WithAny<Cat, Mouse>().WithReadOnly(cells).WithReadOnly(forcedDirectionData).WithReadOnly(walls)
                .ForEach((ref Direction direction, ref Translation translation, ref Rotation rotation) =>
            {
                bool recenter = false;

                int index = InputSystem.CellAtWorldPosition(translation.Value, gameConfig);

                Entity cell =  cells[index];
                Cardinals wallCollision = walls[index];
                ForcedDirection fd = forcedDirectionData[cell];

                float2 cellCenter = new float2(math.round(translation.Value.x), math.round(translation.Value.z));

                // If we're stepping on an arrow
                if (
                    fd.Value != Cardinals.None                                                                                                                    // If there's a forced direction (arrow) ....
                    && fd.Value != direction.Value                                                                                                                // ... and we're not already in the given direction
                    && math.distancesq(cellCenter, new float2(translation.Value.x, translation.Value.z)) < (gameConfig.SnapDistance * gameConfig.SnapDistance)    // ... and we're close to the center
                    )
                {
                    direction.Value = fd.Value;
                    recenter = true;
                    
                }

                // Until we don't have a wall facing us

                while (
                    wallCollision != Cardinals.All &&
                    ((wallCollision & direction.Value) != 0) &&
                    math.distancesq(cellCenter, new float2(translation.Value.x, translation.Value.z)) < (gameConfig.SnapDistance * gameConfig.SnapDistance)
                    )
                {
                    direction.Value = Direction.RotateLeft(direction.Value);
                    recenter = true;
                }

                if(recenter)
                {
                    // Recenter on Cell?
                    translation.Value.x = cellCenter.x;
                    translation.Value.z = cellCenter.y;
                }

                rotation.Value = math.slerp(rotation.Value, quaternion.RotateY(Direction.GetAngle(direction.Value)), 0.1f);

            }).ScheduleParallel();
        }
    }
}
