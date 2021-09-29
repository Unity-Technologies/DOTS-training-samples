using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = this.GetSingleton<GameObjectRefs>().Camera;
        var tornado = GetSingleton<Tornado>();
        camera.transform.position = new Vector3(tornado.tornadoX,10f,tornado.tornadoZ) - camera.transform.forward * 60f;
    }
}
