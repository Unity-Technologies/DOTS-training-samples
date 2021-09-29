using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class AntMovementSystem : SystemBase
{
    float timerElapsed = 0;
    float timerTick = 2f;

    protected override void OnUpdate()
    {
        // weighted rotation
        // - jitter factor LOW (.14 for jitter)
        // - pheromone factor LOW .015
        // - collision factor HIGH .12
        // - LOS factor 
        // - Nest factor

        // METHOD
        // 

        var random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        var config = GetSingleton<Config>();
        var time = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        //bool isNewTargetTick = false;
        //timerElapsed += time;

        //if (timerElapsed > timerTick)
        //{
        //    timerElapsed -= timerTick;
        //    isNewTargetTick = true;
        //    //Debug.Log(string.Format("Tick"));
        //}

        bool isFirst = true;
        var cellMap = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());

        var pheromoneMap = EntityManager.GetBuffer<PheromoneMap>(GetSingletonEntity<PheromoneMap>());

        

        Entities
            .WithReadOnly(cellMap)
            .ForEach((ref Translation translation, ref Rotation rotation, ref AntMovement ant, in LocalToWorld ltw) =>
            {
                // JITTER
                float jitterAngle = random.NextFloat(-config.RandomSteering, config.RandomSteering);
                ant.FacingAngle += jitterAngle;




                // ---
                //Debug.DrawLine(ltw.Position, new Vector3(1, 0, 1), Color.blue);


                //if (isNewTargetTick)
                //{
                //    ant.Target = rotation.Value * Quaternion.Euler(0, random.NextFloat(0, 360), 0);

                //    if (isFirst)
                //    {
                //        //Debug.Log(string.Format("Tick {0}", ant.Target.eulerAngles.y));
                //    }
                //}

                //Quaternion _rotationDelta = Quaternion.Euler(0, random.NextFloat(-15*time, 15*time), 0);

                // Need the amount the ant will rotate this frame
                //Quaternion _rotateThisFrame = Quaternion.RotateTowards(
                //    rotation.Value,
                //    ant.Target,
                //    config.RotationSpeed * time
                //);

                //rotation.Value = _rotateThisFrame;
                rotation.Value = quaternion.Euler(0, ant.FacingAngle, 0);

                Debug.Log(string.Format("{1} {0}", ant.FacingAngle, jitterAngle));



                //translation.Value += ltw.Forward * config.MoveSpeed * time;

                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);

                //Debug.Assert(cellMapHelper.IsInitialized());
                //if (cellMapHelper.IsInitialized())
                //{
                //    var cellState = cellMapHelper.GetCellStateFrom2DPos(new float2(translation.Value.x, translation.Value.z));
                //    if (cellState == CellState.IsObstacle)
                //    {
                //        translation.Value = new float3(0,0,0);
                //    }
                //}

                //// Pheromone
                //var pheromoneHelper = new PheromoneMapHelper(pheromoneMap, config.CellMapResolution, config.WorldSize);
                //if (pheromoneHelper.IsInitialized())
                //{
                //    pheromoneHelper.IncrementIntensity(
                //        new float2(ltw.Position.x, ltw.Position.z),
                //        config.PheromoneProductionPerSecond * time
                //    );
                //}
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
