using System;
using Unity.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    public int NumQuadsTotal;
    public Transform Cameras;
    public Text NumText;
    public Transform CameraButton;
    public Transform TimeScaleButton;
    public Transform QuadButton;
    
    [HideInInspector]
    public bool QuadButtonPressed;
    [HideInInspector]
    public int ActiveQuad;
    [HideInInspector]
    public float QuadX;
    [HideInInspector]
    public float QuadY;
    [HideInInspector]
    public float QuadZ;

    private int cameraIdx;
    private float[] timeScales = new float[] { 30f, 60f, 180f }; // 30 60 180
    private int timeIdx = 1;

    // Set text value for cars
    public void UpdateSpawnCount(int val){
        NumText.text = "Num cars: " + val.ToString();
    }

    // Update camera view
    public void CameraOnclick(){
        cameraIdx = (cameraIdx + 1) % Cameras.childCount;
        CameraButton.GetComponentInChildren<Text>().text = "Active camera: " + cameraIdx.ToString();
        
        for (int i = 0; i < Cameras.childCount; i++){
            if (i == cameraIdx) Cameras.GetChild(i).gameObject.SetActive(true);
            else Cameras.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void QuadOnclick(){
        ActiveQuad = (ActiveQuad + 1) % NumQuadsTotal;
        QuadButtonPressed = true;
    }

    public void ChangeQuad(){
        QuadButtonPressed = false;
        Cameras.position = new Vector3(QuadX, QuadY, QuadZ);
        QuadButton.GetComponentInChildren<Text>().text = "Current quad: " + ActiveQuad.ToString();
    }

    public void TimeScale(){
        timeIdx = (timeIdx + 1) % timeScales.Length;
        var fixedSimulationGroup = World.DefaultGameObjectInjectionWorld?.GetExistingSystem<FixedStepSimulationSystemGroup>();
        // The group timestep can be set at runtime:
        if (fixedSimulationGroup != null)
        {
            fixedSimulationGroup.Timestep = 1.0f / timeScales[timeIdx];
        }

        TimeScaleButton.GetComponentInChildren<Text>().text = "Time scale: " + (timeScales[timeIdx] / 60f).ToString("#.0");
    }
}
