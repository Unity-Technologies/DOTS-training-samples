using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
	public class ConfigAuthoring : MonoBehaviour
	{
		public GameObject ObstaclePrefab;

		public GameObject BallPrefab;

		public int NumObstacles;

		public int NumBalls;

		public float2 Boundary;

		public class ConfigBaker : Baker<ConfigAuthoring>
		{
			public override void Bake(ConfigAuthoring authoring)
			{
				AddComponent(new Config()
				{
					ObstaclePrefab = GetEntity(authoring.ObstaclePrefab),
					BallPrefab = GetEntity(authoring.BallPrefab),
					NumObstacles = authoring.NumObstacles,
					NumBalls = authoring.NumBalls,
					Boundary = authoring.Boundary
				});
				
			}
		}
		
	}

	public struct Config : IComponentData
	{
		public Entity ObstaclePrefab;
		public Entity BallPrefab;
		public int NumObstacles;
		public int NumBalls;
		public float2 Boundary;
	}
}