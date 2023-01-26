using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class FlameCellAuthoring : MonoBehaviour
{
    class Baker : Baker<FlameCellAuthoring>
    {
        public override void Bake(FlameCellAuthoring authoring)
        {
            AddComponent<Position>();
            AddComponent<DisplayHeight>();
            AddComponent<OnFireTag>();
            AddComponent<CellInfo>();
            AddComponent<URPMaterialPropertyBaseColor>();
            AddComponent<PostTransformScale>();
        }
    }
}
