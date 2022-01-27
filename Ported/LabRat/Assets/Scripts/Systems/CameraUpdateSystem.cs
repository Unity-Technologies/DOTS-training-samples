using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(MapSpawningSystem))]
public partial class CameraUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
        // this system should only perform work if MapSpawner is present without MapWasSpawned
        RequireForUpdate(
            GetEntityQuery(
                ComponentType.ReadOnly<MapSpawner>(),
                ComponentType.Exclude<MapWasSpawned>()));
        RequireSingletonForUpdate<Config>();
    }

    protected override void OnUpdate()
    {
        Config conf = GetSingleton<Config>();

        Camera cam = this.GetSingleton<GameObjectRefs>().Camera;

        int size = math.max(conf.MapWidth, conf.MapHeight);
        cam.orthographicSize = 0.75f * size;
        float factor = size / 20.0f;
        cam.transform.position = new Vector3(18 * factor, 25 * factor, 18 * factor);

        // Set clip planes to increase shadow quality
        cam.nearClipPlane = size;
        cam.farClipPlane = size * 2;
    }
}