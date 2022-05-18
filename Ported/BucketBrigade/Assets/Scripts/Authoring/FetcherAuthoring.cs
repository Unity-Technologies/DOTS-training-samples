using Unity.Entities;
using Unity.Mathematics;

class FetcherAuthoring : UnityEngine.MonoBehaviour
{
    public FetcherState InitialState;
}

class FetcherBaker : Baker<FetcherAuthoring>
{
    public override void Bake(FetcherAuthoring authoring)
    {
        AddComponent(new Fetcher()
        {
            CurrentState = authoring.InitialState
        });
    }
}