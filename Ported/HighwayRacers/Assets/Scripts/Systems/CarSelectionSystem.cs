using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateAfter(typeof(CarSpawningSystem))]
    public partial struct CarSelectionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectedCar>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            SelectedCar sc = SystemAPI.GetSingleton<SelectedCar>();
            Entity sce = SystemAPI.GetSingletonEntity<SelectedCar>();
            //foreach (var car in SystemAPI.Query<CarAspect>())
            //{
            //    state.EntityManager.SetComponentData<SelectedCar>(sce, new()
            //    {
            //        Selected = car.Entity
            //    });

            //    break;
            //}
        }
    }
}