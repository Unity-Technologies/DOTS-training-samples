using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [SerializeField] private int trainId = 0;
    [SerializeField] private float maxSpeed = .2f;
    //[SerializeField] private int carriageCount = 4;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp); 
        
        dstManager.AddComponent<TrainComponent>(entity);
        dstManager.AddSharedComponentData(entity, new TrainIDComponent() { Value = trainId });
        dstManager.AddComponentData<SpeedComponent>(entity, new SpeedComponent() { Value = maxSpeed });
        
        dstManager.AddComponent<TrackPositionComponent>(entity);
        dstManager.AddComponent<WaypointIndexComponent>(entity);

        //for (int i = 0; i < carriageCount; i++)
        //{
        //    Entity carriage = ecb.Instantiate(conversionSystem.GetPrimaryEntity(carriagePrefab));
        //    var carriageComponent = new CarriageComponent() {Index = i, Train = entity};
        //    ecb.AddComponent(carriage, carriageComponent);
        //}
        
        //ecb.Playback(dstManager);
        //ecb.Dispose();
    }
}
