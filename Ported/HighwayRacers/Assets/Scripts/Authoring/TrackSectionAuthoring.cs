using Unity.Entities;

class TrackSectionAuthoring : UnityEngine.MonoBehaviour
{

}

class TrackSectionBaker : Baker<TrackSectionAuthoring>
{
    public override void Bake(TrackSectionAuthoring authoring)
    {
        AddComponent<TrackSection>();
    }
}