using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using TMPro;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public  struct TrackSettings
{
    public  float TrackSize;
    public  float CornerRadius;
    public  float DriverRatio;
    public  int CarCount;
    public int Iteration;
}


public class UIValues : MonoBehaviour
{
    
    public static UIValues Singleton;
    
    public Slider TrackSizeSlider;
    public Slider CornerRadiusSlider;
    public Slider CarCountSlider;
    public Slider DriverTypeSlider;

    protected int Iteration;
    
    public void SetModified()
    {
        Iteration++;
        Debug.Log(UIValues.Singleton);
    }


    public TrackSettings CurrentTrackSettings()
    {
        return  new TrackSettings
        {
            TrackSize = TrackSizeSlider.value,
            CornerRadius = CornerRadiusSlider.value,
            CarCount = Mathf.RoundToInt(CarCountSlider.value),
            DriverRatio = DriverTypeSlider.value,
            Iteration = Iteration
        };
    }

    public static int GetModified()
    {
        return UIValues.Singleton.Iteration;
    }

    public static TrackSettings GetTrackSettings()
    {
        return UIValues.Singleton.CurrentTrackSettings();
    }
    


    // Start is called before the first frame update
    void Start()
    {
        if (Singleton == null)
        {        
            Singleton = this;
        }
        else
        {
            throw new Exception("There should only be one UIValues object in a single scene");
        }
    }

}
