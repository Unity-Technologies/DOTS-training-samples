using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


[BurstCompile]
partial struct FetcherMovementSystem : ISystem
{  
    private TransformAspect.EntityLookup m_TransformFromEntity;
    
    private EntityQuery m_FetcherPickUpQuery;
    private EntityQuery m_FetcherDropZoneQuery;

    private Random m_Random;
    
    public void OnCreate(ref SystemState state)
    {       
        m_Random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
        m_FetcherPickUpQuery = state.GetEntityQuery(typeof(Bucket), typeof(Translation));
        m_FetcherDropZoneQuery = state.GetEntityQuery(typeof(FetcherDropZone), typeof(Translation));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_TransformFromEntity.Update(ref state);
      
        var fetcherTargets = m_FetcherPickUpQuery.ToEntityArray(Allocator.Temp);
        var fetcherTargetTranslations = m_FetcherPickUpQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        
        var fetcherDropZones = m_FetcherDropZoneQuery.ToEntityArray(Allocator.Temp);
        var fetcherDropZoneTranslations = m_FetcherDropZoneQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        foreach (var fetcher in SystemAPI.Query<FetcherAspect>())
        {
            UpdateCurrentState(fetcher);
            FindClosestPickUp(fetcher, fetcherTargets, fetcherTargetTranslations);
            FindClosestDropZone(fetcher, fetcherDropZones, fetcherDropZoneTranslations);
            MoveTowardsTarget(fetcher, ref state);
        }
    }

    private void UpdateCurrentState(FetcherAspect fetcher)
    {
        switch (fetcher.CurrentState)
        {
            case FetcherState.Idle:
            {
                if (fetcher.TargetPickUp == Entity.Null && fetcher.TargetDropZone == Entity.Null)
                {
                    fetcher.CurrentState = FetcherState.MoveTowardsBucket;
                } 
            }
                break;

            case FetcherState.MoveTowardsBucket:
            {
                // Handled by MoveTowardsTarget
            }
                break;

            case FetcherState.ArriveAtBucket:
            {
                fetcher.CurrentState = FetcherState.MoveTowardsWater;
                //pick up bucket
                
                
            }
                break;

            case FetcherState.MoveTowardsWater:
            {
                // Handled by MoveTowardsTarget
            }
                break;

            case FetcherState.ArriveAtWater:
            {
                fetcher.CurrentState = FetcherState.FillingBucket;
            }
                break;
            
            case FetcherState.FillingBucket:
            {
                fetcher.TargetDropZone = Entity.Null;
                fetcher.TargetPickUp = Entity.Null;
                fetcher.CurrentState = FetcherState.Idle;
            }
                break;
        }
    }
    
    private void FindClosestPickUp(FetcherAspect fetcher, NativeArray<Entity> fetcherTargets, NativeArray<Translation> fetcherTargetTranslations)
    {
        if (fetcher.CurrentState != FetcherState.MoveTowardsBucket)
        {
            return;
        }
        
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
            else
            {
                fetcher.CurrentState = FetcherState.Idle;
            }
        }
    }
    
    // TODO: Merge FindClosestPickUp & FindClosestDropZone
    private void FindClosestDropZone(FetcherAspect fetcher, NativeArray<Entity> fetcherDropZones, NativeArray<Translation> fetcherDropZoneTranslations)
    {
        if (fetcher.CurrentState != FetcherState.MoveTowardsWater)
        {
            return;
        }
        
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
            else
            {
                // TODO: Drop bucket first?
                fetcher.CurrentState = FetcherState.Idle;
            }
        }
    }

    private void MoveTowardsTarget(FetcherAspect fetcher, ref SystemState state)
    {
        if (fetcher.CurrentState != FetcherState.MoveTowardsBucket &&
            fetcher.CurrentState != FetcherState.MoveTowardsWater)
        {
            return;
        }
        
        bool arrivedX = false;
        bool arrivedZ = false;
        
        float movementSpeed = 0;
        float3 targetPos = float3.zero;
        if (fetcher.CurrentState == FetcherState.MoveTowardsBucket)
        {
            targetPos = m_TransformFromEntity[fetcher.TargetPickUp].Position;
            movementSpeed = state.Time.DeltaTime * fetcher.SpeedEmpty;
        }
        else if (fetcher.CurrentState == FetcherState.MoveTowardsWater)
        {
            targetPos = m_TransformFromEntity[fetcher.TargetDropZone].Position;
            movementSpeed = state.Time.DeltaTime * fetcher.SpeedFull;
        }

        float arriveThreshold = 0.5f;
        TransformAspect aspect = m_TransformFromEntity[fetcher.Self];
        aspect.LookAt(targetPos);
        
        // X-Pos
        if (aspect.Position.x < targetPos.x - arriveThreshold)
        {
            aspect.TranslateWorld(movementSpeed *  new float3(1, 0, 0));
        }
        else if (aspect.Position.x > targetPos.x + arriveThreshold)
        {
            aspect.TranslateWorld(-movementSpeed *  new float3(1, 0, 0));
        }
        else
        {
            arrivedX = true;
        }

        // Z-Pos
        if (aspect.Position.z < targetPos.z - arriveThreshold)
        {
            aspect.TranslateWorld(movementSpeed *  new float3(0, 0, 1));
        }
        else if (aspect.Position.z > targetPos.z + arriveThreshold)
        {
            aspect.TranslateWorld(-movementSpeed *  new float3(0, 0, 1));
        }
        else
        {
            arrivedZ = true;
        }

        if (arrivedX && arrivedZ)
        {
            if (fetcher.CurrentState == FetcherState.MoveTowardsBucket)
            {
                fetcher.CurrentState = FetcherState.ArriveAtBucket;
            }
            else
            {
                fetcher.CurrentState = FetcherState.ArriveAtWater;
            }
        }
    }

}
