using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

public class BotAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public enum WorkerType
    {
        Fetcher,
        FireCaptain,
        WaterCaptain,
        FullBucketHauler,
        EmptyBucketHauler,
        Omniworker
    }

    public WorkerType Type;
    
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        var allRenderers = transform.GetComponentsInChildren<UnityMeshRenderer>();
        var needBaseColor = new NativeArray<Entity>(allRenderers.Length, Allocator.Temp);

        for(int i = 0; i < allRenderers.Length; ++i)
        {
            var meshRenderer = allRenderers[i];
            needBaseColor[i] = conversionSystem.GetPrimaryEntity(meshRenderer.gameObject);
        }

        // We could have used AddComponent in the loop above, but as a general rule in
        // DOTS, doing a batch of things at once is more efficient.

        dstManager.RemoveComponent<Rotation>(entity);

        dstManager.AddComponent<Position>(entity);
        dstManager.AddComponent<Destination>(entity);
        dstManager.AddComponent<WorkerTag>(entity);
        dstManager.AddComponent<HoldingWhichBucket>(entity);
        dstManager.AddComponent<MyWorkerState>(entity);

        switch (Type)
        {
            case WorkerType.Fetcher:
                dstManager.AddComponent<FetcherTag>(entity);
                dstManager.AddComponent<MyWaterCaptain>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                break;
            
            case WorkerType.Omniworker:
                // todo
                break;
            
            case WorkerType.FireCaptain:
                dstManager.AddComponent<FireCaptainTag>(entity);
                dstManager.AddComponent<MyWaterCaptain>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                break;
            
            case WorkerType.WaterCaptain:
                dstManager.AddComponent<WaterCaptainTag>(entity);
                dstManager.AddComponent<SourcePool>(entity);
                dstManager.AddComponent<SourcePosition>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                break;
            
            case WorkerType.EmptyBucketHauler:
                dstManager.AddComponent<BucketReturnerTag>(entity);
                dstManager.AddComponent<MyFireCaptain>(entity);
                dstManager.AddComponent<MyWaterCaptain>(entity);
                dstManager.AddComponent<RelocationPosition>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                break;
            
            case WorkerType.FullBucketHauler:
                dstManager.AddComponent<WaterHaulerTag>(entity);
                dstManager.AddComponent<MyFireCaptain>(entity);
                dstManager.AddComponent<MyWaterCaptain>(entity);
                dstManager.AddComponent<RelocationPosition>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                break;
        }
    }
}
