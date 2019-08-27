using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Show the score on the screen
/// </summary>
public class PlayerScore : MonoBehaviour
{
    /// <summary>
    /// Player ID for this score
    /// </summary>
    public Players Player;

    /// <summary>
    /// Reference to the text component
    /// </summary>
    private Text m_ScoreText;

    /// <summary>
    /// Initialize the component
    /// </summary>
    private void Start()
    {
        m_ScoreText = GetComponent<Text>();
    }

    /// <summary>
    /// Update the score
    /// </summary>
    public void SetScore(int value)
    {
        m_ScoreText.text = value.ToString();
    }
}
