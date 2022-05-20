using Unity.Entities;

public enum BucketInteractions {
    Dropped,
    PickedUp,
    Drop,
    Pour
}

public struct Bucket : IComponentData {
    public float fillLevel;
    public Fetcher holder;

    public BucketInteractions Interactions;
}