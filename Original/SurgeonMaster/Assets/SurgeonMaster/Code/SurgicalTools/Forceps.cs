using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forceps : MonoBehaviour {

#region Editor Fields

    public float maxHeight = 0.1f;

	private float maximumPullForce = 2f;

#endregion

#region Fields

    /// <summary>
    /// The skin vertex where the cut will start.
    /// </summary>
    private SkinVertex _dragSkinVertex;

    private SkinMesh _skinMesh;

    private Skin _skin;

    private float _draggingHeight = 0f;
    private float originalVertexHeight = 0f;

#endregion

#region Properties

    /// <summary>
    /// The skin vertex where the cut will start.
    /// </summary>
    private SkinVertex dragSkinVertex
    {
        get { return _dragSkinVertex; }
        set
        {
            if (_dragSkinVertex != null)
            {
                _dragSkinVertex.cannotBeRemoved = false;
            }
            _dragSkinVertex = value;
            if (_dragSkinVertex != null)
            {
                _dragSkinVertex.cannotBeRemoved = true;
            }
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
            Debug.LogWarning("Forceps could not find skin");
        }else{
            _skinMesh = _skin.skinMesh;
            if(_skinMesh == null){
                Debug.LogWarning("Forceps could not find the skinmesh");
            }
        }
    }

#endregion

#region Methods

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
            //we are using the needle
            return;
        }
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            //we are using the scalpel
            return;
        }
        if(Input.GetMouseButton(0)) {
            if(dragSkinVertex == null){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                dragSkinVertex = _skinMesh.GetClosestSkinVertexToLineSgement(ray.origin, ray.GetPoint(10f), _skin.snapDistance);
                if(dragSkinVertex != null){
                    originalVertexHeight = dragSkinVertex.currentPosition.y;
                }
                _draggingHeight = 0f;
            }else{
                if(_draggingHeight < maxHeight){
                    _draggingHeight = Mathf.Min(maxHeight, _draggingHeight + Time.deltaTime * maxHeight); //1 second to get to the max height
                }
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(
                        new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - (originalVertexHeight + _draggingHeight)));

				Vector3 skinVertexForce = dragSkinVertex.resultingForce;
				Vector3 skinVertexForceNormalized = skinVertexForce.normalized;
				Vector3 newForce = newPosition - dragSkinVertex.currentPosition;

				//The size of the projection of the new force on the normalized skin vertex force.
				float newForceProjectedMagnitude = Vector3.Dot(newForce, skinVertexForceNormalized);

				if (newForceProjectedMagnitude < 0.0f)
				{
					float multiplier = Mathf.Clamp01(0.5f - 0.33f * Mathf.Atan(skinVertexForce.magnitude - maximumPullForce));
					newPosition = dragSkinVertex.currentPosition + newForce * multiplier;
				}
                dragSkinVertex.currentPosition = newPosition;
            }
        }else{
            _dragSkinVertex = null;
        }
    }

#endregion
}