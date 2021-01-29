using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class CarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        var allRenderers = transform.GetComponentsInChildren<MeshRenderer>();
        var needBaseColor = new NativeArray<Entity>(allRenderers.Length, Allocator.Temp);

        for(int i = 0; i < allRenderers.Length; ++i)
        {
            var gameObject = allRenderers[i].gameObject;
            needBaseColor[i] = conversionSystem.GetPrimaryEntity(gameObject);
        }

        // We could have used AddComponent in the loop above, but as a general rule in
        // DOTS, doing a batch of things at once is more efficient.
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(needBaseColor);
        dstManager.AddComponent<CarMovement>(entity);
    }
}
