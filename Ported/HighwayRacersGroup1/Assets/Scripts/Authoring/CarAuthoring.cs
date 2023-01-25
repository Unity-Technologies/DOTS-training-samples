using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class CarAuthoring : MonoBehaviour
{
    class CarBaker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent<CarData>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}

// todo: rename to "Car"
struct CarData : IComponentData
{
    public int SegmentID;
    public int Lane;
}
