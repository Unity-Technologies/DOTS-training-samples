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

        var config = GetSingleton<Config>();

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
                    config.RotationSpeed * time
                );

                rotation.Value = _rotateThisFrame;
                translation.Value += ltw.Forward * config.MoveSpeed * time;

                var cellMapHelper = new CellMapHelper(map, config.CellMapResolution, config.WorldSize);
                var cellState = cellMapHelper.GetCellStateFrom2DPos(new float2(translation.Value.x, translation.Value.z));
                if (cellState == CellState.IsObstacle)
                {
                    translation.Value = new float3(0,0,0);
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
