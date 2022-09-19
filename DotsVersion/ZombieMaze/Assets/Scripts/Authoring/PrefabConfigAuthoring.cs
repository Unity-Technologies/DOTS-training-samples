using Unity.Entities;
using UnityEngine;

public class PrefabConfigAuthoring : MonoBehaviour
{
    public GameObject WallPrefab;
    public GameObject PillPrefab;
    public GameObject CharacterPrefab;
    public GameObject TilePrefab;
    public GameObject ZombiePrefab;
    public GameObject MovingWallPrefab;
}

class PrefabConfigBaker : Baker<PrefabConfigAuthoring>
{
    public override void Bake(PrefabConfigAuthoring authoring)
    {
        AddComponent(new PrefabConfig
        {
            WallPrefab = GetEntity(authoring.WallPrefab),
            PillPrefab = GetEntity(authoring.PillPrefab),
            CharacterPrefab = GetEntity(authoring.CharacterPrefab),
            TilePrefab = GetEntity(authoring.TilePrefab),
            ZombiePrefab = GetEntity(authoring.ZombiePrefab),
            MovingWallPrefab = GetEntity(authoring.MovingWallPrefab)
        });
    }
}