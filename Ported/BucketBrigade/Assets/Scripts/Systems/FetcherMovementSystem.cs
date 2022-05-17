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

    private Random m_Random;
    
    public void OnCreate(ref SystemState state)
    {       
        m_Random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        m_LocalToWorldFromEntity = state.GetComponentDataFromEntity<LocalToWorld>(true);
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
        m_FetcherTargetQuery = state.GetEntityQuery(typeof(FetcherTarget));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_LocalToWorldFromEntity.Update(ref state);
        m_TransformFromEntity.Update(ref state);
    
        foreach (var fetcher in SystemAPI.Query<FetcherAspect>())
        {
            if (fetcher.TargetBucket == Entity.Null)
            {
                if(!m_FetcherTargetQuery.IsEmpty)
                {
                    var fetcherTargets = m_FetcherTargetQuery.ToEntityArray(Allocator.Temp);

                    for (int idx = 0; idx < fetcherTargets.Length; idx++)
                    {
                        // TODO: Find closest....
                    }
                    
                    fetcher.TargetBucket = fetcherTargets[m_Random.NextInt(fetcherTargets.Length)];
                }
            }
            
            var targetPos = m_LocalToWorldFromEntity[fetcher.TargetBucket].Position * state.Time.DeltaTime;

            m_TransformFromEntity[fetcher.Self].TranslateWorld(targetPos);
        }
    }
}
