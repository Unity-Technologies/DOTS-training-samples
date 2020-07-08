using Unity.Entities;
using Unity.Rendering;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    [MaterialProperty("_Tilled", MaterialPropertyFormat.Float)]
    struct TilledMaterialProperty : IComponentData
    {
        public float Value;
    }
}