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

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        m_TransformFromEntity = new TransformAspect.EntityLookup(ref state, false);
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

        int index = 0;
        foreach (var worker in Query<FiremanAspect>())
        {
            float3 currentPosition = m_TransformFromEntity[worker.Self].Position;
            //temp
            worker.Destination = new float3(0.0f, 0.0f, 100.0f);
            // Do stuff; 
            float3 nextPosition = GetChainPosition(index, worker.Position, worker.Destination);

            worker.Position = nextPosition;

            m_TransformFromEntity[worker.Self].TranslateWorld(nextPosition);
            //worker.Update(state.Time.DeltaTime);

            index++;
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
        float distance = math.length(heading);
        float2 direction = heading / distance;
        float2 perpendicular = new float2(direction.y, -direction.x);


        return math.lerp(startPos, endPos, (float)index / (float)m_ChainLength) + (new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
    }
}
