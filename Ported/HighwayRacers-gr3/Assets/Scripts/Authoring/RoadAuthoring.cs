using Unity.Entities;

class RoadAuthoring : UnityEngine.MonoBehaviour
{
    
}

class RoadBaker : Baker<RoadAuthoring>
{
    public override void Bake(RoadAuthoring authoring)
    {
        AddComponent<Road>();
    }
}