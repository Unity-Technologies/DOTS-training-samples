using Unity.Entities;

class FetcherTargetAuthoring : UnityEngine.MonoBehaviour
{
}

class FetcherTargetBaker : Baker<FetcherTargetAuthoring>
{
    public override void Bake(FetcherTargetAuthoring authoring)
    {
        AddComponent<FetcherTarget>();
    }
}