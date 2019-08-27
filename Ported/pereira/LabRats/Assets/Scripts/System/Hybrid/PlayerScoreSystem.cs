using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Update the game camera
/// </summary>
public class PlayerScoreSystem : ComponentSystem
{
    private PlayerScore[] m_Scores = null;

    /// <summary>
    /// Update the game
    /// </summary>
    protected override void OnUpdate()
    {
        if (m_Scores == null)
        {
            var scores = GameObject.FindObjectsOfType<PlayerScore>();
            if (scores.Length == 0)
                return;

            m_Scores = new PlayerScore[4];
            foreach (var score in scores)
            {
                m_Scores[(int)score.Player] = score;
            }
        }

        Entities.ForEach((ref LbPlayer player, ref LbPlayerScore score) =>
        {
            var playerScore = m_Scores[player.Value];
            playerScore.SetScore(score.Value);
        });
    }
}
