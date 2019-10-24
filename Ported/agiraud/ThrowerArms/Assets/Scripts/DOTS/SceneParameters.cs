using UnityEngine;

public class SceneParameters : MonoBehaviour
{
    private static SceneParameters s_Instance;

    public static SceneParameters Instance
    {
        get
        {
            if (s_Instance == null)
                s_Instance = GameObject.FindObjectOfType<SceneParameters>();

            return s_Instance;
        }
    }

    public Vector3 RockSpawnBoxMin = new Vector3(10, 0f, 0f);
    public Vector3 RockSpawnBoxMax = new Vector3(-100, 0f, 0f);
    public Vector3 RockInitialVelocity = new Vector3(-1f, 0f, 0f);

    public Vector3 TinCanSpawnBoxMin = new Vector3(-10, 4f, 15f);
    public Vector3 TinCanSpawnBoxMax = new Vector3(-100, 16f, 15f);
    public Vector3 TinCanInitialVelocity = new Vector3(1f, 0f, 0f);

    public Vector3 DestroyBoxMin = new Vector3(-100, -5, -100);
    public Vector3 DestroyBoxMax = new Vector3(10, 50, 100);

    void DrawBoxGizmo(Vector3 min, Vector3 max)
    {
        Gizmos.DrawWireCube(min + (max - min) * 0.5f, max - min);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        DrawBoxGizmo(DestroyBoxMin, DestroyBoxMax);

        Gizmos.color = Color.yellow;
        DrawBoxGizmo(TinCanSpawnBoxMin, TinCanSpawnBoxMax);

        Gizmos.color = Color.cyan;
        DrawBoxGizmo(RockSpawnBoxMin, RockSpawnBoxMax);
    }
}
