using Unity.Entities;
using Unity.Transforms;

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