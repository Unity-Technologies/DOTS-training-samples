using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system will take care of Mixing up all active steering on ants and proceed to update their rotation / speed
 */
public partial class SteeringSystem : SystemBase
{
    private float m_AntMaxSpeed = 0.2f;
    private float m_AntMaxTurn = math.PI / 2.0f;
    private float m_AntAcceleration = 0.07f;
    
    private float m_PheromoneStrength = 1.0f;
    private float m_ContainmentStrength = 1.0f;
    private float m_GeneralDirectionStrength = 1.0f;
    private float m_ProximityStrength = 1.0f;
    private float m_AvoidanceStrength = 1.0f;

    protected override void OnStartRunning()
    {
    }

    protected override void OnUpdate()
    { 
        float pheromoneStrength = m_PheromoneStrength;
        float containmentStrength = m_ContainmentStrength;
        float generalDirectionStrength = m_GeneralDirectionStrength;
        float proximityStrength = m_ProximityStrength;
        float avoidanceStrength = m_AvoidanceStrength;
        float antMaxSpeed = m_AntMaxSpeed;
        float antMaxTurn = m_AntMaxTurn;
        float antAcceleration = m_AntAcceleration;
        
        Entities
            .ForEach((Entity entity, ref Velocity velocity,
                in Translation translation,
                in PheromoneSteering pheromoneSteering,
                in MapContainmentSteering containmentSteering,
                in GeneralDirection generalDirection,
                in ProximitySteering proximitySteering,
                in ObstacleAvoidanceSteering avoidanceSteering) =>
            {
                float2 finalSteering = pheromoneSteering.Value * pheromoneStrength
                                       + containmentSteering.Value * containmentStrength
                                       + generalDirection.Value * generalDirectionStrength
                                       + proximitySteering.Value * proximityStrength
                                       + avoidanceSteering.Value * avoidanceStrength;
                    
                finalSteering = math.normalize(finalSteering);
                float alignment = (math.dot(finalSteering, velocity.Direction) + 1.0f) / 2.0f;
                float targetSpeed = alignment * antMaxSpeed;
                float accelerationThisFrame = antAcceleration * Time.DeltaTime;
                float acceleration =  math.clamp(targetSpeed - velocity.Speed, -accelerationThisFrame, accelerationThisFrame);
                float newSpeed = targetSpeed + acceleration;

                float targetFacingAngle = math.atan2(finalSteering.y, finalSteering.x);
                float currentFacingAngle = math.atan2(velocity.Direction.y, velocity.Direction.x);

                float turnRateThisFrame = antMaxTurn / Time.DeltaTime;
                float turn = math.clamp(targetFacingAngle - currentFacingAngle, -turnRateThisFrame, turnRateThisFrame);
                float newFacingAngle = currentFacingAngle + turn;
                
                float newDirectionX = math.cos(newFacingAngle) * newSpeed;
                float newDirectionY = math.sin(newFacingAngle) * newSpeed;

                velocity.Direction = new float2(newDirectionX, newDirectionY);
                velocity.Speed = newSpeed;
                
            }).WithoutBurst().Run();
    }
}