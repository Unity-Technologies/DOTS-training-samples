using Unity.Entities;
using Unity.Rendering;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

namespace DOTSRATS
{
    public class ArrowAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
            dstManager.AddComponent<Arrow>(entity);
        }
    }
}