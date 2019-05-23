using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the Player/Camera down the street through the Parade
/// </summary>
public class CameraMover : MonoBehaviour {

    private static CameraMover _instance;
    public static CameraMover Instance {
        get { return _instance; }
    }

    CityStreamManager myCityStreamManager = null;

    void Awake()
    {

        if (_instance != null)
        {
            Debug.Log("CameraMover:: Duplicate instance of CameraMover, deleting second instance.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    void Start () {

        myCityStreamManager = CityStreamManager.Instance;

        if (!myCityStreamManager)
        {
            Debug.LogError("CameraMover:: Cannot find CityStreamManager in this scene. Demo will not function correctly!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }

    void Update()
    {

        gameObject.transform.Translate(new Vector3(0.0f, 0.0f, myCityStreamManager.CityMovementSpeed) * Time.deltaTime);

    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void OnDrawGizmos()
    {

        Color c = Color.blue;
        c.a = 0.5f;
        Gizmos.color = c;

        Gizmos.DrawSphere(gameObject.transform.position, 0.5f);
        Gizmos.DrawWireSphere(gameObject.transform.position, 0.5f);

    }

#endif

}
