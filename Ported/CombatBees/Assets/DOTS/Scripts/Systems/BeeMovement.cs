using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

[UpdateAfter(typeof(BeePerception))]
public class BeeMovement : SystemBase
{
    const float maxRotation =180f;
    
    [ReadOnly]
    private ComponentDataFromEntity<Translation> cdfe;

    protected override void OnUpdate()
    {
        cdfe = GetComponentDataFromEntity<Translation>(true);
        var random = new Random(1234);
        var deltaTime = Time.DeltaTime;
        
        // Update all TargetPositions with current position of Target (deterministic!)
        Entities
            .WithoutBurst()
            .ForEach((ref TargetPosition targetPosition, in Target target) => {
                targetPosition.Value = cdfe[target.Value].Value;
        }).Run();

        // TODO when a bee dies it's target must be removed
        // Move bees that are targetting (a Resource or Base) towards the target's position
        Entities
            .WithAll<IsBee>()
            .ForEach((ref Translation translation, ref Velocity velocity, in TargetPosition targetPosition, in Speed speed) =>
            {
                float3 currentPosition = translation.Value;

                float3 newLookAt = targetPosition.Value - currentPosition;

                var q = UnityEngine.Quaternion.FromToRotation(velocity.Value, newLookAt);
                q.ToAngleAxis(out var angle, out var axis);
                var deltaAngle = maxRotation * deltaTime;
                deltaAngle = angle > deltaAngle ? deltaAngle : angle;
                q = UnityEngine.Quaternion.AngleAxis(deltaAngle, axis);

                velocity.Value = math.rotate(q, velocity.Value);
                //velocity.Value += speed.Value;

                /*float3 axis = math.cross(newLookAt, velocity.Value);
                float sinAngle = math.length(axis);
                float cosAngle = math.dot(newLookAt, velocity.Value);

                float angle = math.atan2(sinAngle, cosAngle);

                float deltaAngle = angle > maxRotation * deltaTime ? maxRotation * deltaTime : angle;

                quaternion rotation = quaternion.AxisAngle(axis, deltaAngle);

                velocity.Value = math.rotate(rotation, velocity.Value);
                velocity.Value += speed.Value;
                */

                //translation.Value += velocity.Value;
                // move toward target Resource
                //var move = math.normalize(targetPosition.Value - translation.Value) * speed.Value * deltaTime;
                translation.Value += velocity.Value * deltaTime;
                translation.Value.y = math.max(translation.Value.y, 0.05f);
            }).Schedule();

        // Move bees that are not targetting a Resource in a random direction bounded by the Arena
        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

        Entities
            .WithAll<IsBee>()
            .WithNone<Target, IsDead, IsAttacking>()
            .ForEach((ref Translation translation, in Speed speed) =>
            {
                var targetPosition = Utils.BoundedRandomPosition(arenaAABB, ref random);

                // move toward target Resource
                var move = math.normalize(targetPosition - translation.Value) * speed.Value * deltaTime;
                translation.Value += move;
            }).Schedule();
    }
}
