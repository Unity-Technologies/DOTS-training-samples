using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
namespace HighwayRacers
{
    [UpdateAfter(typeof(CarSelectionSystem))]
    [UpdateAfter(typeof(CarMovementSystem))]
    public partial struct CameraSystem : ISystem
    {
        private TransformAspect.EntityLookup m_TransformFromEntity;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectedCar>();
            m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //m_TransformFromEntity.Update(ref state);

            //var sc = SystemAPI.GetSingleton<SelectedCar>();
            //if (sc.Selected != Entity.Null)
            //{
            //    // how to set active?
            //    var selectedCarTransform = m_TransformFromEntity[state.EntityManager
            //        .GetComponentData<CarCameraPoint>(sc.Selected).CameraPoint];
            //    CameraManager.Instance.SetCarCameraTransform(selectedCarTransform.Rotation,
            //        selectedCarTransform.Position);
            //    CameraManager.Instance.ToCarView();
            //}
            //else
            //{
            //    CameraManager.Instance.ToTopDownView();
            //}
        }
    }
}