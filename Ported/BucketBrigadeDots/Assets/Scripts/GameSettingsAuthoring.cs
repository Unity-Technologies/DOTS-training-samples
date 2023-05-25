using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour
{
    // Number of rows and columns: it's a square.
    public int GridWidth = 100;
    public int StartingFires = 4;
    public float MaxHeat = 1f;
    public float FlashPoint = .2f;
    public int HeatRadius = 2;
    public float HeatTransferRate = 0.0003f;
    public float FireSimUpdateRate = 0.01f;
    public Color BucketEmptyColor = new Color32(171, 105, 33, 255);
    public Color BucketFullColor = new Color32(16, 120, 167, 255);
    public Color WorkerFullColor = new Color32(197, 236, 188, 255);
    public Color WorkerEmptyColor = new Color32(238, 192, 236, 255);
    public Color RunnerWorkerColor = new Color32(0, 255, 39, 255);
    public Color WorkerOmniColor = new Color32(32, 32, 32, 255);

    class Baker : Baker<GameSettingsAuthoring>
    {
        public override void Bake(GameSettingsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameSettings
            {
                RowsAndColumns = authoring.GridWidth,
                StartingFires = authoring.StartingFires,
                DefaultGridSize = 0.3f,
                MaxHeat = authoring.MaxHeat,
                FlashPoint = authoring.FlashPoint,
                HeatRadius = authoring.HeatRadius,
                HeatTransferRate = authoring.HeatTransferRate,
                FireSimUpdateRate = authoring.FireSimUpdateRate,
                BucketEmptyColor = authoring.BucketEmptyColor.ToFloat4(),
                BucketFullColor = authoring.BucketFullColor.ToFloat4(),
                WorkerEmptyColor = authoring.WorkerEmptyColor.ToFloat4(),
                WorkerFullColor = authoring.WorkerFullColor.ToFloat4(),
                RunnerWorkerColor = authoring.RunnerWorkerColor.ToFloat4(),
                WorkerOmniColor = authoring.WorkerOmniColor.ToFloat4()
            });
        }
    }
}

public struct GameSettings : IComponentData
{
    public int RowsAndColumns;
    public int StartingFires;
    public float DefaultGridSize;
    public float MaxHeat;
    public float FlashPoint;
    public int HeatRadius;
    public float HeatTransferRate;
    public float FireSimUpdateRate;
    public float4 BucketEmptyColor;
    public float4 BucketFullColor;
    public float4 WorkerEmptyColor;
    public float4 WorkerFullColor;
    public float4 RunnerWorkerColor;
    public float4 WorkerOmniColor;

    public int Size => RowsAndColumns * RowsAndColumns;
}