using Unity.Entities;
using UnityEngine;

class YellowTeamAuthoring : UnityEngine.MonoBehaviour
{
}

class YellowTeamBaker : Baker<YellowTeamAuthoring>
{
    public override void Bake(YellowTeamAuthoring authoring)
    {
        AddComponent(new YellowTeam());
    }
}