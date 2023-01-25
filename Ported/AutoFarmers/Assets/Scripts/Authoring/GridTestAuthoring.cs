using Unity.Entities;
class GridTestAuthoring : UnityEngine.MonoBehaviour
{
    class ConfigBaker : Baker<GridTestAuthoring>
    {
        public override void Bake(GridTestAuthoring authoring)
        {
            AddComponent<GridTest>();
        }
    }
}

struct GridTest : IComponentData
{
    
}