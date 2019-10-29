using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTest : MonoBehaviour
{
    /// <summary>
    /// Player text
    /// </summary>
    public Text PlayerText;

    /// <summary>
    /// Current selected player
    /// </summary>
    private Players m_Player = Players.Player1;

    /// <summary>
    /// Update the player text
    /// </summary>
    private void Start()
    {
        UpdatePlayerText();
    }

    /// <summary>
    /// Update the text displayed on the UI
    /// </summary>
    private void UpdatePlayerText()
    {
        PlayerText.text = m_Player.ToString();
    }

    /// <summary>
    /// Input handling
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_Player = Players.Player1;
            UpdatePlayerText();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_Player = Players.Player2;
            UpdatePlayerText();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_Player = Players.Player3;
            UpdatePlayerText();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_Player = Players.Player4;
            UpdatePlayerText();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddRATScore();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            AddCATScore();
        }
    }

    /// <summary>
    /// Add a rat score
    /// </summary>
    private void AddRATScore()
    {
        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity();
        entityManager.AddComponentData(entity, new LbRatScore() { Player = (byte)m_Player });
    }

    /// <summary>
    /// Add a rat score
    /// </summary>
    private void AddCATScore()
    {
        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity();
        entityManager.AddComponentData(entity, new LbCatScore() { Player = (byte)m_Player });
    }
}
