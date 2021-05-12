using Unity.Transforms;
using Unity.Entities;
using UnityEngine;

public class ScaleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float scale = 1.0f;

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Scale()
        {
            Value = scale
        });
    }
}
