using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

namespace DOTSRATS
{
    public class GoalAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            var allRenderers = transform.GetComponentsInChildren<UnityMeshRenderer>();
            var needBaseColor = new NativeArray<Entity>(allRenderers.Length, Allocator.Temp);

            for (int i = 0; i < allRenderers.Length; ++i)
            {
                var meshRenderer = allRenderers[i];
                needBaseColor[i] = conversionSystem.GetPrimaryEntity(meshRenderer.gameObject);
            }
            
            dstManager.AddComponent<URPMaterialPropertyBaseColor>(needBaseColor);
            dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
            dstManager.AddComponent<Goal>(entity);
        }
    }
}