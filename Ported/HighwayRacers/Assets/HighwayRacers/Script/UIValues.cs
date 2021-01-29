using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public struct TrackSettings
{
    public float TrackSize;
    public float CornerRadius;
    public float DriverRatio;
    public int CarCount;
    public int Iteration;
}


public class UIValues : MonoBehaviour
{
    protected static UIValues Singleton;

    public Slider TrackSizeSlider;
    public Slider CornerRadiusSlider;
    public Slider CarCountSlider;
    public Text DisplayText;
    
    protected int Iteration;
    protected int CarCount = 40;
    public void SetModified()
    {
        
        float scaled = math.pow(CarCountSlider.value, 9);
        CarCount = Mathf.FloorToInt(math.lerp(40, 600000, scaled));


        Iteration++;
        DisplayText.text = CarCount.ToString();
    }


    public TrackSettings CurrentTrackSettings()
    {
  
        return new TrackSettings
        {
            TrackSize = TrackSizeSlider.value,
            CornerRadius = Mathf.Min(CornerRadiusSlider.value, TrackSizeSlider.value / 4f),
            CarCount = CarCount,
            DriverRatio = 1,  // this would be nice to change interactively, but too late
            Iteration = Iteration
        };
    }

    
    
    public static int GetModified()
    {
        if (UIValues.Singleton == null)
        {
            return -1;
        }

        return UIValues.Singleton.Iteration;
    }

    public static TrackSettings GetTrackSettings()
    {
        if (UIValues.Singleton == null)
        {
            return new TrackSettings
            {
                TrackSize = 1, CornerRadius = 1, CarCount = 1, DriverRatio = 0.5f, Iteration = -1
            };
        }

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