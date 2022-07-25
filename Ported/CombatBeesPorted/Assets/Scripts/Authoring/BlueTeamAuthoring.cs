using Unity.Entities;
using UnityEngine;

class BlueTeamAuthoring : UnityEngine.MonoBehaviour
{
}

class BlueTeamBaker : Baker<BlueTeamAuthoring>
{
    public override void Bake(BlueTeamAuthoring authoring)
    {
        AddComponent(new BlueTeam());
    }
}