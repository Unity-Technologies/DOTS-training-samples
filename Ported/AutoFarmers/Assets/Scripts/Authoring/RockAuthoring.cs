using Unity.Entities;

class RockAuthoring : UnityEngine.MonoBehaviour
{
    class RockBaker : Baker<RockAuthoring>
    {
        public override void Bake(RockAuthoring authoring)
        {
            AddComponent<Rock>();
        }
    }
}

struct Rock : IComponentData
{
}