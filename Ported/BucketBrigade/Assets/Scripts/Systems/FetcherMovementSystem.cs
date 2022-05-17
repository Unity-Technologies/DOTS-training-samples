using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


[BurstCompile]
partial struct FetcherMovementSystem : ISystem
{  
    private ComponentDataFromEntity<LocalToWorld> m_LocalToWorldFromEntity;
    private TransformAspect.EntityLookup m_TransformFromEntity;
    
    private EntityQuery m_FetcherPickUpQuery;
    private EntityQuery m_FetcherDropZoneQuery;

    private Random m_Random;
    
    public void OnCreate(ref SystemState state)
    {       
        m_Random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        m_LocalToWorldFromEntity = state.GetComponentDataFromEntity<LocalToWorld>(true);
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
        m_FetcherPickUpQuery = state.GetEntityQuery(typeof(FetcherTarget), typeof(Translation));
        m_FetcherDropZoneQuery = state.GetEntityQuery(typeof(FetcherDropZone), typeof(Translation));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_LocalToWorldFromEntity.Update(ref state);
        m_TransformFromEntity.Update(ref state);
      
        var fetcherTargets = m_FetcherPickUpQuery.ToEntityArray(Allocator.Temp);
        var fetcherTargetTranslations = m_FetcherPickUpQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        
        var fetcherDropZones = m_FetcherDropZoneQuery.ToEntityArray(Allocator.Temp);
        var fetcherDropZoneTranslations = m_FetcherDropZoneQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        foreach (var fetcher in SystemAPI.Query<FetcherAspect>())
        {
            FindClosestPickUp(fetcher, fetcherTargets, fetcherTargetTranslations);
            FindClosestDropZone(fetcher, fetcherDropZones, fetcherDropZoneTranslations);
            MoveTowardsTarget(fetcher, ref state);
        }
    }

    private void FindClosestPickUp(FetcherAspect fetcher, NativeArray<Entity> fetcherTargets, NativeArray<Translation> fetcherTargetTranslations)
    {
        if (fetcher.TargetPickUp == Entity.Null)
        {
            if(fetcherTargetTranslations.Length > 0)
            {
                int closestIdx = 0;
                float closestDistance = float.MaxValue;
                for (int idx = 0; idx < fetcherTargets.Length; idx++)
                {
                    Translation currentTranslation = fetcherTargetTranslations[idx];
                    var distance = math.distance(fetcher.Position, currentTranslation.Value);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestIdx = idx;
                    }
                }
                fetcher.TargetPickUp = fetcherTargets[closestIdx];
            }
        }
    }
    
    // TODO: Merge FindClosestPickUp & FindClosestDropZone
    private void FindClosestDropZone(FetcherAspect fetcher, NativeArray<Entity> fetcherDropZones, NativeArray<Translation> fetcherDropZoneTranslations)
    {
        if (fetcher.TargetDropZone == Entity.Null)
        {
            if(fetcherDropZoneTranslations.Length > 0)
            {
                int closestIdx = 0;
                float closestDistance = float.MaxValue;
                for (int idx = 0; idx < fetcherDropZones.Length; idx++)
                {
                    Translation currentTranslation = fetcherDropZoneTranslations[idx];
                    var distance = math.distance(fetcher.Position, currentTranslation.Value);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestIdx = idx;
                    }
                }
                fetcher.TargetDropZone = fetcherDropZones[closestIdx];
            }
        }
    }

    private void MoveTowardsTarget(FetcherAspect fetcher, ref SystemState state)
    {
        float3 targetPos = float3.zero;
        if (fetcher.CurrentState == FetcherState.FetchingBucket)
        {
            targetPos = m_LocalToWorldFromEntity[fetcher.TargetPickUp].Position * state.Time.DeltaTime;
        }
        else if (fetcher.CurrentState == FetcherState.MoveTowardsWater)
        {
            targetPos = m_LocalToWorldFromEntity[fetcher.TargetDropZone].Position * state.Time.DeltaTime;
        }
        m_TransformFromEntity[fetcher.Self].TranslateWorld(targetPos);
    }
}
