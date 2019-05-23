using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

    public Text meshResolutionValue;
    public Slider meshResolutionSlider;

    private Skin _skin;

	// Use this for initialization
	void Start () {
        _skin = FindObjectOfType<Skin>();
	}
	
	// Update is called once per frame
	void Update () {
		meshResolutionValue.text = "" + _skin.meshResolution;
	}

    public void UpdateMeshResolution(){
		_skin.meshResolution = (int)meshResolutionSlider.value;
    }

    public void ResetSkin(){
        _skin.Reset();
    }
}
