using Unity.Entities;
using Unity.Rendering;

class ColorAuthoring : UnityEngine.MonoBehaviour
{
}

class ColorBaker : Baker<ColorAuthoring>
{
    public override void Bake(ColorAuthoring authoring)
    {
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}

//AddComponent<URPMaterialPropertyBaseColor>();