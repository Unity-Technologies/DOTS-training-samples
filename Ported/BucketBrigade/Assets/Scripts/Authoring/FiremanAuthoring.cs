using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class FiremanAuthoring : MonoBehaviour
{
    public int Team;
    public float Speed;
    public float SearchRadius;
    public bool HandleEmptyBuckets;
    public FiremanState InitialState;
}

class FiremanBaker : Baker<FiremanAuthoring>
{
    public override void Bake(FiremanAuthoring authoring)
    {
        Fireman fireman = new Fireman();
        fireman.Team = authoring.Team;
        fireman.Speed = authoring.Speed;
        fireman.SearchRadius = authoring.SearchRadius;
        fireman.State = authoring.InitialState;
        AddComponent<Fireman>(fireman);

        if (authoring.HandleEmptyBuckets)
            AddComponent<EmptyBucketPasser>();
        else
            AddComponent<FilledBucketPasser>();
    }
}