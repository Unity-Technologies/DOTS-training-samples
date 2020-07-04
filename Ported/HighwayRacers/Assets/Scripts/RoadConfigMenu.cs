using System.Collections;
using System.Collections.Generic;
using HighwayRacer;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


public class RoadConfigMenu : MonoBehaviour
{
    [Header("Children")] public Text numCarsText;
    public Slider numCarsSlider;
    public Text highwaySizeText;
    public Slider highwaySizeSlider;

    public static RoadConfigMenu instance { get; private set; }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateButtonPressed()
    {
        var ecbSys = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        var ecb = ecbSys.CreateCommandBuffer();
        var ent = ecb.CreateEntity();
        ecb.AddComponent<RoadInit>(ent);
        ecb.SetComponent(ent,
            new RoadInit()
            {
                Length = highwaySizeSlider.value,
                NumCars = Mathf.RoundToInt(numCarsSlider.value)
            }
        );
    }

    public void UpdateSliderValues()
    {
    }

    // Use this for initialization
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        numCarsSlider.onValueChanged.AddListener(NumCarsSliderValueChanged);
        highwaySizeSlider.onValueChanged.AddListener(HighwaySizeSliderValueChanged);
    }

    void Start()
    {
        // update slider values
        numCarsSlider.minValue = 1;
        numCarsSlider.maxValue = (float) RoadSys.GetMaxCars(RoadSys.roadLength);

        highwaySizeSlider.minValue = Mathf.Ceil(RoadSys.minLength);
        highwaySizeSlider.maxValue = RoadSys.maxLength;

        numCarsSlider.value = RoadSys.numCars;
        highwaySizeSlider.value = RoadSys.roadLength;
    }

    private void NumCarsSliderValueChanged(float value)
    {
        numCarsText.text = "Number of Cars: " + Mathf.RoundToInt(value);
    }

    private void HighwaySizeSliderValueChanged(float value)
    {
        highwaySizeText.text = "Highway Size: " + Mathf.RoundToInt(value);
        numCarsSlider.maxValue = (float) RoadSys.GetMaxCars(value);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}