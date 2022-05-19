using Unity.Entities;
using Unity.Mathematics;

class FetcherAuthoring : UnityEngine.MonoBehaviour
{
    public FetcherState InitialState;
    public float SpeedEmpty;
    public float SpeedFull;
}

class FetcherBaker : Baker<FetcherAuthoring>
{
    public override void Bake(FetcherAuthoring authoring)
    {
        AddComponent(new Fetcher()
        {
            CurrentState = authoring.InitialState,
            SpeedEmpty = authoring.SpeedEmpty,
            SpeedFull = authoring.SpeedFull
        });
    }
}