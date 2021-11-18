using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeMoveBehavior : SystemBase
{
    protected override void OnUpdate()
    {
        var globalDataEntity = GetSingletonEntity<GlobalData>();
        var globalData = GetComponent<GlobalData>(globalDataEntity);
        var beeDefinitions = GetBuffer<TeamDefinition>(globalDataEntity);
        var lookupTranslation = GetComponentDataFromEntity<Translation>(true);

        var frameCount = UnityEngine.Time.frameCount +1;
        
        var dt = Time.DeltaTime;
        
        Entities
            .WithNone<Ballistic, Decay, BeeAttackMode>()
            .WithReadOnly(beeDefinitions)
            .WithNativeDisableContainerSafetyRestriction(lookupTranslation)
            .WithNativeDisableContainerSafetyRestriction(beeDefinitions)
            .ForEach((Entity entity, ref Translation position, ref Velocity velocity, in Bee myself, in TeamID team) =>
                {
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
                    
                    // desiredVelocity =math.lerp(velocity.Value, desiredVelocity, 0.05f);
                    //
                    // var axialSpeed = new float3(teamDef.speed * .3f, teamDef.speed * 0.2f, teamDef.speed * 0.3f);
                    // var flutterVelocity = float3.zero;
                    // flutterVelocity.x = math.
                    
                    

                    velocity.Value = math.lerp(velocity.Value, desiredVelocity, 0.05f);

                    position.Value += velocity.Value * dt;
                }
            ).Schedule();
    }
}
