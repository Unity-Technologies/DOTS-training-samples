using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class AntMovementSystem : SystemBase
{
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

        var config = GetSingleton<Config>();
        var random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        var time = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var cellMap = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());
        var pheromoneMap = EntityManager.GetBuffer<PheromoneMap>(GetSingletonEntity<PheromoneMap>());

        Entities
            .WithReadOnly(cellMap)
            .ForEach((ref Translation translation, ref Rotation rotation, ref AntMovement ant, in LocalToWorld ltw) =>
            {
                UnityEngine.Time.timeScale = config.Speed;

                // JITTER
                float jitterAngle = random.NextFloat(-config.RandomSteering, config.RandomSteering);
                ant.FacingAngle += jitterAngle;

                // Pheromone
                var pheromoneHelper = new PheromoneMapHelper(pheromoneMap, config.CellMapResolution, config.WorldSize);
                float pheroSteering = PheromoneSteering(pheromoneHelper, ant.FacingAngle, ltw.Position, (config.WorldSize/(float)config.CellMapResolution) * 3f);
                ant.FacingAngle += pheroSteering * config.PheromoneSteerStrength;

                //Debug.Log(string.Format("{1} {0}", ant.FacingAngle, pheroSteering));

                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);

                int wallSteering = WallSteering(cellMapHelper, ant.FacingAngle, ltw.Position, (config.WorldSize/(float)config.CellMapResolution) * 1.5f);
                ant.FacingAngle += wallSteering * config.WallSteerStrength;

                float targetSpeed = config.MoveSpeed;
                float slowDownFactor = 1f - (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f;

                targetSpeed *= slowDownFactor;
                ant.AntSpeed += (targetSpeed - ant.AntSpeed) * config.AntAcceleration;

                // ---
                //Debug.DrawLine(ltw.Position, new Vector3(1, 0, 1), Color.blue);

                var newRotation = quaternion.Euler(0, ant.FacingAngle, 0);

                //Debug.Log(string.Format("{0} {1} {2} {3}",
                //    ant.FacingAngle,
                //    jitterAngle,
                //    pheroSteering * config.PheromoneSteerStrength,
                //    wallSteering * config.WallSteerStrength));

                var newPosDelta = ltw.Forward * targetSpeed * time;
                var newPos = translation.Value + newPosDelta;
                bool turnAround = false;

                Debug.Assert(cellMapHelper.IsInitialized());
                var cellState = cellMapHelper.GetCellStateFrom2DPos(new float2(newPos.x, newPos.z));
                if (cellState == CellState.IsObstacle)
                {
                    turnAround = true;
                }
                else if (cellState == CellState.IsFood && ant.State != AntState.ReturnHome)
                {
                    ant.State = AntState.ReturnHome;
                    turnAround = true;
                }
                else if (cellState == CellState.IsNest && ant.State == AntState.ReturnHome)
                {
                    ant.State = AntState.Searching;
                    turnAround = true;
                }

                if (turnAround)
                {
                    newPos = translation.Value - newPosDelta;
                    ant.FacingAngle += Mathf.PI;
                    newRotation = quaternion.Euler(0, ant.FacingAngle, 0);
                }

                rotation.Value = newRotation;
                translation.Value = newPos;

                float excitement = config.PheromoneProductionPerSecond * time;
                if (ant.State == AntState.ReturnHome)
                {
                    excitement *= config.AntHasFoodPeromoneMultiplier;
                }
                excitement *= ant.AntSpeed / config.MoveSpeed;

                Debug.Assert(pheromoneHelper.IsInitialized());
                pheromoneHelper.IncrementIntensity(
                    new float2(ltw.Position.x, ltw.Position.z),
                    excitement
                );

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static float PheromoneSteering(PheromoneMapHelper pheromone, float facingAngle, float3 antPosition, float distance)
    {
        float output = 0;

        for (int i=-1;i<=1;i+=2)
        {
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

    static int WallSteering(CellMapHelper cellMapHelper, float facingAngle, float3 antPosition, float distance)
    {
        // If wall detected to the right or left of the current direction then return -1 / 1 so that we steer away from it
        // If no wall then it returns 0
        // TBD: if wall detected in both checked directions then it continues towards collision, is that desirable?
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
