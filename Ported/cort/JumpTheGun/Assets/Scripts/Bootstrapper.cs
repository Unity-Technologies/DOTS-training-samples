using JumpTheGun;
using Unity.Entities;
using UnityEngine;

public class Bootstrapper : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<NewGameTag>(entity);
    }
}
