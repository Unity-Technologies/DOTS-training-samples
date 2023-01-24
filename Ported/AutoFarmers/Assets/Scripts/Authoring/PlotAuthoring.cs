using Unity.Entities;

class PlotAuthoring : UnityEngine.MonoBehaviour
{
    class PlotBaker : Baker<PlotAuthoring>
    {
        public override void Bake(PlotAuthoring authoring)
        {
            AddComponent<Plot>();
        }
    }
}

struct Plot : IComponentData
{
}