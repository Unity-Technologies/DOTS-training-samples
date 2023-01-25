using Unity.Entities;
using Unity.Rendering;

class PlotAuthoring : UnityEngine.MonoBehaviour
{
    class PlotBaker : Baker<PlotAuthoring>
    {
        public override void Bake(PlotAuthoring authoring)
        {
            AddComponent(new Plot { TillStatus = 0, HasPlant = false, HasSeed = false });
        }
    }
}

struct Plot : IComponentData
{
    public int TillStatus;
    public bool HasPlant;
    public bool HasSeed;
    public Entity Plant;
}