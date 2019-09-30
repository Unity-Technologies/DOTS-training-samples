using Unity.Entities;
using UnityEngine;

namespace JumpTheGun
{
    namespace Authoring
    {
        [DisallowMultipleComponent]
        [RequiresEntityConversion]
        public class Cannonball : MonoBehaviour, IConvertGameObjectToEntity
        {
            public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponent<CannonballArcState>(entity);
            }
        }
    }
}