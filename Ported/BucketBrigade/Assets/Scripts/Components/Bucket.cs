using Unity.Entities;

public enum BucketInteractions {
    Dropped,
    PickedUp,
    Drop,
    Pour,
    Chaining
}

public struct Bucket : IComponentData {
    public float fillLevel;
    public Fetcher holder;

    public BucketInteractions Interactions;

    // TODO: these are all hacks
    public int NbOfFiremen; // TODO: Need to be tracked on the team level
    public int CurrentFiremanIdx;
    public int AwaiterCount;
}