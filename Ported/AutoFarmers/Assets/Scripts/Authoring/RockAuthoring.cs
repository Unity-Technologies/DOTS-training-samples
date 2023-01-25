using Unity.Entities;

class RockAuthoring : UnityEngine.MonoBehaviour
{
    public const int MAX_ROCK_HEALTH = 100;
    class RockBaker : Baker<RockAuthoring>
    {
        public override void Bake(RockAuthoring authoring)
        {
            AddComponent(new Rock { RockHealth = MAX_ROCK_HEALTH} );
        }
    }
}

struct Rock : IComponentData
{
    public int RockHealth;
    public const int type = 1;
}