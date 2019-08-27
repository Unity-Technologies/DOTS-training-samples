using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Update the game camera
/// </summary>
public class CameraSystem : ComponentSystem
{
    private const float kOverheadFactor = 1.5f;
    private Camera m_Camera = null;

    /// <summary>
    /// Update the game
    /// </summary>
    protected override void OnUpdate()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
            if (m_Camera == null)
                return;

            m_Camera.orthographic = true;
        }

        Entities.ForEach((ref LbBoard board) => 
        {
            var maxSize = Mathf.Max(board.SizeX, board.SizeY);
            var maxCellSize = 1.0f; // Change if the game will have different cell sizes

            m_Camera.orthographicSize = maxSize * maxCellSize * .65f;
            var posXZ = Vector2.Scale(new Vector2(board.SizeX, board.SizeY) * 0.5f, Vector2.one);

            var t = m_Camera.transform;
            t.position = new Vector3(0, maxSize * maxCellSize * kOverheadFactor, 0);
            t.LookAt(new Vector3(posXZ.x, 0f, posXZ.y));
        });
    }
}
