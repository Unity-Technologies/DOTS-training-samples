using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

// [UpdateInGroup(typeof(SimulationSystemGroup))]
// [UpdateAfter(typeof(LineUpdateSystem))]
public class LineMoveSystem : SystemBase
{
    private EntityQuery m_LineModifyQuery;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_LineModifyQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<LineModify>(),
            }
        });
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    private static float3 GetLinePosition(float t, float3 startPos, float3 endPos)
    {
        // Compute a position along a line defined by a start and end position.
        // Returns positions that are curved from the center by a perpendicular offset.
        // Positive t means offset to one side of the line, and negative means the other.
        float signT = math.sign(t);
        t = math.saturate(math.abs(t));
        float curveOffset = math.sin(t * math.PI);
        float2 direction = new float2(startPos.x, startPos.z) - new float2(endPos.x, endPos.y);
        direction = math.normalize(direction);
        // Switch sides based on the pasitive or negative of t
        float3 perpendicular = signT * new float3(direction.y, 0, -direction.x);

        return math.lerp(startPos, endPos, t) + perpendicular * curveOffset;
    } 
    
    protected override void OnUpdate()
    {
        // Get the lines.
        var lineModifiers =
            m_LineModifyQuery.ToComponentDataArrayAsync<LineModify>(Allocator.TempJob, out var lineModifiersHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, lineModifiersHandle);
        
        var ecb = m_ECBSystem.CreateCommandBuffer();

        Entities
            .WithName("LineAssignBot")
            .WithDisposeOnCompletion(lineModifiers)
            .ForEach((
                Entity botEntity,
                ref BotRootPosition rootPosition,
                in LineId lineId,
                in BotLineLocationId lineLocation) =>
            {
                for (int i = 0; i < lineModifiers.Length; i++)
                {
                    if (lineId.Value == lineModifiers[i].lineId)
                    {
                        float3 linePosition = GetLinePosition(lineLocation.Value,
                            lineModifiers[i].fillTranslation, lineModifiers[i].tossTranslation);
                        rootPosition.Value = linePosition;
                        // Now make the bot walk to the linePosition
                        // TODO: Check that AddComponent will overwrite an existing component.
                        ecb.AddComponent<TargetPosition>(botEntity, new TargetPosition
                        {
                            Value = linePosition
                        });
                    }
                }
            }).Schedule();

        // Destroy all of the LineModifier Entities (because they are processed only once).
        // Note that the entityCommandBuffer dependency is already set up to run after the job.
        
        // DestroyEntity doesn't work, because it destroys them before lineModifiers is set.
        // Even when I set the order of the system: [UpdateAfter(typeof(LineUpdateSystem))].
        // ecb.DestroyEntity(m_LineModifyQuery);
        
        Entities
            .WithName("DeleteLineModify")
            .ForEach((Entity lineModifyEntity, in LineModify lineModify) =>
            {
                ecb.DestroyEntity(lineModifyEntity);
            }).Schedule();

        // Register a dependency for the EntityCommandBufferSystem.
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}