using Unity.Entities;
using Unity.Rendering;

class PlotAuthoring : UnityEngine.MonoBehaviour
{
    class PlotBaker : Baker<PlotAuthoring>
    {
        public override void Bake(PlotAuthoring authoring)
        {
            AddComponent(new Plot { HasPlant = false, HasSeed = false });
        }
    }
}

struct Plot : IComponentData
{
    public const int type = 3;
    public bool HasPlant;
    public bool HasSeed;
}