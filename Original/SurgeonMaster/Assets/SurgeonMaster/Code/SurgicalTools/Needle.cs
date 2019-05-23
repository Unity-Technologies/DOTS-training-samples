using UnityEngine;

/// <summary>
/// Script that controls the suturing tool with the mouse (for testing purposes in the editor).
/// </summary>
public class Needle : MonoBehaviour
{

	#region Fields

	/// <summary>
	/// The skin vertex where the thread will start.
	/// </summary>
	private SkinVertex _threadStart;

	/// <summary>
	/// The visuals for the thread.
	/// </summary>
	private GameObject _model;

    private SkinMesh _skinMesh;
    private Skin _skin;

	#endregion

	#region Properties

	/// <summary>
	/// The skin vertex where the thread will start.
	/// </summary>
	private SkinVertex threadStart
	{
		get { return _threadStart; }
		set
		{
			if (_threadStart != null)
			{
				_threadStart.cannotBeRemoved = false;
			}
			_threadStart = value;
			if (_threadStart != null)
			{
				_threadStart.cannotBeRemoved = true;
			}
		}
	}

	/// <summary>
	/// The visuals for the thread.
	/// </summary>
	private GameObject model
	{
		get { return _model; }
		set
		{
			if (_model != null)
			{
				Destroy(_model);
			}
			_model = value;
		}
	}

	#endregion

	#region Initialization

	/// <summary>
	/// Start is called just before any of the Update methods is called the first time.
	/// </summary>
	public void Start()
	{
        _skin = FindObjectOfType<Skin>();
        if(_skin == null){
            Debug.LogWarning("Needle could not find skin");
        }else{
            _skinMesh = _skin.skinMesh;
            if(_skinMesh == null){
                Debug.LogWarning("Needle could not find the skinmesh");
            }
        }
	}

	#endregion

	#region Destructor

	/// <summary>
	/// This function is called when the MonoBehaviour will be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		threadStart = null;
		model = null;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	public void Update()
	{
        if(!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)){
            threadStart = null;
        }
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            //we are using the scalpel
            return;
        }
        
        if(Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))){
            if(threadStart == null){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                threadStart = _skinMesh.GetClosestSkinVertexToLineSgement(ray.origin, ray.GetPoint(10f), _skin.snapDistance);
            }else{
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                SkinVertex skinVertex = _skinMesh.GetClosestSkinVertexToLineSgement(ray.origin, ray.GetPoint(10f), _skin.snapDistance);
                if(skinVertex != null){
                    _skinMesh.AddThread(threadStart.vertexIndex,skinVertex.vertexIndex);
                    threadStart = skinVertex;
                }
            }
        }
	}

	#endregion
}