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
        public Entity BucketFillerPrefab;
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

        [Tooltip("How many workers are fetching the buckets Per Team")]
        public int BucketFetcherCountPerTeam;

        [Tooltip("How many buckets we have on the map")]
        public int BucketCount;

        [Header("WATER")]
        [Tooltip("Number of cells affected by a bucket of water")]
        public int SplashRadius;

        [Tooltip("Water bucket reduces fire temperature by this amount")]
        public float CoolingStrength;

        [Tooltip("Splash damage of water bucket. (1 = no loss of power over distance)")]
        public float CoolingStrength_Falloff;

        [Tooltip("Team Assignments are expensive. We can artificially limit the total number here to prevent spam.")]
        [Range(0, 100)]
        public int MaxTeamAssignmentsPerFrame;

        public float GetTemperatureForCell(in DynamicBuffer<Temperature> temperatureBuffer, int column, int row)
        {
            if (row < 0 || column < 0 || row >= Rows || column >= Columns)
                return 0;

            int cellIndex = column + row * Columns;

            return temperatureBuffer[cellIndex].Intensity;
        }

        public void SetTemperatureForCell(ref DynamicBuffer<Temperature> temperatureBuffer, int column, int row, float temperature)
        {
            if (row >= 0 && column >= 0 && row < Rows && column < Columns)
            {
                int cellIndex = GetCellIdOfColRow(column, row);

                temperatureBuffer[cellIndex] = new Temperature {Intensity = temperature};
            }
        }

        public int GetCellIdOfColRow(int column, int row) => column + row * Columns;

        public int2 GetColRowOfCell(int cellId)
        {
            var colRowOfCell = new int2(cellId % Columns, cellId / Columns);
            Debug.Assert(GetCellIdOfColRow(colRowOfCell.x, colRowOfCell.y) == cellId, "'GetColRowOfCell' does not match 'GetCellIdOfRowCol'!");
            return colRowOfCell;
        }

        public int GetCellIdOfPosition2D(float2 pos) => GetCellIdOfColRow(Mathf.FloorToInt(pos.x / CellSize), Mathf.FloorToInt(pos.y / CellSize));

        public int2 GetColRowOfPosition2D(float2 pos)
        {
            int column = Mathf.FloorToInt(pos.x / CellSize);
            int row = Mathf.FloorToInt(pos.y / CellSize);

            return new int2(column, row);
        }

        public float2 GetPosition2DOfColRow(int column, int row) => new float2(column, row) * CellSize;

        /// <summary>
        ///     Assumes world map starts at 0, 0.
        /// </summary>
        public float2 GetCellWorldPosition2D(int cellId)
        {
            var colRow = GetColRowOfCell(cellId);
            var cellWorldPosition = GetPosition3DOfCellColRow(colRow.x, colRow.y);
            // Ignore Y axis when converting back to 2D.
            return new float2(cellWorldPosition.x, cellWorldPosition.z);
        }

        /// <summary>
        ///     Assumes world map starts at 0, 0.
        /// </summary>
        public float3 GetPosition3DOfCellColRow(int col, int row, float3 origin = default) => origin + new float3(col, 0f, row) * CellSize;

        public int2 GetCellColRowOfPosition3D(float3 worldPosition, float3 origin = default)
        {
            var localPosition = worldPosition - origin;
            // Convert to grid position
            var row = (int)(localPosition.z / CellSize);
            var col = (int)(localPosition.x / CellSize);
            if (localPosition.z < 0)
                row--;
            if (localPosition.x < 0)
                col--;
            return new int2(col, row);
        }


        public bool IsOnFire(Temperature temperature)
        {
            return temperature.Intensity > Flashpoint;
        }
    }
}