using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(ClothApplyGravitySystem))]
public class ClothApplyConstraintsSystem : SystemBase
{
	protected override void OnUpdate()
	{
		//var entityQuery = GetEntityQuery(typeof(ClothMeshToken), typeof(ClothMesh));
		//var entityQueryIter = entityQuery.GetArchetypeChunkIterator();

		Entities.ForEach((ref ClothMeshToken clothMeshToken, in ClothMesh clothMesh) =>
		{
			var vertexPosition = clothMesh.vertexPosition;
			var vertexInvMass = clothMesh.vertexInvMass;

			clothMeshToken.jobHandle = Entities.WithSharedComponentFilter(clothMesh).ForEach((Entity entity, in ClothEdge edge) =>
			{
				int index0 = edge.IndexPair.x;
				int index1 = edge.IndexPair.y;

				var p0 = vertexPosition[index0];
				var p1 = vertexPosition[index1];
				var w0 = vertexInvMass[index0];
				var w1 = vertexInvMass[index1];

				float3 r = p1 - p0;
				float rd = math.length(r);

				float delta = 1.0f - edge.Length / rd;
				float W_inv = delta / (w0 + w1);

				vertexPosition[index0] += r * (w0 * W_inv);
				vertexPosition[index1] -= r * (w1 * W_inv);
			}
			).Schedule(clothMeshToken.jobHandle);
			//TODO ScheduleParallel
		}
		).WithoutBurst().Run();
	}
}
