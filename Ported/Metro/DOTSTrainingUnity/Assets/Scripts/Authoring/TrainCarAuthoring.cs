using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class TrainCarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int trainCarIndex = 0;
    public UnityEngine.Color color;
    public GameObject theTrainEngine;
    public GameObject doorLeft;
    public GameObject doorRight;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponents(entity, new ComponentTypes(typeof(TrainCarIndex), typeof(TrainEngineRef), typeof(Color), typeof(PropagateColor)));
        dstManager.SetComponentData(entity, new TrainCarIndex(){value = trainCarIndex});
        
        Entity trainEntity = conversionSystem.GetPrimaryEntity(theTrainEngine);
        dstManager.SetComponentData(entity, new TrainEngineRef(){value = trainEntity});
        
        dstManager.SetComponentData(entity, new Color() {value = new float4(color.r, color.g, color.b, color.a)});
        dstManager.AddComponentData(entity, new DoorsRef() { doorEntLeft = conversionSystem.GetPrimaryEntity(doorLeft), doorEntRight = conversionSystem.GetPrimaryEntity(doorRight)});

        for (int childIdx = 0; childIdx < transform.childCount; ++childIdx)
        {
            Transform child = transform.GetChild(childIdx);
            if (child.gameObject != doorLeft && child.gameObject != doorRight)
            {
                var allRenderers = child.GetComponentsInChildren<MeshRenderer>();
                var needBaseColor = new NativeArray<Entity>(allRenderers.Length, Allocator.Temp);
                for(int i = 0; i < allRenderers.Length; ++i)
                {
                    var meshRenderer = allRenderers[i];
                    needBaseColor[i] = conversionSystem.GetPrimaryEntity(meshRenderer.gameObject);
                }
                
                // We could have used AddComponent in the loop above, but as a general rule in
                // DOTS, doing a batch of things at once is more efficient.
                dstManager.AddComponent<URPMaterialPropertyBaseColor>(needBaseColor);
                needBaseColor.Dispose();
            }
        }
        
        
       
    }
}
