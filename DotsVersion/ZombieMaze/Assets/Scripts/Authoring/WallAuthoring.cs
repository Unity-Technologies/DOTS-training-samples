using Unity.Entities;

public class WallAuthoring : UnityEngine.MonoBehaviour
{
}

class WallAuthoringBaker : Baker<WallAuthoring>
{
    public override void Bake(WallAuthoring authoring)
    {
        AddComponent<Wall>();
    }
}