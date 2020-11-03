using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CommuterNavPoint : MonoBehaviour
{

    private Transform t;
    
    private void Start()
    {
        t = transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(t.position, Vector3.one * 0.1f);
    }
}
