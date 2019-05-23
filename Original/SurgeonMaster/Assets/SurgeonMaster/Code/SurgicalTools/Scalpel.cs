using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalpel : MonoBehaviour {

#region Fields

    /// <summary>
    /// The minimum world space distance between two points before a cut is made.
    /// </summary>
    public const float minimumCutDistance = 0.04f;

    /// <summary>
    /// The skin vertex where the cut will start.
    /// </summary>
    private SkinVertex _cutStart;

    private SkinMesh _skinMesh;
    private Skin _skin;

    /// <summary>
    /// The cut depth.
    /// </summary>
    private float cutDepth = 1f;

#endregion

#region Properties

    /// <summary>
    /// The skin vertex where the cut will start.
    /// </summary>
    private SkinVertex cutStart
    {
        get { return _cutStart; }
        set
        {
            if (_cutStart != null)
            {
                _cutStart.cannotBeRemoved = false;
            }
            _cutStart = value;
            if (_cutStart != null)
            {
                _cutStart.cannotBeRemoved = true;
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
            Debug.LogWarning("Scalpel could not find skin");
        }else{
            _skinMesh = _skin.skinMesh;
            if(_skinMesh == null){
                Debug.LogWarning("Scalpel could not find the skinmesh");
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

		if(Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ) {
            if(cutStart == null){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                cutStart = _skinMesh.GetClosestSkinVertexToLineSgement(ray.origin, ray.GetPoint(10f), _skin.snapDistance);

            }else{
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                SkinVertex cutEnd = _skinMesh.GetClosestSkinVertexToLineSgement(ray.origin, ray.GetPoint(10f), _skin.snapDistance);
                _skinMesh.Cut(cutStart, cutEnd, _skin.snapDistance, cutDepth);
                cutStart = cutEnd;
            }
        }else{
            _cutStart = null;
        }
    }

#endregion
}
