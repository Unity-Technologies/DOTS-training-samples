using UnityEngine;
using UnityEngine.UI;

namespace HighwayRacers
{
    public class HighwayOptions : MonoBehaviour {

        [Header("Children")]
        public Text numCarsText;
        public Slider numCarsSlider;
        public Text highwaySizeText;
        public Slider highwaySizeSlider;

        public static HighwayOptions instance { get; private set; }

        public void Show() {
            gameObject.SetActive (true);
        }
        public void Hide() {
            gameObject.SetActive(false);
        }

        public void UpdateButtonPressed()
        {
            Highway.instance.CreateHighway(highwaySizeSlider.value);
            Highway.instance.SetNumCars(Mathf.RoundToInt(numCarsSlider.value));
            numCarsSlider.value = Highway.instance.GetNumCars();
        }

        public void UpdateSliderValues(){
            
            numCarsSlider.minValue = 0;
            numCarsSlider.maxValue = Game.instance.maxNumCars;
            highwaySizeSlider.minValue = Mathf.Ceil(Highway1.MinHighwayLane0Length);
            highwaySizeSlider.maxValue = Game.instance.highwayMaxSize;

            numCarsSlider.value = Highway.instance.GetNumCars();
            highwaySizeSlider.value = Highway.instance.highway1.innerLaneLength;

        }

        // Use this for initialization
        void Awake () {

            if (instance != null) {
                Destroy (gameObject);
                return;
            }
            instance = this;
            
            numCarsSlider.onValueChanged.AddListener(NumCarsSliderValueChanged);
            highwaySizeSlider.onValueChanged.AddListener(HighwaySizeSliderValueChanged);

        }

        void Start() {

            UpdateSliderValues();

        }

        private void NumCarsSliderValueChanged(float value) {
            numCarsText.text = "Number of Cars: " + Mathf.RoundToInt(value);
        }
        private void HighwaySizeSliderValueChanged(float value) {
            highwaySizeText.text = "Highway Size: " + Mathf.RoundToInt(value);
        }
        
        // Update is called once per frame
        void Update () {
            
        }

        void OnDestroy() {

            if (instance == this) {
                instance = null;
            }

        }

    }

}