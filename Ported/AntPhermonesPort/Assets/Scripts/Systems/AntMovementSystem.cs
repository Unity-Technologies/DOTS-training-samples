using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(4567);
        var time = Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((ref Translation translation, ref Rotation rotation, in AntMovement ant, in LocalToWorld ltw) =>
            {
                Quaternion _rotationDelta = Quaternion.Euler(0, 0.5f, 0);

                // Need the amount the ant will rotate this frame
                Quaternion _rotateThisFrame = Quaternion.RotateTowards(
                    Quaternion.identity,
                    _rotationDelta, 
                    Config.RotationSpeed * (float)time
                );

                rotation.Value = rotation.Value * _rotateThisFrame;
                translation.Value += ltw.Forward * Config.MoveSpeed * (float) time;
            }).Run();

        // EXAMPLE CODE
        int index = CellMap_TRY.GetNearestIndex(new float2(0, 0));
        Debug.Log(CellMap_TRY.CellMap[index]);
        CellMap_TRY.CellMap[index] = CellState.IsFood;

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
