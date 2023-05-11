using UnityEngine;

namespace Metro
{
// From Tanks Sample
    public class CameraSingleton : MonoBehaviour
    {
        public static Camera Instance;

        void Awake()
        {
            Instance = GetComponent<Camera>();
        }
    }
}
