using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class HomebaseAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Players Player;

    // The MonoBehaviour data is converted to ComponentData on the entity.
    // We are specifically transforming from a good editor representation of the data (Represented in degrees)
    // To a good runtime representation (Represented in radians)
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LbPlayer { Value = (byte)Player });
        dstManager.AddComponentData(entity, new LbPlayerScore { Value = 0 });
    }
}
