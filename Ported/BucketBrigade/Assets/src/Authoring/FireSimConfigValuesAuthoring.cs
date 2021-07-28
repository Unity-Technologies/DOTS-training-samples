using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace src.Components
{
    public class FireSimConfigValuesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public FireSimConfigValues settings = new FireSimConfigValues()
        {
            StartingFireCount = 1,
            MaxFlameHeight = 0.1f,
            CellSize = 0.05f,
            Rows = 20,
            Columns = 20,
            Flashpoint = 0.5f,
            HeatRadius = 1,
            HeatTransferRate = 0.7f,
            FireSimUpdateRate = 0.5f, 
            WorkerSpeed = 4f,
            WorkerSpeedWhenHoldingBucket = 2f,
            DistanceToPickupBucket = 1f, 
            WaterFillUpDuration = 4f, 
            WorkerCountPerTeam = 10
        };
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, settings);
        }
    }
}
    