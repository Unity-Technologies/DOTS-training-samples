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

        var cellMap = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());
        var pheromoneMap = EntityManager.GetBuffer<PheromoneMap>(GetSingletonEntity<PheromoneMap>());

        Entities
            .WithReadOnly(cellMap)
            .ForEach((ref Translation translation, ref Rotation rotation, ref AntMovement ant, in LocalToWorld ltw) =>
            {
                // JITTER
                float jitterAngle = random.NextFloat(-config.RandomSteering, config.RandomSteering);
                ant.FacingAngle += jitterAngle;

                // Pheromone
                var pheromoneHelper = new PheromoneMapHelper(pheromoneMap, config.CellMapResolution, config.WorldSize);
                float pheroSteering = PheromoneSteering(pheromoneHelper, ant.FacingAngle, ltw.Position, (config.WorldSize/128f) * 3f);
                ant.FacingAngle += pheroSteering * config.PheromoneSteerStrength;

                //Debug.Log(string.Format("{1} {0}", ant.FacingAngle, pheroSteering));

                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);

                int wallSteering = WallSteering(cellMapHelper, ant.FacingAngle, ltw.Position, (config.WorldSize/128f) * 1.5f);
                ant.FacingAngle += wallSteering * config.WallSteerStrength;

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
                var newRotation = quaternion.Euler(0, ant.FacingAngle, 0);


                //Debug.Log(string.Format("{0} {1} {2} {3}",
                //    ant.FacingAngle,
                //    jitterAngle,
                //    pheroSteering * config.PheromoneSteerStrength,
                //    wallSteering * config.WallSteerStrength));

                var newPosDelta = ltw.Forward * config.MoveSpeed * time;
                var newPos = translation.Value + newPosDelta;

                Debug.Assert(cellMapHelper.IsInitialized());
                if (cellMapHelper.IsInitialized())
                {
                    var cellState = cellMapHelper.GetCellStateFrom2DPos(new float2(newPos.x, newPos.z));
                    if (cellState == CellState.IsObstacle)
                    {
                        newPos = translation.Value - newPosDelta;
                        ant.FacingAngle += Mathf.PI;
                        newRotation = quaternion.Euler(0, ant.FacingAngle, 0);
                    }
                }

                rotation.Value = newRotation;
                translation.Value = newPos;

                pheromoneHelper.IncrementIntensity(
                    new float2(ltw.Position.x, ltw.Position.z),
                    config.PheromoneProductionPerSecond * time
                );

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static float PheromoneSteering(PheromoneMapHelper pheromone, float facingAngle, float3 antPosition, float distance)
    {
        float output = 0;

        for (int i=-1;i<=1;i+=2) {
            float angle = facingAngle + i * Mathf.PI*.25f;
            float testX = antPosition.x + Mathf.Cos(angle) * distance;
            float testY = antPosition.y + Mathf.Sin(angle) * distance;

            var intensity = pheromone.GetPheromoneIntensityFrom2DPos(new float2(testX, testY));
            if (intensity != -1)
            {
                output += intensity*i;
            }
        }

        return output == 0f ? 0f : Mathf.Sign(output);
    }

    static int WallSteering(CellMapHelper cellMapHelper, float facingAngle, float3 antPosition, float distance) {
        int output = 0;

        for (int i = -1; i <= 1; i+=2) {
            float angle = facingAngle + i * Mathf.PI*.25f;
            float testX = antPosition.x + Mathf.Cos(angle) * distance;
            float testY = antPosition.y + Mathf.Sin(angle) * distance;

            if (cellMapHelper.GetCellStateFrom2DPos(new float2(testX, testY)) == CellState.IsObstacle)
            {
                output -= i;
            }
        }
        return output;
    }

}
