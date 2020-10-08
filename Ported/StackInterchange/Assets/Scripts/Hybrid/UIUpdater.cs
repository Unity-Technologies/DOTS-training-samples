using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    public Transform Cameras;
    public Text CameraText;
    public Text NumText;
    public bool buttonClick;
    public int cameraIdx;

    // Set text value for cars
    public void UpdateSpawnCount(int val){
        NumText.text = "Num cars: " + val.ToString();
    }

    // Update camera view
    public void CameraButton(){
        buttonClick = true;
        cameraIdx = (cameraIdx + 1) % Cameras.childCount;
        // cameraIdx = (cameraIdx + 1) % camerasCount;
        CameraText.text = "Active camera: " + cameraIdx.ToString();
        
        for (int i = 0; i < Cameras.childCount; i++){
            if (i == cameraIdx) Cameras.GetChild(i).gameObject.SetActive(true);
            else Cameras.GetChild(i).gameObject.SetActive(false);
        }
    }
}
