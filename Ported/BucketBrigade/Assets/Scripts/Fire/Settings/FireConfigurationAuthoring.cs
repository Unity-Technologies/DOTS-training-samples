using Unity.Entities;
using UnityEngine;

public struct FireConfiguration : IComponentData
{
    public float CellSize;
    public int GridWidth;
    public int GridHeight;

    public int NumInitialFires;

    // Fire simulation
    public float FlashPoint;
    public float MaxTemperature;
    public int HeatRadius;
    public float HeatTransferRate;
    public float FireSimUpdateRate;
}

public class FireConfigurationAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public float CellSize = 0.3f;
    public int GridWidth = 50;
    public int GridHeight = 50;
    
    public int NumInitialFires = 5;
    public float FlashPoint = 0.2f;
    public float MaxTemperature = 1f;
        
    [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usafge")]
    public int HeatRadius = 1;

    public float HeatTransferRate = 0.7f;

    [Range(0.0001f,2f)]
    [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
    public float FireSimUpdateRate = 0.5f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new FireConfiguration
        {
            CellSize = CellSize,
            GridWidth = GridWidth,
            GridHeight = GridHeight,
            NumInitialFires = NumInitialFires,
            FlashPoint = FlashPoint,
            MaxTemperature = MaxTemperature,
            HeatRadius = HeatRadius,
            HeatTransferRate = HeatTransferRate,
            FireSimUpdateRate = FireSimUpdateRate
        });
    }
}
