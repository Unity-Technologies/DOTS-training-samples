using UnityEngine;
using UnityEngine.UI;

public class UpdateTextUI : MonoBehaviour
{
    public Slider slider;
    Text text;
    
    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = "Scale : " + slider.value;
        Debug.Log(text.text);
    }
}
