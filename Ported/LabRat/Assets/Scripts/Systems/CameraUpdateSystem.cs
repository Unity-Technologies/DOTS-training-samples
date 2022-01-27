using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class CameraUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var conf = GetSingleton<Config>();
        var goRefs = this.GetSingleton<GameObjectRefs>();
        Camera cam = goRefs.Camera;

        int size = math.max(conf.MapWidth, conf.MapHeight);
        cam.orthographicSize = 0.75f * size;
        float factor = size / 20.0f;
        cam.transform.position = new Vector3(18 * factor, 25 * factor, 18 * factor);

        // Set clip planes to increase shadow quality
        cam.nearClipPlane = size;
        cam.farClipPlane = size * 2;
    }
}