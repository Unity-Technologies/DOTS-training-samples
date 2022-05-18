using Unity.Entities;

class FetcherDropZoneAuthoring : UnityEngine.MonoBehaviour
{
}

class FetcherDropZoneBaker : Baker<FetcherDropZoneAuthoring>
{
    public override void Bake(FetcherDropZoneAuthoring authoring)
    {
        AddComponent<FetcherDropZone>();
    }
}