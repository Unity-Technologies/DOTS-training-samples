using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial class AntMovementSystem : SystemBase
{
    float timerElapsed = 0;
    float timerTick = 2f;

    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(4567);
        var time = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        bool isNewTargetTick = false;
        timerElapsed += time;

        if (timerElapsed > timerTick)
        {
            timerElapsed -= timerTick;
            isNewTargetTick = true;
            //Debug.Log(string.Format("Tick"));
        }

        bool isFirst = true;
        var map = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());

        Entities
            .WithReadOnly(map)
            .ForEach((ref Translation translation, ref Rotation rotation, ref AntMovement ant, in LocalToWorld ltw) =>
            {
                if (isNewTargetTick)
                {
                    ant.Target = rotation.Value * Quaternion.Euler(0, random.NextFloat(0, 360), 0);

                    if (isFirst)
                    {
                        //Debug.Log(string.Format("Tick {0}", ant.Target.eulerAngles.y));
                    }
                }

                //Quaternion _rotationDelta = Quaternion.Euler(0, random.NextFloat(-15*time, 15*time), 0);

                // Need the amount the ant will rotate this frame
                Quaternion _rotateThisFrame = Quaternion.RotateTowards(
                    rotation.Value,
                    ant.Target,
                    Config.RotationSpeed * time
                );

                rotation.Value = _rotateThisFrame;
                translation.Value += ltw.Forward * Config.MoveSpeed * time;

                //Temporarily just reference something in the map to avoid warning, eventually we will actually use it for collision detection
                var len = map.Length;
            }).Run();

        // EXAMPLE CODE
        //int index = CellMap_TRY.GetNearestIndex(new float2(0, 0));
        //Debug.Log(CellMap_TRY.CellMap[index]);
        //CellMap_TRY.CellMap[index] = CellState.IsFood;

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
