using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    void Start()
    {
        // var translation = World.Active.EntityManager.GetComponentData<TornadoPosition>(entity[0]);
    }

    void Update()
    {
        // Crappy solution: copy & paste the same math
        float tornadoX = Mathf.Cos(Time.time / 6f) * 30f;
        float tornadoZ = Mathf.Sin(Time.time / 6f * 1.618f) * 30f;
        
        transform.position = new Vector3(tornadoX, 10f, tornadoZ) - transform.forward * 60f;
    }
}
