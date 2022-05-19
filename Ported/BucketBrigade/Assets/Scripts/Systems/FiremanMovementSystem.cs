using Unity.Burst;
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

    public void OnCreate(ref SystemState state)
    {
        m_Random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_TransformFromEntity.Update(ref state);

        var config = SystemAPI.GetSingleton<Config>();
        m_ChainLength = config.WorkerEmptyPerTeamCount;

        float movementSpeed = state.Time.DeltaTime * 1.25f;

        int index = 0;
        foreach (var fireman in Query<FiremanAspect>())
        {
            UpdateState(fireman, index);
            
            if (fireman.FiremanState == FiremanState.OnRouteToDestination)
            {
                UpdatePosition(fireman, movementSpeed);
            }

            index++;
        }
    }

    public void UpdateState(FiremanAspect fireman, int index)
    {
        switch (fireman.FiremanState)
        {
            case FiremanState.Awaiting:
                //float3 destination = GetChainPosition(index, fireman.Position, new float3(0.0f, 0.0f, 3.0f));
                var randz = m_Random.NextFloat(100.0f, -100.0f);
                var randx = m_Random.NextFloat(100.0f, -100.0f);
                //replace with finding target fire cell and  target water cell
                float3 randomPosition = new float3(randx, 0.0f, randz);
                float3 destination = GetChainPosition(index, m_TransformFromEntity[fireman.Self].Position, randomPosition);
                
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

        float3 translation = m_TransformFromEntity[fireman.Self].Forward + math.sign(fireman.Destination - currentPosition) * maxDelta;

        m_TransformFromEntity[fireman.Self].LookAt(fireman.Destination);
        m_TransformFromEntity[fireman.Self].TranslateWorld(translation);
        //fireman.Translate(translation);
    }
}
