using Unity.Entities;
using Unity.Rendering;

// Baking system are similar to regular systems, so we need to explicitly declare them so.
[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial class TrainBakingSystem : SystemBase
{
    protected override void OnUpdate()
	{
		// We have only one tank prefab at the moment, but writing code to handle many
		// entities everywhere possible is good practice, hence this ForEach loop.
		Entities
			.WithEntityQueryOptions(EntityQueryOptions.IncludePrefab)
			.WithImmediatePlayback()
			.ForEach((DynamicBuffer<ChildrenWithRenderer> group, EntityCommandBuffer ecb) =>
			{
				// Notice the EntityCommandBuffer parameter and the WithImmediatePlayback.
				// WithImmediatePlayback only works with Run.
				var entities = group.AsNativeArray().Reinterpret<Entity>();

				// Unlike Bakers, baking systems can modify all the entities they want.
				ecb.AddComponent(entities, new URPMaterialPropertyBaseColor());
			}).Run();
	}
}