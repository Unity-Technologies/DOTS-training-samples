using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

readonly partial struct ConfigAspect : IAspect
{
    private readonly RefRO<Config> Config;

    public Entity WallPrefab
    {
        get => Config.ValueRO.WallPrefab;
    }
}
