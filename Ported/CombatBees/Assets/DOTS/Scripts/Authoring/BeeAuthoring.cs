using Unity.Entities;
using UnityEngine;

public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<IsBee>(entity);
        dstManager.AddComponent<Team>(entity);
        dstManager.AddComponent<Velocity>(entity);
        dstManager.AddComponent<Speed>(entity);
        dstManager.AddComponent<TargetPosition>(entity);
        dstManager.AddComponent<Aggression>(entity);
        dstManager.AddComponent<IsOriented>(entity);
        dstManager.AddComponent<IsStretched>(entity);
    }
}
