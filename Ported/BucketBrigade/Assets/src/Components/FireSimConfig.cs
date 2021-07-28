using System;
using src.Systems;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Components
{
    /// <summary>
    ///     All config data for the simulation.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct FireSimConfig : IComponentData
    {
        public Entity FullBucketPasserWorkerPrefab;
        public Entity EmptyBucketPasserWorkerPrefab;
        public Entity BucketFetcherPrefab;
        public Entity BucketThrowerWorkerPrefab;
        public Entity OmniWorkerPrefab;
        
        public Entity BucketPrefab;

        public Entity FireCellPrefab;
    }

    [Serializable]
    public struct FireSimConfigValues : IComponentData
    {
        // Fire config
        [Tooltip("How many random fires do you want to battle?")]
        public int StartingFireCount;

        [Tooltip("How high the flames reach at max temperature")]
        public float MaxFlameHeight;

        [Tooltip("Size of an individual flame. Full grid will be (rows * cellSize)")]
        public float CellSize;

        [Tooltip("How many cells WIDE the simulation will be")]
        public int Rows;

        [Tooltip("How many cells DEEP the simulation will be")]
        public int Columns;

        [Tooltip("When temperature reaches *flashpoint* the cell is on fire")]
        public float Flashpoint;

        [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usafge")]
        public int HeatRadius;

        [Tooltip("How fast will adjascent cells heat up?")]
        public float HeatTransferRate;

        [Range(0.0001f, 2f)] [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
        public float FireSimUpdateRate;

        [Range(0.0001f, 100f)] public float WorkerSpeed;
        [Range(0.0001f, 100f)] public float WorkerSpeedWhenHoldingBucket;

        [Range(0.0001f, 5f)] public float DistanceToPickupBucket;

        [Range(0.0001f, 50f)] public float WaterFillUpDuration;

        [Tooltip("How many workers for full and empty bucket passers we have")]
        public int WorkerCountPerTeam;
        
        [Tooltip("How many teams we have")]
        [Min(0)] public int TeamCount;

        [Tooltip("How many workers are fetching the buckets")]
        public int BucketFetcherCount;

        [Tooltip("How many buckets we have on the map")]
        public int BucketCount;

        public float GetTemperatureForCell(in DynamicBuffer<Temperature> temperatureBuffer, int row, int column)
        {
            if (row < 0 || column < 0 || row >= Rows || column >= Columns)
                return 0;

            int cellIndex = column + row * Columns;

            return temperatureBuffer[cellIndex].Intensity;
        }

        public void SetTemperatureForCell(ref DynamicBuffer<Temperature> temperatureBuffer, int row, int column, float temperature)
        {
            if (row >= 0 && column >= 0 && row < Rows && column < Columns)
            {
                int cellIndex = GetCellIdOfRowCol(row, column);

                temperatureBuffer[cellIndex] = new Temperature {Intensity = temperature};
            }
        }

        public int GetCellIdOfRowCol(int row, int column) => column + row * Columns;

        public int2 GetRowColOfCell(int cellId)
        {
            var rowColOfCell = new int2(cellId / Columns, cellId % Columns);
            Debug.Assert(GetCellIdOfRowCol(rowColOfCell.x, rowColOfCell.y) == cellId, "'GetRowColOfCell' does not match 'GetCellIdOfRowCol'!");
            return rowColOfCell;
        }

        public int GetCellIdOfPosition2D(float2 pos) => GetCellIdOfRowCol(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        /// <summary>
        ///     Assumes world map starts at 0, 0.
        /// </summary>
        public float2 GetCellWorldPosition2D(int cellId)
        {
            var rowCol = GetRowColOfCell(cellId);
            var cellWorldPosition = GetCellWorldPosition3D(rowCol.x, rowCol.y);
            // Ignore Y axis when converting back to 2D.
            return new float2(cellWorldPosition.x, cellWorldPosition.z);
        }

        /// <summary>
        ///     Assumes world map starts at 0, 0.
        /// </summary>
        public float3 GetCellWorldPosition3D(int row, int col, float3 origin = default) => origin + new float3(col, 0f, row) * CellSize;

        public int2 GetCellRowCol(float3 worldPosition, float3 origin = default)
        {
            var localPosition = worldPosition - origin;
            // Convert to grid position
            var row = (int)(localPosition.z / CellSize);
            var col = (int)(localPosition.x / CellSize);
            if (localPosition.z < 0)
                row--;
            if (localPosition.x < 0)
                col--;
            return new int2(row, col);
        }
    }
}