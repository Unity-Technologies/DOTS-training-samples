using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct FireConfiguration : IComponentData
{
    // Fire initialization
    public float3 Origin;
    public float CellSize;
    public int GridWidth;
    public int GridHeight;

    public int NumInitialFires;
    public float InitialFireTemperature;
    
    // Fire simulation
    public float FlashPoint;
    public float MaxTemperature;
    public int HeatRadius;
    public float HeatTransferRate;
    public float FireSimUpdateRate;
    
    // Fire rendering
    public float4 DefaultColor;
    public float4 LowFireColor;
    public float4 HigHFireColor;
    public float MaxFireHeight;

    public float FlickerRange;
    public float FlickerRate;
}

public class FireConfigurationAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    // Fire initialization
    public Vector3 Origin = Vector3.zero;
    public float CellSize = 0.3f;
    public int GridWidth = 50;
    public int GridHeight = 50;
    
    public int NumInitialFires = 5;
    [Range(0.2f, 1f)]
    public float InitialFireTemperature = 1;
    
    // Fire simulation
    public float FlashPoint = 0.2f;
    public float MaxTemperature = 1f;
        
    [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usafge")]
    public int HeatRadius = 1;

    public float HeatTransferRate = 0.7f;

    [Range(0.0001f,2f)]
    [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
    public float FireSimUpdateRate = 0.5f;
    
    // Fire rendering
    public Color DefaultColor;
    public Color LowFireColor;
    public Color HigHFireColor;

    [Range(0.01f,1f)]
    public float MaxFireHeight = 0.2f;

    public float FlickerRange = 0.4f;
    public float FlickerRate = 0.5f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new FireConfiguration
        {
            Origin = Origin,
            CellSize = CellSize,
            GridWidth = GridWidth,
            GridHeight = GridHeight,
            NumInitialFires = NumInitialFires,
            InitialFireTemperature = InitialFireTemperature, 
            FlashPoint = FlashPoint,
            MaxTemperature = MaxTemperature,
            HeatRadius = HeatRadius,
            HeatTransferRate = HeatTransferRate,
            FireSimUpdateRate = FireSimUpdateRate,
            DefaultColor = ColorToFloat4(DefaultColor),
            LowFireColor = ColorToFloat4(LowFireColor),
            HigHFireColor = ColorToFloat4(HigHFireColor),
            MaxFireHeight = MaxFireHeight,
            FlickerRange = FlickerRange,
            FlickerRate = FlickerRate
        });
    }

    public float4 ColorToFloat4(UnityEngine.Color c)
    {
        return new float4(c.r, c.g, c.b, c.a);
    }
}
