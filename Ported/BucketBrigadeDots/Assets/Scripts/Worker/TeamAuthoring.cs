using Unity.Entities;
using UnityEngine;

public class TeamAuthoring : MonoBehaviour
{
    public GameObject WorkerPrefab;
    public int NumberOfTeams = 2;
    public int WorkersPerTeam = 10;
}

public class TeamBaker : Baker<TeamAuthoring>
{
    public override void Bake(TeamAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new TeamSpawner() {
            WorkerPrefab = GetEntity(authoring.WorkerPrefab, TransformUsageFlags.WorldSpace),
            NumberOfTeams = authoring.NumberOfTeams,
            WorkersPerTeam = authoring.WorkersPerTeam
        });
    }
}
