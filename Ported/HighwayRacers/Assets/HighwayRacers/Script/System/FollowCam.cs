using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Mathematics;
using UnityEngine.UI;
using  System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework.Constraints;
using Unity.Entities.CodeGeneratedJobForEach;
using UnityEditor.Build.Pipeline;
using UnityEngine.Rendering;

// this exists only so that the UI  has a monobehavior to talk to
public class FollowCam : MonoBehaviour
{
    public void SetFPV(bool val)
    {
        Debug.Log(val);
        FollowCamSystem.SetFPV(val);
    }
    
}


public class FollowCamSystem : SystemBase
{
    public static GameObject Camera;
    public EntityQuery eq;

    protected override void OnCreate()
    {
        Camera = new GameObject();
        var c = Camera.AddComponent<Camera>();
        c.fieldOfView = 90;
        Camera.SetActive(false);
        EntityManager.CreateEntityQuery(typeof(CarMovement), typeof(Translation));

    }

    
    protected override void OnUpdate()
    {
        if (Camera == null) return;
        
        Translation tr = new Translation();
        Rotation rt = new Rotation();
        Entities.ForEach((Entity e, in CarMovement m, in Translation t, in Rotation r) =>
        {
            tr = t;
            rt = r;
            return;
        }).Run();

        Camera.transform.position = new Vector3(tr.Value.x, tr.Value.y + 2, tr.Value.z);
        Camera.transform.rotation = rt.Value;
    }

    public static void SetFPV(bool value)
    {
        if (Camera != null)
        {
            Camera.SetActive(value);
        }
        
    }
}
