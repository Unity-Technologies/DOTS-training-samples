using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace src.Components
{
    public class FireSimConfigValuesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        // Fire config
        [Header("FIRE")]
        [Tooltip("How many random fires do you want to battle?")]
        public int StartingFireCount = 1;
        [Tooltip("How high the flames reach at max temperature")]
        public float MaxFlameHeight = 0.1f;
        [Tooltip("Size of an individual flame. Full grid will be (rows * cellSize)")]
        public float CellSize = 0.05f;
        [Tooltip("How many cells WIDE the simulation will be")]
        public int Rows = 20;
        [Tooltip("How many cells DEEP the simulation will be")]
        public int Columns = 20;
        [Tooltip("When temperature reaches *flashpoint* the cell is on fire")]
        public float Flashpoint = 0.5f;
        [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usafge")]
        public int HeatRadius = 1;
        [Tooltip("How fast will adjascent cells heat up?")]
        public float HeatTransferRate = 0.7f;

        [Range(0.0001f, 2f)]
        [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
        public float FireSimUpdateRate = 0.5f;


        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FireSimConfigValues
            {
                StartingFireCount = StartingFireCount,
                MaxFlameHeight = MaxFlameHeight,
                CellSize = CellSize,
                Rows = Rows,
                Columns = Columns,
                Flashpoint = Flashpoint,
                HeatRadius = HeatRadius,
                HeatTransferRate = HeatTransferRate,
                FireSimUpdateRate = FireSimUpdateRate
            });
        }
    }
}
    