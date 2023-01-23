using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
	public class BallAuthoring : MonoBehaviour
	{
    
	}

	public class BallBaker : Baker<BallAuthoring>
	{
		public override void Bake(BallAuthoring authoring)
		{
			AddComponent(new Ball());
		}
	}

	public struct Ball : IComponentData
    {
        public float3 Speed;
    }
}