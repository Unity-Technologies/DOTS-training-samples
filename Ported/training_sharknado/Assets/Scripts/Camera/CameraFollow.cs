using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    void Update()
    {
        Vector3 tornadoCenter = World.Active.GetOrCreateSystem<ParticleSys>().tornadoCenter;
        transform.position = new Vector3(tornadoCenter.x, 10f, tornadoCenter.z) - transform.forward * 60f;
    }
}
