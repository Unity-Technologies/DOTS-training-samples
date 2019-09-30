using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace JumpTheGun {
    namespace Authoring {
        [DisallowMultipleComponent]
        [RequiresEntityConversion]
        public class Tank : MonoBehaviour, IConvertGameObjectToEntity {
            public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
                dstManager.AddComponent<BlockIndex>(entity);
                dstManager.AddComponent<BlockPositionXZ>(entity);
                dstManager.AddComponent<BlockHeight>(entity);
                dstManager.AddComponent<UpdateBlockTransformTag>(entity);
                dstManager.AddComponent<MaterialColor>(entity);
            }
        }
    }
}
