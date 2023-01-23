using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
	public class CylinderAuthoring : MonoBehaviour
	{
    
	}

	public class CylinderBaker : Baker<CylinderAuthoring>
	{
		public override void Bake(CylinderAuthoring authoring)
		{
			AddComponent(new Cylinder());
			AddComponent(new URPMaterialPropertyBaseColor());
		}
	}

	public struct Cylinder : IComponentData
	{
		
	}
}