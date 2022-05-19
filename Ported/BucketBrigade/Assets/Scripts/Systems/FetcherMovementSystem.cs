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
    private EntityQuery m_CombustableTileQuery;

    private Random m_Random;
    
    public void OnCreate(ref SystemState state)
    {
        m_Random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
        m_FetcherPickUpQuery = state.GetEntityQuery(typeof(FetcherTarget), typeof(Translation));
        m_FetcherDropZoneQuery = state.GetEntityQuery(typeof(FetcherDropZone), typeof(Translation));
        m_CombustableTileQuery = state.GetEntityQuery(typeof(Combustable), typeof(Translation));
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

        var combustableTiles = m_CombustableTileQuery.ToEntityArray(Allocator.Temp);
        var combustableTileTranslations = m_CombustableTileQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        foreach (var fetcher in SystemAPI.Query<FetcherAspect>())
        {
            UpdateCurrentState(fetcher);
            FindClosestPickUp(fetcher, fetcherTargets, fetcherTargetTranslations);
            FindClosestDropZone(fetcher, fetcherDropZones, fetcherDropZoneTranslations);
            FindClosestFireTile(ref state, fetcher, combustableTiles, combustableTileTranslations);
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
                //ResetFetcher(fetcher);
                fetcher.CurrentState = FetcherState.MoveTowardsFire;
            }
                break;

            case FetcherState.MoveTowardsFire:
            {
                // Handled by MoveTowardsTarget
            }
                break;

            case FetcherState.ArriveAtFire:
            {
                ResetFetcher(fetcher);
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
                ResetFetcher(fetcher);
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
                ResetFetcher(fetcher);
            }
        }
    }
    
    private void FindClosestFireTile(ref SystemState state, FetcherAspect fetcher, NativeArray<Entity> combustableTiles, NativeArray<Translation> combustableTileTranslations)
    {
        if (fetcher.CurrentState != FetcherState.MoveTowardsFire)
        {
            return;
        }
        
        if (fetcher.TargetFireTile == Entity.Null)
        {
            if(combustableTileTranslations.Length > 0)
            {
                TileGrid tileGrid = SystemAPI.GetSingleton<TileGrid>();
                var heatBuffer = state.EntityManager.GetBuffer<HeatBufferElement>(tileGrid.entity);
                
                int closestIdx = -1;
                float closestDistance = float.MaxValue;
                for (int idx = 0; idx < combustableTiles.Length; idx++)
                { 
                    if (heatBuffer[idx].Heat > 0)
                    {   
                        // Tile on fire
                        Translation currentTranslation = combustableTileTranslations[idx];
                        var distance = math.distance(fetcher.Position, currentTranslation.Value);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestIdx = idx;
                        }
                    }
                }

                if (closestIdx >= 0)
                {
                    fetcher.TargetFireTile = combustableTiles[closestIdx];
                }
            }
            else
            {
                ResetFetcher(fetcher);
            }
        }
    }

    private void ResetFetcher(FetcherAspect fetcher)
    {
        // TODO: Drop bucket first?
        fetcher.TargetDropZone = Entity.Null;
        fetcher.TargetPickUp = Entity.Null;
        fetcher.TargetFireTile = Entity.Null;
        fetcher.CurrentState = FetcherState.Idle;
    }

    private void MoveTowardsTarget(FetcherAspect fetcher, ref SystemState state)
    {
        if (fetcher.CurrentState != FetcherState.MoveTowardsBucket &&
            fetcher.CurrentState != FetcherState.MoveTowardsWater &&
            fetcher.CurrentState != FetcherState.MoveTowardsFire)
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
            movementSpeed = state.Time.DeltaTime * fetcher.SpeedEmpty;
        }
        else if (fetcher.CurrentState == FetcherState.MoveTowardsFire)
        {
            targetPos = m_TransformFromEntity[fetcher.TargetFireTile].Position;
            movementSpeed = state.Time.DeltaTime * fetcher.SpeedFull;
        }

        float arriveThreshold = 0.025f;
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
            else if(fetcher.CurrentState == FetcherState.MoveTowardsWater)
            {
                fetcher.CurrentState = FetcherState.ArriveAtWater;
            }
            else // MoveTowardsFire
            {
                fetcher.CurrentState = FetcherState.ArriveAtFire;
            }
        }
    }
}
