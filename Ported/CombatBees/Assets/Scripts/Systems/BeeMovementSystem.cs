using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Mathf = UnityEngine.Mathf;
using Unity.Collections;

[UpdateAfter(typeof(Systems.TargetSystem))]
public partial class BeeMovementSystem : SystemBase
{
    static readonly float flightJitter = 200f;
    static readonly float damping = 0.1f;
    static readonly float speedStretch = 0.2f;
    static readonly float teamAttraction = 5f;
    static readonly float teamRepulsion = 4f;

    static readonly float chaseForce = 50f;
    static readonly float attackDistance = 4f;
    static readonly float attackForce = 500f;
    static readonly float hitDistance = 0.5f;

    EntityQuery[] teamTargets;

    protected override void OnCreate()
    {
        teamTargets = new EntityQuery[2]
        {
                    GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team>()),
                    GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team>())
        };
        teamTargets[0].SetSharedComponentFilter(new Team { TeamId = 0 });
        teamTargets[1].SetSharedComponentFilter(new Team { TeamId = 1 });
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var random = Random.CreateFromIndex(GlobalSystemVersion);


        var team0 = teamTargets[0].ToComponentDataArray<Translation>(Allocator.TempJob);
        var team1 = teamTargets[1].ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithReadOnly(team0)
            .WithDisposeOnCompletion(team0)
            .WithSharedComponentFilter(new Team { TeamId = 0 })
            .ForEach((ref Translation translation, ref NonUniformScale scale, ref BeeMovement bee, in Target target) =>
            {
                UpdateBee(ref translation, ref scale, ref bee, ref random, in team0, in target, deltaTime);
            }).ScheduleParallel();

        Entities
            .WithReadOnly(team1)
            .WithDisposeOnCompletion(team1)
            .WithSharedComponentFilter(new Team { TeamId = 1 })
            .ForEach((ref Translation translation, ref NonUniformScale scale, ref BeeMovement bee, in Target target) =>
            {
                UpdateBee(ref translation, ref scale, ref bee, ref random, in team1, in target, deltaTime);
            }).ScheduleParallel();

    }

    private static void UpdateBee(ref Translation translation,
        ref NonUniformScale scale,
        ref BeeMovement bee,
        ref Random random,
        in NativeArray<Translation> team,
        in Target target,
        float deltaTime)
    {
        var velocity = bee.Velocity;
        var position = translation.Value;
        UpdateJitterAndTeamVelocity(ref random, ref velocity, in position, in team, deltaTime);

        if (target.TargetEntity != null && target.Type == Target.TargetType.Enemy)
        {
            var enemyPos = target.Position;
            var delta = enemyPos - position;
            float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
            if (sqrDist > attackDistance * attackDistance)
            {
                velocity += delta * (chaseForce * deltaTime / Mathf.Sqrt(sqrDist));
            }
            else
            {
                //bee.isAttacking = true;
                velocity += delta * (attackForce * deltaTime / Mathf.Sqrt(sqrDist));
                if (sqrDist < hitDistance * hitDistance)
                {
                    /*ParticleManager.SpawnParticle(bee.enemyTarget.position, ParticleType.Blood, bee.velocity * .35f, 2f, 6);
                    bee.enemyTarget.dead = true;
                    bee.enemyTarget.velocity *= .5f;
                    bee.enemyTarget = null;*/
                    
                }
            }
        }

        position += velocity * deltaTime;
        UpdateBorders(ref velocity, ref position);
        bee.Velocity = velocity;
        translation.Value = position;
        UpdateScale(ref scale, in bee, in velocity);
    }

    private static void UpdateJitterAndTeamVelocity(ref Random random, ref float3 velocity, in float3 position, in NativeArray<Translation> team, float deltaTime)
    {
        velocity += random.NextFloat3Direction() * (flightJitter * deltaTime);
        velocity *= 1f - damping;

        var attractiveFriend = team[random.NextInt(team.Length)];
        var delta = attractiveFriend.Value - position;
        float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
        if (dist > 0f)
        {
            velocity += delta * (teamAttraction * deltaTime / dist);
        }

        var repellentFriend = team[random.NextInt(team.Length)];
        delta = repellentFriend.Value - position;
        dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
        if (dist > 0f)
        {
            velocity -= delta * (teamRepulsion * deltaTime / dist);
        }
    }

    private static void UpdateBorders(ref float3 velocity, ref float3 position)
    {
        if (Mathf.Abs(position.x) > PlayField.size.x * .5f)
        {
            position.x = PlayField.size.x * .5f * Mathf.Sign(position.x);
            velocity.x *= -0.5f;
            velocity.y *= .8f;
            velocity.z *= .8f;
        }
        if (Mathf.Abs(position.z) > PlayField.size.z * .5f)
        {
            position.z = PlayField.size.z * .5f * Mathf.Sign(position.z);
            velocity.z *= -0.5f;
            velocity.x *= .8f;
            velocity.y *= .8f;
        }
        if (Mathf.Abs(position.y) > PlayField.size.y * .5f)
        {
            position.y = PlayField.size.y * .5f * Mathf.Sign(position.y);
            velocity.y *= -0.5f;
            velocity.z *= .8f;
            velocity.x *= .8f;
        }
    }

    private static void UpdateScale(ref NonUniformScale scale, in BeeMovement bee, in float3 velocity)
    {
        var size = bee.Size;
        var scl = new float3(size, size, size);
        float stretch = Mathf.Max(1f, math.distance(velocity, float3.zero) * speedStretch);
        scl.z *= stretch;
        scl.x /= (stretch - 1f) / 5f + 1f;
        scl.y /= (stretch - 1f) / 5f + 1f;
        scale.Value = scl;
    }
}