using UnityEngine;
using Unity.Entities;

public class CarriageAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private int index = 0;

    [SerializeField] private int trainId = 0;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CarriageComponent() { Index = index } );
        dstManager.AddSharedComponentData(entity, new TrainIDComponent() { Value = trainId });
    }
}