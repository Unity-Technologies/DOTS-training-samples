using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Mathf = UnityEngine.Mathf;

public partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var random = Random.CreateFromIndex(GlobalSystemVersion);
        float3 fieldSize = new float3(100f, 20f, 30f);
        float flightJitter = 200f;
        float damping = 0.1f;
        float speedStretch = 0.2f;

        Entities
            .WithAll<BeeMovement>()
            .ForEach((Entity entity, ref Translation translation, ref NonUniformScale scale, ref BeeMovement bee) =>
            {
                var velocity = bee.Velocity;
                velocity += random.NextFloat3Direction() * (flightJitter * deltaTime);
                velocity *= 1f - damping;

                var position = translation.Value;
                position += velocity * deltaTime;

                if (Mathf.Abs(position.x) > fieldSize.x * .5f)
                {
                    position.x = (fieldSize.x * .5f) * Mathf.Sign(position.x);
                    velocity.x *= -0.5f;
                    velocity.y *= .8f;
                    velocity.z *= .8f;
                }
                if (Mathf.Abs(position.z) > fieldSize.z * .5f)
                {
                    position.z = (fieldSize.z * .5f) * Mathf.Sign(position.z);
                    velocity.z *= -0.5f;
                    velocity.x *= .8f;
                    velocity.y *= .8f;
                }
                if (Mathf.Abs(position.y) > fieldSize.y * .5f)
                {
                    position.y = fieldSize.y * .5f * Mathf.Sign(position.y);
                    velocity.y *= -0.5f;
                    velocity.z *= .8f;
                    velocity.x *= .8f;
                }
                bee.Velocity = velocity;
                translation.Value = position;

                var size = bee.Size;
                var scl = new float3(size, size, size);
                float stretch = Mathf.Max(1f, math.distance(velocity, float3.zero) * speedStretch);
                scl.z *= stretch;
                scl.x /= (stretch - 1f) / 5f + 1f;
                scl.y /= (stretch - 1f) / 5f + 1f;
                scale.Value = scl;

            }).ScheduleParallel();
    }
}