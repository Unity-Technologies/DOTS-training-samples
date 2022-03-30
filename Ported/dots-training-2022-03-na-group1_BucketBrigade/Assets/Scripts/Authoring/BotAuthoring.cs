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
        Captain,
        INVALID,
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
        dstManager.AddComponent<RelocatePosition>(entity);
        dstManager.AddComponent<BucketHeld>(entity);
        dstManager.AddComponent<BucketToWant>(entity);
        dstManager.AddComponent<MyWorkerState>(entity);
        dstManager.AddComponent<Speed>(entity);
        
        switch (Type)
        {
            case WorkerType.Fetcher:
                dstManager.AddComponent<FetcherTag>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                dstManager.AddComponent<MyWaterPool>(entity);
                dstManager.AddComponent<Home>(entity);
                dstManager.AddComponent<MyTeam>(entity);
                break;
            
            case WorkerType.Omniworker:
                dstManager.AddComponent<OmniworkerTag>(entity);
                dstManager.AddComponent<MyWaterPool>(entity);
                break;
            
            case WorkerType.Captain:
                dstManager.AddComponent<CaptainTag>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                dstManager.AddComponent<Home>(entity);
                dstManager.AddComponent<MyTeam>(entity);
                dstManager.AddComponent<EvalOffsetFrame>(entity);
                break;
            
            case WorkerType.EmptyBucketHauler:
            case WorkerType.FullBucketHauler:
                dstManager.AddComponent<WorkerTag>(entity);
                dstManager.AddComponent<DestinationWorker>(entity);
                dstManager.AddComponent<Home>(entity);
                break;
        }
    }
}
