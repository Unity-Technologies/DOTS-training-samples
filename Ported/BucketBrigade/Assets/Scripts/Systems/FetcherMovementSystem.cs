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
    
    private EntityQuery m_FetcherTargetQuery;
    private EntityQuery m_WaterTargetQuery;

    private Random m_Random;
    
    public void OnCreate(ref SystemState state)
    {       
        m_Random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        m_LocalToWorldFromEntity = state.GetComponentDataFromEntity<LocalToWorld>(true);
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
        m_FetcherTargetQuery = state.GetEntityQuery(typeof(FetcherTarget), typeof(Translation));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_LocalToWorldFromEntity.Update(ref state);
        m_TransformFromEntity.Update(ref state);
      
        var fetcherTargets = m_FetcherTargetQuery.ToEntityArray(Allocator.Temp);
        var fetcherTargetTranslations = m_FetcherTargetQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        foreach (var fetcher in SystemAPI.Query<FetcherAspect>())
        {
            FindClosestBucket(fetcher, fetcherTargets, fetcherTargetTranslations);
            MoveTowardsTarget(fetcher, ref state);
        }
    }

    private void FindClosestBucket(FetcherAspect fetcher, NativeArray<Entity> fetcherTargets, NativeArray<Translation> fetcherTargetTranslations)
    {
        if (fetcher.TargetBucket == Entity.Null)
        {
            if(!m_FetcherTargetQuery.IsEmpty)
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
                fetcher.TargetBucket = fetcherTargets[closestIdx];
            }
        }
    }

    private void MoveTowardsTarget(FetcherAspect fetcher, ref SystemState state)
    {
        var targetPos = m_LocalToWorldFromEntity[fetcher.TargetBucket].Position * state.Time.DeltaTime;
        m_TransformFromEntity[fetcher.Self].TranslateWorld(targetPos);
    }
}
