using UnityEngine;

/// <summary>
/// Represent a cell in the editor
/// </summary>
public class Cell : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int location;
    [HideInInspector]
    public bool isHole = false;
    [HideInInspector]
    public bool hasSpawner = false;
    [HideInInspector]
    public GameObject homebase = null;
}
