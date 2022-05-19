using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static Unity.Entities.SystemAPI;

[BurstCompile]
partial struct FiremanMovementSystem : ISystem
{
    private TransformAspect.EntityLookup m_TransformFromEntity;
    private int m_ChainLength;
    private Random m_Random;
    TileGrid m_TileGrid;
    private float3 targetPosition;

    public void OnCreate(ref SystemState state)
    {
        m_Random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);

        state.RequireForUpdate<TileGrid>();
        state.RequireForUpdate<TileGridConfig>();
        state.RequireForUpdate<HeatBufferElement>();
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //update the entity lookup
        m_TransformFromEntity.Update(ref state);
        var config = SystemAPI.GetSingleton<Config>();
        //set chain length to worker count
        m_ChainLength = config.WorkerEmptyPerTeamCount;

        //calculate movement speed
        float movementSpeed = state.Time.DeltaTime * 1.25f;

        int index = 0;
        foreach (var fireman in Query<FiremanAspect>())
        {
            UpdateState(fireman, index, ref state);
            
            if (fireman.FiremanState == FiremanState.OnRouteToDestination)
            {
                UpdatePosition(fireman, movementSpeed);
            }

            index++;
        }
    }

    public void UpdateState(FiremanAspect fireman, int index, ref SystemState state)
    {
        switch (fireman.FiremanState)
        {
            case FiremanState.Awaiting:
                
                //bad hack, do something better later, for now assume the first fireman is the leader by which all other firemen follow in the chain
                if (index == 0)
                {
                    //get singletons
                    var tileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
                    m_TileGrid = SystemAPI.GetSingleton<TileGrid>();
                    //get the heat buffer
                    var heatBuffer = state.EntityManager.GetBuffer<HeatBufferElement>(m_TileGrid.entity);
                    //create an allocator and native array to store on fire tiles to find the closest index later
                    var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
                    NativeArray<float3> fireCells = CollectionHelper.CreateNativeArray<float3>(tileGridConfig.Size * tileGridConfig.Size, allocator);

                    int count = 0;
                    foreach (var tile in SystemAPI.Query<TileAspect>().WithAll<Combustable>())
                    {
                        float heat = heatBuffer[count].Heat;
                        //TODO: figure out closet tile with the highest heat value, for now just grab closest on fire tile, stash the heat value into the z of the vector for convenience
                        fireCells[count] = new float3(tile.Position.x, tile.Position.y, heat);
                    }

                    float closestDistance = float.MaxValue;
                    float2 closestCell = new float2();
                    foreach (var cell in fireCells)
                    {
                        //z is heat, ignore low heat tiles
                        if (cell.z < .3f)
                            continue;

                        //compare the 2d vector values, height doesn't factor
                        float distance = math.distance(fireman.Position.xz, cell.xz);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestCell = cell.xy;
                        }
                    }
                    targetPosition.x = closestCell.x;
                    targetPosition.y = closestCell.y;
                    targetPosition.z = 0.0f;
                }

                float3 destination = GetChainPosition(index, m_TransformFromEntity[fireman.Self].Position, targetPosition);
                
                fireman.Destination = destination;
                fireman.FiremanState = FiremanState.OnRouteToDestination;
                break;
            case FiremanState.OnRouteToDestination:
                break;
        }
    }

    //converted chain lerp from original project
    private float3 GetChainPosition(int index, float3 startPos, float3 endPos)
    {
        // adds two to pad between the SCOOPER AND THROWER
        float progress = (float)index / m_ChainLength;
        float curveOffset = math.sin(progress * math.PI) * 1f;

        // get float2 data
        float2 heading = new float2(startPos.x, startPos.z) - new float2(endPos.x, endPos.y);

        float distance = math.distance(startPos,endPos);

        float2 direction = heading / distance;
        float2 perpendicular = new float2(direction.y, -direction.x);

        return math.lerp(startPos, endPos, (float)index / (float)m_ChainLength) + (new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
    }

    public void UpdatePosition(FiremanAspect fireman, float maxDelta)
    {
        float3 currentPosition = m_TransformFromEntity[fireman.Self].Position;
        //float3 delta = math.abs(fireman.Destination - fireman.Position);
        float3 delta = fireman.Destination - currentPosition;
        //float3 translation = currentPosition + math.sign(fireman.Destination - currentPosition) * maxDelta;

        float distance = math.abs(math.distance(currentPosition, fireman.Destination));
        if (distance <= maxDelta)
        {
            //UnityEngine.Debug.Log($"fireman stopping at distance {distance} delta {delta} fireman.Destination {fireman.Destination} currentPosition {currentPosition}");
            fireman.FiremanState = FiremanState.Awaiting;
            return;
        }

        float3 translation = m_TransformFromEntity[fireman.Self].Forward * fireman.Speed;//+ math.sign(fireman.Destination - currentPosition) * (maxDelta / 5);

        m_TransformFromEntity[fireman.Self].LookAt(fireman.Destination);
        m_TransformFromEntity[fireman.Self].TranslateWorld(translation);
        //fireman.Translate(translation);
    }
}
