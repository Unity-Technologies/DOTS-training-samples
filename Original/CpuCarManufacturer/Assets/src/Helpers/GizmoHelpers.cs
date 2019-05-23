using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GizmoHelpers : MonoBehaviour {


    public static void DrawRect(Color _colour, Vector3 _pos, float _width, float _height, string _label = "")
    {
        Gizmos.color = _colour;
        GUI.color = _colour;
		
        Vector3 _W = new Vector3(_width,0f,0f);
        Vector3 _H = new Vector3(0f,0f,_height);


        if (_label != "")
        {
            // name
            Handles.Label(_pos - Vector3.one * 0.2f, _label);
        }

        // box
        Gizmos.DrawLine(_pos, _pos + _W);
        Gizmos.DrawLine(_pos, _pos + _H);
        Gizmos.DrawLine(_pos + _W, _pos + _W + _H);
        Gizmos.DrawLine(_pos + _H, _pos + _W + _H);
    }
    
    public static void DrawLine(Color _colour, Vector3 _start, Vector3 _end)
    {
        Gizmos.color = _colour;
        Gizmos.DrawLine(_start, _end);
    }
}
