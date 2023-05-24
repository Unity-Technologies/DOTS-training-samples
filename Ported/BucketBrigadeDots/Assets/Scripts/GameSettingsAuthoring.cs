using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour
{
    // Number of rows and columns: it's a square.
    public int GridWidth = 100;
    public int StartingFires = 4;
    public Color BucketEmptyColor = new Color32(171, 105, 33, 255);
    public Color BucketFullColor = new Color32(16, 120, 167, 255);
    public Color WorkerFullColor = new Color32(197, 236, 188, 255);
    public Color WorkerEmptyColor = new Color32(238, 192, 236, 255);
    public Color RunnerWorkerColor = new Color32(0, 255, 39, 255);

    class Baker : Baker<GameSettingsAuthoring>
    {
        public override void Bake(GameSettingsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameSettings
            {
                RowsAndColumns = authoring.GridWidth,
                StartingFires = authoring.StartingFires,
                BucketEmptyColor = authoring.BucketEmptyColor.ToFloat4(),
                BucketFullColor = authoring.BucketFullColor.ToFloat4(),
                WorkerEmptyColor = authoring.WorkerEmptyColor.ToFloat4(),
                WorkerFullColor = authoring.WorkerFullColor.ToFloat4(),
                RunnerWorkerColor = authoring.RunnerWorkerColor.ToFloat4()
            });
        }
    }
}

public struct GameSettings : IComponentData
{
    public int RowsAndColumns;
    public int StartingFires;
    public float4 BucketEmptyColor;
    public float4 BucketFullColor;
    public float4 WorkerEmptyColor;
    public float4 WorkerFullColor;
    public float4 RunnerWorkerColor;

    public int Size => RowsAndColumns * RowsAndColumns;
}