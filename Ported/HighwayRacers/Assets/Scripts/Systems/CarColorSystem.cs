using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Profiling;
using Unity.Rendering;
using UnityEngine;

[UpdateAfter(typeof(CarSpawningSystem))]
[BurstCompile]
public partial struct CarColorSystem : ISystem
{
    [BurstCompile]
    partial struct UpdateColorsJob : IJobEntity
    {
        [ReadOnly] public CarGlobalColors CarColors;
        public EntityCommandBuffer ECB;
        public EntityQueryMask QueryMask;
        [ReadOnly] public bool Preview;

        void Execute(ref UpdateColorAspect car)
        {
            if (car.Preview)
            {
                ECB.SetComponentForLinkedEntityGroup(car.Entity, QueryMask, new URPMaterialPropertyBaseColor { Value = (Vector4)Color.magenta });
            }
            else if (car.SecondaryPreview)
            {
                ECB.SetComponentForLinkedEntityGroup(car.Entity, QueryMask, new URPMaterialPropertyBaseColor { Value = (Vector4)Color.cyan });
            }
            else
            {
                Color color = Color.white;
                if (car.CurrentSpeed > car.DesiredSpeed || (car.DistanceAhead > car.MinDistanceInFront && car.CurrentSpeed < car.DesiredSpeed))
                {
                    color = CarColors.fastColor;
                }
                else if (car.DesiredSpeed > car.CurrentSpeed)
                {
                    color = CarColors.slowColor;
                }
                else
                {
                    color = CarColors.defaultColor;
                }

                if (color != car.CurrentColor)
                {
                    car.CurrentColor = color;
                    ECB.SetComponentForLinkedEntityGroup(car.Entity, QueryMask, new URPMaterialPropertyBaseColor { Value = (Vector4)(color / (Preview ? 4f : 1f)) });
                }
            }
        }
    }

    private EntityQuery m_BaseColorQuery;
    public void OnCreate(ref SystemState state)
    {        
        m_BaseColorQuery = state.GetEntityQuery(typeof(URPMaterialPropertyBaseColor));
        state.RequireForUpdate<CarGlobalColors>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        bool preview = false;
        foreach (var car in SystemAPI.Query<CarAspect>())
        {
            if (car.Preview)
            {
                preview = true;
                if (car.CarInFront != Entity.Null)
                {
                    state.EntityManager.SetComponentData(car.CarInFront, new CarPreview()
                    {
                        Preview = false,
                        SecondaryPreview = true
                    });
                }
            }
            else
            {
                if (car.CarInFront != Entity.Null)
                {
                    state.EntityManager.SetComponentData(car.CarInFront, new CarPreview()
                    {
                        Preview = state.EntityManager.GetComponentData<CarPreview>(car.CarInFront).Preview,
                        SecondaryPreview = false
                    });
                }
            }
        }

        var carColors = SystemAPI.GetSingleton<CarGlobalColors>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        var updateColorsJob = new UpdateColorsJob
        {
            CarColors = carColors,
            ECB = ecb,
            QueryMask = queryMask,
            Preview = preview
        };
        // Schedule execution in a single thread, and do not block main thread.
        updateColorsJob.Schedule();
    }
}
