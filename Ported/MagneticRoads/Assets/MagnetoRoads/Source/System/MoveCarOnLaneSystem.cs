using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(TripleIntersectionSystem))]
public class MoveCarOnLaneSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        ComponentDataFromEntity<CarPosition> positionAccessor = GetComponentDataFromEntity<CarPosition>();
        ComponentDataFromEntity<CarSpeed> speedAccessor = GetComponentDataFromEntity<CarSpeed>();
        
        Entities
	        .WithNativeDisableContainerSafetyRestriction(positionAccessor)
	        .WithNativeDisableContainerSafetyRestriction(speedAccessor)
            .ForEach((Entity entity, ref Lane lane, ref DynamicBuffer<CarBufferElement> buffer) =>
            {
                DynamicBuffer<Entity> carEntities = buffer.Reinterpret<Entity>();
                // A lane doesn't actually care about the first car, that's being handled by the Intersection
                if (carEntities.Length <= 1)
                    return;

                var previousCarPosition = positionAccessor[carEntities[0]].Value;
                
                for(int i = 1; i < carEntities.Length; i++)
                {
	                Entity car = carEntities[i];

	                CarPosition carPosition = positionAccessor[car];
	                CarSpeed carSpeed = speedAccessor[car];
	                
	                carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
	                if (carSpeed.NormalizedValue > 1.0f){
		                carSpeed.NormalizedValue = 1.0f;    
	                }

	                float newPosition = carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
	                if(newPosition > lane.Length)
		                newPosition = lane.Length;
	                
	                float approachMaxSpeed = 1f;
	                
	                float maxCarPosition = previousCarPosition - CarSpeed.CAR_SPACING;
	                if (newPosition > maxCarPosition) {
		                // Break for the car
		                newPosition = maxCarPosition;
		                carSpeed.NormalizedValue = 0f;
		                approachMaxSpeed = 0;
	                } else {
		                // Slow down when approaching another car
		                approachMaxSpeed = (maxCarPosition - newPosition)*5f;
	                }

	                if (carSpeed.NormalizedValue > approachMaxSpeed) {
		                carSpeed.NormalizedValue = approachMaxSpeed;
	                }

	                positionAccessor[car] = new CarPosition {Value = newPosition};
	                speedAccessor[car] = carSpeed;

                    previousCarPosition = newPosition;
                }
            }).ScheduleParallel();
    }
}
