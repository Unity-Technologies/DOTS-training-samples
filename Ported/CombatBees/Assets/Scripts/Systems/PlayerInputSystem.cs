using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((in InputData inputData) =>
        {
            bool isSpaceKeyPressed = Input.GetKey(inputData.spaceKey);
            bool isLeftMousePressed = Input.GetMouseButton(inputData.leftMouse);
            
            if (isSpaceKeyPressed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                World.GetExistingSystem<BeeSpawner>().Enabled = true;
                World.GetExistingSystem<FoodSpawner>().Enabled = true;
            }
            if (isLeftMousePressed)
            {
                World.GetExistingSystem<FoodSpawner>().Enabled = true;
            }
            
        }).Run();
    }
}