using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<Player>();
        var translation = GetComponent<Translation>(player);
        var rotation = GetComponent<Rotation>(player);
        var GojRef = this.GetSingleton<GameObjectRefs>();
        var camera = GojRef.Camera;
        var sensitivity = GojRef.Sensitivity;
        var zoomSensitivity = GojRef.ZoomSensitivity;
        var stiffness = GojRef.Stiffness;
      

        if (Input.GetKey(KeyCode.Mouse1))
        {
            GojRef.ViewAngles.x += Input.GetAxis("Mouse X") * sensitivity / Screen.height;
            GojRef.ViewAngles.y -= Input.GetAxis("Mouse Y") * sensitivity / Screen.height;

            GojRef.ViewAngles.y = Mathf.Clamp(GojRef.ViewAngles.y, -89f, 89f);
        }

        GojRef.ViewDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity * GojRef.ViewDist;
        GojRef.ViewDist = Mathf.Clamp(GojRef.ViewDist, 5f, 80f);

        GojRef.SmoothViewAngles = Vector2.Lerp(GojRef.SmoothViewAngles, GojRef.ViewAngles, stiffness * Time.DeltaTime);
        GojRef.SmoothViewDist = Mathf.Lerp(GojRef.SmoothViewDist, GojRef.ViewDist, stiffness * Time.DeltaTime);

        rotation.Value = Quaternion.Euler(GojRef.SmoothViewAngles.y, GojRef.SmoothViewAngles.x, 0f);
        translation.Value = -camera.transform.forward * GojRef.SmoothViewDist;
        
        
        SetComponent(player, translation);

        // Note the use of the explicit "this" in the following line, the GetSingleton that takes a managed
        // component is an extension method with the same name as a member function.
        

        camera.transform.position = translation.Value;
        camera.transform.rotation = rotation.Value;
    }
}