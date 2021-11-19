using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class BeeMoveBehavior : SystemBase
{

    protected override void OnUpdate()
    {
        var globalDataEntity = GetSingletonEntity<GlobalData>();
        var globalData = GetComponent<GlobalData>(globalDataEntity);
        var beeDefinitions = GetBuffer<TeamDefinition>(globalDataEntity);
        var lookupTranslation = GetComponentDataFromEntity<Translation>(true);

        var frameCount = UnityEngine.Time.frameCount +1;

        var elapsedTime = (float)Time.ElapsedTime;
        
        var dt = Time.DeltaTime;
        
        Entities
            .WithNone<Ballistic, Decay, BeeAttackMode>()
            .WithReadOnly(beeDefinitions)
            .WithReadOnly(lookupTranslation)
            .WithNativeDisableContainerSafetyRestriction(lookupTranslation)
            .WithNativeDisableContainerSafetyRestriction(beeDefinitions)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation position, ref Velocity velocity, ref Flutter flutter, in Bee myself, in TeamID team) =>
                {
                    var random = Random.CreateFromIndex((uint)(entityInQueryIndex + frameCount));

                    var teamDef = beeDefinitions[team.Value];
                    var isMoving = math.lengthsq(velocity.Value) > globalData.MinimumSpeed;
                    var desiredVelocity = float3.zero;

                    if (myself.TargetEntity == Entity.Null)
                    {
                        // no target so just wander back and forth
                        if (position.Value.x > globalData.BoundsMax.x * .75f)
                            desiredVelocity = new float3(-teamDef.speed, 0, 0);
                        else if (position.Value.x < globalData.BoundsMin.x * .75f)
                            desiredVelocity = new float3(teamDef.speed, 0, 0);
                        else if (isMoving)
                        {
                            desiredVelocity = math.normalize(velocity.Value) * teamDef.speed;
                        }
                        else
                        {
                            desiredVelocity = new float3(-teamDef.speed, 0, 0);
                        }
                    }
                    else
                    {
                        var targetPos = lookupTranslation[myself.TargetEntity].Value + myself.TargetOffset;
                        var vectorToTarget = targetPos - position.Value;
                        if (math.lengthsq(vectorToTarget) > math.EPSILON)
                            desiredVelocity = math.normalize(vectorToTarget) * teamDef.speed;
                    }
                    
                    desiredVelocity =math.lerp(velocity.Value, desiredVelocity, 0.05f);

                    if (!flutter.initialized)
                    {
                        flutter.initialized = true;
                        
                        flutter.nextValue = new float3(
                            random.NextFloat(-1, 1),
                            random.NextFloat(-1, 1),
                            random.NextFloat(-1, 1)
                        ) * teamDef.flutterMagnitude;

                        flutter.localSpeed = random.NextFloat(2.5f, 4.5f);
                    }
                    
                    var t = flutter.t;

                    t += new float3(dt, dt, dt);
                    if (t.x > teamDef.flutterInterval.x)
                    {
                        t.x -= teamDef.flutterInterval.x;
                        flutter.prevValue.x = flutter.nextValue.x;
                        flutter.nextValue.x = random.NextFloat(-1, 1) * teamDef.flutterMagnitude.x;
                    }
                    if (t.y > teamDef.flutterInterval.y)
                    {
                        t.y -= teamDef.flutterInterval.y;
                        flutter.prevValue.y = flutter.nextValue.y;
                        flutter.nextValue.y = random.NextFloat(-1, 1) * teamDef.flutterMagnitude.y;
                    }
                    if (t.z > teamDef.flutterInterval.z)
                    {
                        t.z -= teamDef.flutterInterval.z;
                        flutter.prevValue.z = flutter.nextValue.z;
                        flutter.nextValue.z = random.NextFloat(-1, 1) * teamDef.flutterMagnitude.z;
                    }

                    flutter.t = t;

                    t /= teamDef.flutterInterval;
                        
                    var flutterVel = math.lerp(flutter.prevValue, flutter.nextValue, t);

                    float flutterT = math.abs(math.sin(elapsedTime * flutter.localSpeed)); 

                    desiredVelocity = math.lerp(desiredVelocity, flutterVel, 0.15f * flutterT);

                    // move away from the edges
                    float3 absPos = (math.abs(position.Value) - globalData.TurnbackZone) / new float3(globalData.TurnbackWidth);

                    if (absPos.x > 0)
                    {
                        if ((position.Value.x * desiredVelocity.x) >= 0)
                        {
                            desiredVelocity = math.lerp(desiredVelocity, desiredVelocity * new float3(-1, 1, 1),
                                math.max(absPos.x, 1));
                        }
                    }
                    
                    if (absPos.y > 0)
                    {
                        if ((position.Value.y * desiredVelocity.y) >= 0)
                        {
                            desiredVelocity = math.lerp(desiredVelocity, desiredVelocity * new float3(1, -1, 1),
                                math.max(absPos.y, 1));
                        }
                    }

                    if (absPos.z > 0)
                    {
                        if ((position.Value.z * desiredVelocity.z) >= 0)
                        {
                            desiredVelocity = math.lerp(desiredVelocity, desiredVelocity * new float3(1, 1, -1),
                                math.max(absPos.z, 1));
                        }
                    }


                    velocity.Value = desiredVelocity;

                    position.Value += velocity.Value * dt;
                }
            ).ScheduleParallel();
    }
}
