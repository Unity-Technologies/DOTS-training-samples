using System;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;

public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = Camera.main;
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        var rotateValue = new Vector3(x, y * -1, 0);
        float defaultFov = 90;
        float zoomMultiplier = 2;
        float angle = Mathf.Abs((defaultFov / zoomMultiplier) - defaultFov);
        
        Entities
            .WithStructuralChanges()
            .ForEach((in InputData inputData) =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    World.GetExistingSystem<FoodSpawner>().Enabled = true;
                }

                if (Input.GetMouseButton(1))
                {
                    camera.transform.Rotate(rotateValue);
                }

                if (Input.GetMouseButtonDown(2))
                {
                    camera.fieldOfView = Mathf.MoveTowards(camera.fieldOfView, defaultFov / 2, angle);
                }

                if (Input.GetKey(inputData.spaceKey))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    World.GetExistingSystem<BeeSpawner>().Enabled = true;
                    World.GetExistingSystem<FoodSpawner>().Enabled = true;
                }
            }).Run();
    }
}