using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(CarChangeLaneSystem))]
public partial struct CarChangingLaneSystem : ISystem
{
	// We need type handles to access a chunk's
	// component arrays and entity ID array.
	// It's generally good practice to cache queries and type handles
	// rather than re-retrieving them every update.
	private EntityQuery carChangingLangeStateQuery;
	private ComponentTypeHandle<ChangingLaneState> changingLaneStateHandle;
	
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		var builder = new EntityQueryBuilder(Allocator.Temp);
		builder.WithAll<ChangingLaneState>();
		carChangingLangeStateQuery = state.GetEntityQuery(builder);

		changingLaneStateHandle = state.GetComponentTypeHandle<ChangingLaneState>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		// Type handles must be updated before use in each update.
		changingLaneStateHandle.Update(ref state);

        /*
        CarChangingLaneSystem
		Query all cars in 'ChangingLaneState'
		Move cars in direction of lane
		Once desired lane has been reached,
			Remove car from previous lane
            DISABLE 'ChangingLaneState'
			ENABLE 'OvertakeTimerState'
		*/
    }
}
