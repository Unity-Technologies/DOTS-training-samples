using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    [NonSerialized]
    public NativeArray<ConstrainedPoint> points;

    // Tornado
    [Range(0f,1f)]
    public float tornadoForce;
    public float tornadoMaxForceDist;
    public float tornadoHeight;
    public float tornadoUpForce;
    public float tornadoInwardForce;

    [NonSerialized]
    public float tornadoX;
    [NonSerialized]
    public float tornadoZ;

    [Range(0f,1f)]
    public float damping;
    [Range(0f,1f)]
    public float friction;

    float tornadoFader = 0;
    
    public void Update()
    {
        tornadoFader = Mathf.Clamp01(tornadoFader + Time.deltaTime / 10f);
        tornadoX = Mathf.Cos(Time.time/6f) * 30f;
        tornadoZ = Mathf.Sin(Time.time/6f * 1.618f) * 30f;

        if (points.Length == 0)
            return;

        // Initialize the job data
        var job = new TornadoJob()
        {
            //time = Time.time,
            tornadoForce = tornadoForce,
            tornadoMaxForceDist = tornadoMaxForceDist,
            tornadoHeight = tornadoHeight,
            tornadoUpForce = tornadoUpForce,
            tornadoInwardForce = tornadoInwardForce,

            tornadoX = tornadoX,
            tornadoZ = tornadoZ,

            tornadoFader = tornadoFader,
            invDamping = 1f - damping,
            friction = friction,
            points = points
        };

        JobHandle jobHandle = job.Schedule(points.Length, 64);
        jobHandle.Complete();
    }
}
