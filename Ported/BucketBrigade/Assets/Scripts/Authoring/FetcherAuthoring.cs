using Unity.Entities;

class FetcherAuthoring : UnityEngine.MonoBehaviour
{
}

class FetcherBaker : Baker<FetcherAuthoring>
{
    public override void Bake(FetcherAuthoring authoring)
    {
        AddComponent<Fetcher>();
    }
}