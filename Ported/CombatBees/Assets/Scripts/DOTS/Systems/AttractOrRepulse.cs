using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class AttractOrRepulse : SystemBase
{
    EntityQuery m_TeamOneQuery;
    EntityQuery m_TeamTwoQuery;

    Random m_Random;

    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_TeamOneQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<Size>()
            }
        });

        m_TeamTwoQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamTwo>(),
                ComponentType.ReadOnly<Size>()
            }
        });

        m_Random = new Random(0x5716318);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // List<Bee> allies = teamsOfBees[bee.team];
        // Bee attractiveFriend = allies[Random.Range(0,allies.Count)];
        // Vector3 delta = attractiveFriend.position - bee.position;
        // float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
        // if (dist > 0f) {
        //     bee.velocity += delta * (teamAttraction * deltaTime / dist);
        // }
        //
        // Bee repellentFriend = allies[Random.Range(0,allies.Count)];
        // delta = attractiveFriend.position - bee.position;
        // dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
        // if (dist > 0f) {
        //     bee.velocity -= delta * (teamRepulsion * deltaTime / dist);
        // }

        var random = m_Random;
        var deltaTime = Time.DeltaTime;
        var teamOneEntities = m_TeamOneQuery.ToEntityArrayAsync(Allocator.TempJob, out var teamOneEntitiesHandle);
        var teamTwoEntities = m_TeamTwoQuery.ToEntityArrayAsync(Allocator.TempJob, out var teamTwoEntitiesHandle);
        var attractionForce = BeeManager.Instance.teamAttraction;
        var repulsionForce = BeeManager.Instance.teamRepulsion;

        Dependency = JobHandle.CombineDependencies(Dependency, teamOneEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, teamTwoEntitiesHandle);

        Dependency = Entities
            .WithNone<DespawnTimer>()
            .WithDeallocateOnJobCompletion(teamOneEntities)
            .ForEach((ref Velocity velocity, in TeamOne team, in Translation translation) =>
            {
                var attractiveFriend = teamOneEntities[random.NextInt(0, teamOneEntities.Length - 1)];
                var friendPos = GetComponent<Translation>(attractiveFriend);
                var delta = friendPos.Value - translation.Value;
                var dist = math.length(delta);
                if (dist > float.Epsilon)
                {
                    velocity.Value += delta * (attractionForce * deltaTime / dist);
                }

                var repulsiveFriend = teamOneEntities[random.NextInt(0, teamOneEntities.Length - 1)];
                friendPos = GetComponent<Translation>(repulsiveFriend);
                delta = friendPos.Value - translation.Value;
                dist = math.length(delta);
                if (dist > float.Epsilon)
                {
                    velocity.Value -= delta * (repulsionForce * deltaTime / dist);
                }
            }).ScheduleParallel(Dependency);

        Dependency = Entities
            .WithNone<DespawnTimer>()
            .WithDeallocateOnJobCompletion(teamTwoEntities)
            .ForEach((ref Velocity velocity, in TeamTwo team, in Translation translation) =>
            {
                var attractiveFriend = teamTwoEntities[random.NextInt(0, teamTwoEntities.Length - 1)];
                var friendPos = GetComponent<Translation>(attractiveFriend);
                var delta = friendPos.Value - translation.Value;
                var dist = math.length(delta);
                if (dist > float.Epsilon)
                {
                    velocity.Value += delta * (attractionForce * deltaTime / dist);
                }

                var repulsiveFriend = teamTwoEntities[random.NextInt(0, teamTwoEntities.Length - 1)];
                friendPos = GetComponent<Translation>(repulsiveFriend);
                delta = friendPos.Value - translation.Value;
                dist = math.length(delta);
                if (dist > float.Epsilon)
                {
                    velocity.Value -= delta * (repulsionForce * deltaTime / dist);
                }
            }).ScheduleParallel(Dependency);

        m_ECBSystem.AddJobHandleForProducer(Dependency);

        m_Random = random;
    }
}
