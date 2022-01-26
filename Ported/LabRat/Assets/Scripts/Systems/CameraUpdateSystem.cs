using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class CameraUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Config conf = GetSingleton<Config>();

        Camera cam = Camera.main;

        int size = math.max(conf.MapWidth, conf.MapHeight);
        cam.orthographicSize = 0.75f * size;
        float factor = size / 20.0f;
        cam.transform.position = new Vector3(18 * factor, 25 * factor, 18 * factor);
    }
}