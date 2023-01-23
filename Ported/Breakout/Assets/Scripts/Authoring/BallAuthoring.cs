using Unity.Entities;
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
			AddComponent(new URPMaterialPropertyBaseColor());
		}
	}

	public struct Ball : IComponentData
	{
		
	}
}