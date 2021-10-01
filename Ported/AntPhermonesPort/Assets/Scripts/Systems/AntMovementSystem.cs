using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Rendering;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class AntMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Food>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        var time = Time.DeltaTime;
        var elapsed = Time.ElapsedTime;
        var cellMap = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());
        var pheromoneMap = EntityManager.GetBuffer<PheromoneMap>(GetSingletonEntity<PheromoneMap>());
        var foodSingleton = GetSingleton<Food>();

        UnityEngine.Time.timeScale = config.Speed;

        Entities
            .WithReadOnly(cellMap)
            .WithReadOnly(pheromoneMap)
            .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref AntMovement ant, ref URPMaterialPropertyBaseColor color, in LocalToWorld ltw) =>
            {
                // Initial jittering motion
                float jitterAngle = random.NextFloat(-config.RandomSteering, config.RandomSteering);
                ant.FacingAngle += jitterAngle;

                // Pheromone
                float pheroSteering = 0;
                var pheromoneHelper = new PheromoneMapHelper(pheromoneMap, config.CellMapResolution, config.WorldSize);
                pheroSteering = PheromoneSteering(pheromoneHelper, ant.FacingAngle, ltw.Position, (config.WorldSize/(float)config.CellMapResolution) * 3f);
                ant.FacingAngle += ant.HasSeenFood > 0? pheroSteering * config.PheromoneSteerStrength : pheroSteering * config.PheromoneSteerStrength * 0.1f;


                //Debug.Log(string.Format("{1} {0}", ant.FacingAngle, pheroSteering));

                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);

                int wallSteering = WallSteering(cellMapHelper, ant.FacingAngle, ltw.Position, (cellMapHelper.grid.worldDimLength/cellMapHelper.grid.gridDimLength) * 1.5f, /*time*/ 0);
                ant.FacingAngle += wallSteering * config.WallSteerStrength;

                float targetSpeed = config.MoveSpeed;
                float slowDownFactor = 1f - (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f;

                targetSpeed *= slowDownFactor;
                ant.AntSpeed += (targetSpeed - ant.AntSpeed) * config.AntAcceleration;

                if ((ant.State == AntState.LineOfSightToFood) || (ant.State == AntState.ReturnToNestWithLineOfSight))
                {
                    Vector2 targetPos;
                    if (ant.State == AntState.LineOfSightToFood)
                    {
                        targetPos = foodSingleton.Position;
                    }
                    else
                    {
                        targetPos.x = 0; // Nest assumed at center
                        targetPos.y = 0;
                    }

                    float targetAngle = Mathf.Atan2(targetPos.x - ltw.Position.x, targetPos.y - ltw.Position.z);

                    if (targetAngle - ant.FacingAngle > Mathf.PI)
                    {
                        ant.FacingAngle += Mathf.PI * 2f;
                    }
                    else if (targetAngle - ant.FacingAngle < -Mathf.PI)
                    {
                        ant.FacingAngle -= Mathf.PI * 2f;
                    }
                    else
                    {
                        if (Mathf.Abs(targetAngle - ant.FacingAngle) < Mathf.PI * .5f)
                            ant.FacingAngle += (targetAngle - ant.FacingAngle) * config.GoalSteerStrength;
                    }
                }

                var newRotation = quaternion.Euler(0, ant.FacingAngle, 0);

                //Debug.Log(string.Format("{0} {1} {2} {3}",
                //    ant.FacingAngle,
                //    jitterAngle,
                //    pheroSteering * config.PheromoneSteerStrength,
                //    wallSteering * config.WallSteerStrength));

                var newPosDelta = ltw.Forward * targetSpeed * time;
                var newPos = translation.Value + newPosDelta;
                bool turnAroundWall = false;
                bool turnAroundOther = false;

                var cellState = cellMapHelper.GetCellStateFrom2DPos(new float2(newPos.x, newPos.z));

                if (cellState == CellState.IsObstacle)
                {
                    turnAroundWall = true;
                }
                else if (cellState == CellState.IsFood && ant.State != AntState.ReturnToNest)
                {
                    ant.State = AntState.ReturnToNest;
                    color.Value = new float4(0.7250f, 0.7116f, 0.3973f, 1);
                    turnAroundOther = true;
                    ant.HasSeenFood = 1;

                }
                else if (cellState == CellState.IsNest && ant.State != AntState.Searching)
                {
                    ant.State = AntState.Searching;
                    color.Value = new float4(0.188f, 0.2108f, 0.3529f, 1);
                    turnAroundOther = true;
                }

                if (turnAroundWall)
                {
                    newPos = translation.Value - newPosDelta;
                    float reflectRandom = random.NextBool() ? -Mathf.PI / 8 * 1 : Mathf.PI / 8 * 1;
                    ant.FacingAngle += Mathf.PI + reflectRandom;
                    newRotation = quaternion.Euler(0, ant.FacingAngle, 0);
                }
                else if(turnAroundOther)
                {
                    newPos = translation.Value - newPosDelta;
                    ant.FacingAngle += Mathf.PI;
                    newRotation = quaternion.Euler(0, ant.FacingAngle, 0);
                }
                else
                {
                    // State transitions that will impact movement in the next call
                    if (ant.State == AntState.Searching && (cellState == CellState.HasLineOfSightToBoth || cellState == CellState.HasLineOfSightToFood))
                    {
                        ant.State = AntState.LineOfSightToFood;
                    }
                    else if (ant.State == AntState.ReturnToNest && (cellState == CellState.HasLineOfSightToBoth || cellState == CellState.HasLineOfSightToNest))
                    {
                        ant.State = AntState.ReturnToNestWithLineOfSight;
                    }
                }

                rotation.Value = newRotation;
                translation.Value = newPos;

                float excitement = config.PheromoneProductionPerSecond * time;
                if (ant.State == AntState.ReturnToNest || ant.State == AntState.ReturnToNestWithLineOfSight)
                {
                    excitement *= config.AntHasFoodPeromoneMultiplier;
                }
                ant.Excitement = excitement * ant.AntSpeed / config.MoveSpeed;
                ant.ActiveCellIndex = pheromoneHelper.grid.GetNearestIndex(new float2(newPos.x, newPos.z));
            }).ScheduleParallel();

        Entities
            .ForEach((Entity entity, ref AntMovement ant) =>
            {
                ref PheromoneMap ph = ref pheromoneMap.ElementAt(ant.ActiveCellIndex);
                ph.intensity = math.clamp(ph.intensity + ant.Excitement, 0, 1);
            }).Schedule();

        Dependency.Complete();
    }

    static float PheromoneSteering(PheromoneMapHelper pheromone, float facingAngle, float3 antPosition, float distance)
    {
        float output = 0;

        for (int i=-1;i<=1;i+=2)
        {
            float angle = facingAngle + i * Mathf.PI*.25f;
            float testX = antPosition.x + Mathf.Cos(angle) * distance;
            float testY = antPosition.z + Mathf.Sin(angle) * distance;

            var intensity = pheromone.GetPheromoneIntensityFrom2DPos(new float2(testX, testY));
            if (intensity != -1)
            {
                output += intensity*i;
            }
        }

        return output == 0f ? 0f : Mathf.Sign(output);
    }

    static int WallSteering(CellMapHelper cellMapHelper, float facingAngle, float3 antPosition, float distance, float delta)
    {
        // If wall detected to the right or left of the current direction then return -1 / 1 so that we steer away from it
        // If no wall then it returns 0
        // TBD: if wall detected in both checked directions then it continues towards collision, is that desirable?
        int output = 0;

        for (int i = -1; i <= 1; i+=2) {
            float angle = facingAngle + i * Mathf.PI*.25f;
            float testX = antPosition.x + Mathf.Cos(angle) * distance;
            float testY = antPosition.z + Mathf.Sin(angle) * distance;

            if (cellMapHelper.GetCellStateFrom2DPos(new float2(testX, testY), delta) == CellState.IsObstacle)
            {
                output -= i;
            }
        }
        return output;
    }

}
