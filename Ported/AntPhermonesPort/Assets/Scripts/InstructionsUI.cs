using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class InstructionsUI : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        if (text != null) text = GetComponent<Text>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            text.enabled = !text.enabled;
        }
    }
}
