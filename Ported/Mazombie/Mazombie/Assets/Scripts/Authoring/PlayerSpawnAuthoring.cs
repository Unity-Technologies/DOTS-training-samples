using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerSpawnAuthoring : MonoBehaviour
{
}

public class PlayerSpawnBaker : Baker<PlayerSpawnAuthoring>
{
    public override void Bake(PlayerSpawnAuthoring authoring)
    {
        AddComponent(new PlayerSpawn());
    }
}