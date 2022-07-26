using Unity.Entities;

class EntityOfInterestAuthoring : UnityEngine.MonoBehaviour
{
}

class EntityOfInterestBaker : Baker<EntityOfInterestAuthoring>
{
    public override void Bake(EntityOfInterestAuthoring authoring)
    {
        AddComponent<EntityOfInterest>();
    }
}
