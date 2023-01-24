using Unity.Entities;

class SiloAuthoring : UnityEngine.MonoBehaviour
{
    class RecepticalBaker : Baker<SiloAuthoring>
    {
        public override void Bake(SiloAuthoring authoring)
        {
            AddComponent<Silo>();
        }
    }
}

struct Silo : IComponentData
{
}