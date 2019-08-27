using System;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    /// <summary>
    /// Reference to the text component
    /// </summary>
    private Text m_Text;

    /// <summary>
    /// Initialize the component
    /// </summary>
    private void Start()
    {
        m_Text = GetComponent<Text>();
    }

    /// <summary>
    /// Update the remaining time
    /// </summary>
    public void SetTime(float value)
    {
        var t = TimeSpan.FromSeconds(value);
        var answer = string.Format("{1:D2}:{2:D2}",
            t.Hours, t.Minutes, t.Seconds, t.Milliseconds);

        m_Text.text = answer.ToString();
        m_Text.color = value < 5f ? Color.red : Color.white;
    }
}
