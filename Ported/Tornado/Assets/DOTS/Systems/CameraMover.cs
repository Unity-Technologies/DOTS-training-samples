using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Dots
{

    [UpdateAfter(typeof(TornadoMover))]
    public partial class CameraMover : SystemBase
    {
        protected override void OnUpdate()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                var tornadoEntity = GetSingletonEntity<TornadoConfig>();
                var tornadoPosition = GetComponent<Translation>(tornadoEntity).Value;
                Transform trans = cam.transform;
                trans.position = new Vector3(tornadoPosition.x, 10f, tornadoPosition.z) - trans.forward * 60f;
            }
        }
    }
}

