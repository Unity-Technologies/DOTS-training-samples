using Unity.Entities;

class RecepticalAuthoring : UnityEngine.MonoBehaviour
{
    class RecepticalBaker : Baker<RecepticalAuthoring>
    {
        public override void Bake(RecepticalAuthoring authoring)
        {
            AddComponent<Receptical>();
        }
    }
}

struct Receptical : IComponentData
{
}