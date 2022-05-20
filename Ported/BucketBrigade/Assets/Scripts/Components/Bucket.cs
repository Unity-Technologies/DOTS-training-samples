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

    // TODO: these are all hacks - shoould probably be tracked elsewhere
    public int CurrentTeam; 
    public int NbOfFiremen;
    public int CurrentFiremanIdx;
    public int AwaiterCount;
}