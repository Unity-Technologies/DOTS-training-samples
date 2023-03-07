using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(CarMoveSystem))]
public partial struct CarChangeLaneSystem : ISystem
{
	// We need type handles to access a chunk's
	// component arrays and entity ID array.
	// It's generally good practice to cache queries and type handles
	// rather than re-retrieving them every update.
	private EntityQuery carChangeLangeStateQuery;
	private ComponentTypeHandle<ChangeLaneState> changeLaneStateHandle;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		var builder = new EntityQueryBuilder(Allocator.Temp);
		builder.WithAll<ChangeLaneState>();
		carChangeLangeStateQuery = state.GetEntityQuery(builder);

		changeLaneStateHandle = state.GetComponentTypeHandle<ChangeLaneState>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state) { }

	
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		// Type handles must be updated before use in each update.
		changeLaneStateHandle.Update(ref state);

		/*
			CarChangeLaneSystem
			Query all cars wanting to change lanes (ChangeLaneState).
			If possible to change lanes,
			    DISABLE 'ChangeLaneState'
				ENABLE 'ChangingLaneState'
				create a "ghost" car in target lane
		*/		
    }
}
