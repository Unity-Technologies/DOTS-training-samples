using UnityEngine;

public class MetroCamera : MonoBehaviour
{
    public static Camera Instance;

    void Awake()
    {
        Instance = GetComponent<Camera>();
    }
}
