using System.Collections;
using System.Collections.Generic;
using HighwayRacer;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;


public class CarProperties : MonoBehaviour
{
    [Header("Children")] public SliderProp defaultSpeedSlider;
    public SliderProp overtakeSpeedSlider;
    public SliderProp headwayBlockingDistanceSlider;

    public Entity selectedCar;

    public static CarProperties instance { get; private set; }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(Entity car)
    {
        gameObject.SetActive(true);
        selectedCar = car;

        // update sliders to display current values for this car
        SetSliderProperties();
    }

    // public Car selectedCar { get; private set; }

    private void SetSliderProperties()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        Assert.IsTrue(em.Exists(selectedCar), "Shouldn't be setting slider property when no car is selected. How'd we get here?");
        var unblockedSpeed = em.GetComponentData<UnblockedSpeed>(selectedCar);
        var overtakeSpeed = em.GetComponentData<OvertakeSpeed>(selectedCar);
        var blockedDist = em.GetComponentData<BlockedDist>(selectedCar);
        
        defaultSpeedSlider.value = unblockedSpeed.Val;
        defaultSpeedSlider.SetText("Default Speed: " + unblockedSpeed.Val.ToString("0.0") + " m/s");
        overtakeSpeedSlider.value = overtakeSpeed.Val;
        overtakeSpeedSlider.SetText("Overtake Speed: " + overtakeSpeed.Val.ToString("0.0") + " m/s");
        headwayBlockingDistanceSlider.value = blockedDist.Val;
        headwayBlockingDistanceSlider.SetText("Look Ahead Distance: " + blockedDist.Val.ToString("0.0"));
    }

    public void SliderUpdated(float value)
    {
        // if (selectedCar == null)
        // 	return;
        // if (preventUpdatingCar)
        // 	return;
        //
        // selectedCar.defaultSpeed = defaultSpeed.value;
        // selectedCar.overtakePercent = overtakePercent.value;
        // selectedCar.leftMergeDistance = leftMergeDistance.value;
        // selectedCar.mergeSpace = mergeSpace.value;
        // selectedCar.overtakeEagerness = overtakeEagerness.value;
        //
        // SetSliderProperties();
    }

    public void BackButtonPressed()
    {
        Hide();
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<CameraControlSys>().ResetCamera();
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        defaultSpeedSlider.slider.onValueChanged.AddListener(SliderUpdated);
        overtakeSpeedSlider.slider.onValueChanged.AddListener(SliderUpdated);
        headwayBlockingDistanceSlider.slider.onValueChanged.AddListener(SliderUpdated);
    }

    // Use this for initialization
    void Start()
    {
        // set slider bounds
        defaultSpeedSlider.SetBounds(CarSpawnSys.minSpeed, CarSpawnSys.maxSpeed);
        overtakeSpeedSlider.SetBounds(CarSpawnSys.minOvertakeModifier * CarSpawnSys.minSpeed, CarSpawnSys.maxOvertakeModifier * CarSpawnSys.maxSpeed);
        headwayBlockingDistanceSlider.SetBounds(CarSpawnSys.minBlockedDist, CarSpawnSys.maxBlockedDist);
        Hide();
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}