using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
    public class CarriageAuthoring : MonoBehaviour
    {
        public BoxCollider collider;
    }

    public class CarriageBaker : Baker<CarriageAuthoring>
    {
        public override void Bake(CarriageAuthoring authoring)
        {
            AddComponent(new CarriageBounds
            {
                Width = authoring.collider.size.z + 0.5f
            });
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}