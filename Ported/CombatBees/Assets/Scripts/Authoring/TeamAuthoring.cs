using Unity.Entities;
using UnityEngine;

class TeamAuthoring : MonoBehaviour
{
    public uint team;
}

class TeamBaker : Baker<TeamAuthoring>
{
    public override void Bake(TeamAuthoring authoring)
    {
        AddSharedComponent(new Team
        {
            number = authoring.team
        });
    }
}