using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class BeeStretchingAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float SpeedStretchSensitivity = 0.09f;
    public float MinStretch = 1f;
    public float MaxStretch = 2.5f;
    public float XStretchMultiplier = 0.6f;
    public float YStretchMultiplier = 0.1f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeStretchingConstants
        {
            SpeedStretchSensitivity = SpeedStretchSensitivity,
            MinStretch = MinStretch,
            MaxStretch = MaxStretch,
            XStretchMultiplier = XStretchMultiplier,
            YStretchMultiplier = YStretchMultiplier
        }); 
    }
}
