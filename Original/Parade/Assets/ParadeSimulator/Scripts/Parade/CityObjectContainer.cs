using UnityEngine;

/// <summary>
/// Just an empty singleton that houses all the CityEntity objects so the scene Hierachy remains readable during testing in the editor.
/// </summary>
public class CityObjectContainer : MonoBehaviour {

    private static CityObjectContainer _instance;
    public static CityObjectContainer Instance {
        get { return _instance; }
    }

    void Awake()
    {

        if (_instance != null)
        {
            Debug.Log("CityObjectContainer:: Duplicate instance of CityObjectContainer, deleting second instance.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

}
