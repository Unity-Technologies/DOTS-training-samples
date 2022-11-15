using UnityEngine;

public class MainCameraTargetGameObject : MonoBehaviour
{
    public static Transform TransformInstance;

    void Awake()
    {
        TransformInstance = transform;
    }
}