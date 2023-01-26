using Unity.Entities;
using Unity.Mathematics;

class PlotAuthoring : UnityEngine.MonoBehaviour
{
    public int2 PlotLocInWorld;
    class PlotBaker : Baker<PlotAuthoring>
    {
        public override void Bake(PlotAuthoring authoring)
        {
            AddComponent(new Plot { 
                HasPlant = false, 
                HasSeed = false,
                PlotLocInWorld = authoring.PlotLocInWorld
            });
        }
    }
}

struct Plot : IComponentData
{
    public const int type = 3;
    public bool HasPlant;
    public bool HasSeed;
    public Entity Plant;
    public int2 PlotLocInWorld;
}