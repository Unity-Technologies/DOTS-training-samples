using UnityEngine;

/// <summary>
/// Base class for all Instantiated objects that are part of the parade. Handles object cleanup based on test parameter for cleanup distance.
/// </summary>
public abstract class CityEntity : MonoBehaviour {

    [Header("Debug")]
    [SerializeField, Tooltip("If true, applicable debug gizmos will be drawn. Has no effect for production builds.")]
    protected bool drawDebugGizmos = false;

    protected bool leftSide = false;
    public bool LeftSide {
        set { leftSide = value; }
    }

    protected virtual void Update ()
    {

        if((CameraMover.Instance.gameObject.transform.position.z - gameObject.transform.position.z) > CityStreamManager.Instance.BufferCleanupDistance)
        {
            handleCleanup();
            Destroy(this.gameObject);
        }
		
	}

    protected abstract void handleCleanup();

}
