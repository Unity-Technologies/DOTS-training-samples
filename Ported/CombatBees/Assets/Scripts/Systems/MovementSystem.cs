using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class MovementSystem : SystemBase
{

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var elapsedMiliseconds = (float)(Time.ElapsedTime - math.trunc( Time.ElapsedTime ));
        var b = GetSingleton<BattleField>();
        
        Dependency = Entities.ForEach((ref TargetPosition position, in TargetEntity target) =>
        {
            // TODO : put this here for now. But can messup
            bool messedup = !HasComponent<Translation>( target.Value );
            if( messedup )
                return;
            Translation entityTranslationComponent = GetComponent<Translation>( target.Value );
            position.Value = entityTranslationComponent.Value;
        }).ScheduleParallel( Dependency );
        
        Dependency = Entities.ForEach((ref Velocity velocity, in Translation translation, in TargetPosition target, in Speed speed ) =>
        {
            float3 direction = target.Value - translation.Value;
            
            float distance = math.length( direction );
            float yDistance = math.abs(direction.y);
            float yEffect = 1-(yDistance/distance);
            
            // push up a little, affected by 
            velocity.Value.y += (deltaTime * 9.8f) * elapsedMiliseconds * 2f * yEffect;
            
            // avoid the floor and negate gravity based on Y"ness" of the direction
            float floor = -b.Bounds.y * 0.5f;
            float f = 5-math.clamp(0, 5, translation.Value.y + floor);
            velocity.Value.y += deltaTime * (9.8f + f) * yEffect;
        
            // apply top speed and orient towards direction
            float currentSpeed = math.length( velocity.Value );
            float3 normalizedDirection = math.normalize( direction );
            velocity.Value += (normalizedDirection * speed.Acceleration) * deltaTime;
            float newSpeed = math.length( velocity.Value );
        
            // orient the velocity towards direction more
            float3 normalizedVelocity = math.normalize( velocity.Value );
            normalizedVelocity = math.normalize(math.lerp( normalizedVelocity, normalizedDirection, deltaTime ));
            
            if( newSpeed > speed.TopSpeed )
            {
                // decrease current speed heavy and clamp to high speed
                currentSpeed -= deltaTime * 14;
                newSpeed = math.max( currentSpeed, speed.TopSpeed );
            }
            
            velocity.Value = normalizedVelocity * newSpeed;
            
            // dampen if were heading in the opposite direction
            if( direction.x * velocity.Value.x < 0 )
                velocity.Value.x -= deltaTime * 5;
            if( direction.y * velocity.Value.y < 0 )
                velocity.Value.y -= deltaTime * 5;
            if( direction.z * velocity.Value.z < 0 )
                velocity.Value.z -= deltaTime * 5;
        }).ScheduleParallel( Dependency );

        // Apply movement for objects that do not stretch
        Dependency = Entities.WithNone<Parent>().ForEach((ref Translation translation, ref Velocity velocity) =>
        {
            velocity.Value.y -= deltaTime * 9.8f;
            translation.Value += velocity.Value * deltaTime;
            
            float y = b.Bounds.y / 2;
            float x = b.Bounds.x / 2;
            float z = b.Bounds.z / 2;
            if (translation.Value.y < -y || translation.Value.y > y)
            {
                velocity.Value.x *= 0.8f;
                velocity.Value.y *= -0.5f;
                velocity.Value.z *= 0.8f;
            }
            if (translation.Value.x < -x || translation.Value.x > x)
            {
                velocity.Value.x *= -0.5f;
                velocity.Value.y *= 0.8f;
                velocity.Value.z *= 0.8f;
            }
            if (translation.Value.z < -z || translation.Value.z > z)
            {
                velocity.Value.x *= 0.8f;
                velocity.Value.y *= 0.8f;
                velocity.Value.z *= -0.5f;
            }
            
            translation.Value.y = math.clamp(translation.Value.y, -b.Bounds.y / 2, b.Bounds.y / 2);
            translation.Value.x = math.clamp(translation.Value.x, -b.Bounds.x / 2, b.Bounds.x / 2);
            translation.Value.z = math.clamp(translation.Value.z, -b.Bounds.z / 2, b.Bounds.z / 2);
            
        }).ScheduleParallel( Dependency );
        
        // apply look at direction and scale for Entities with NonUniformScale
        Dependency = Entities.WithNone<Parent>().ForEach((ref Translation translation, ref Velocity velocity, ref NonUniformScale nonUniformScale, ref Rotation rotation) =>
        {
            rotation.Value = quaternion.LookRotationSafe(velocity.Value, new float3(0, 1, 0));
            nonUniformScale.Value.z = math.length(velocity.Value) * 0.1f;
        }).ScheduleParallel( Dependency );

        
    }


}
