using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameObjectCamera : MonoBehaviour
{
    private void OnEnable()
    {
        GameObjectCameraSystem cameraSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameObjectCameraSystem>();
        cameraSystem.GameObjectCameraTransform = this.transform;
    }
}
