using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedDataDrawer : MonoBehaviour
{
    public Mesh intersectionPreviewMesh;
    public const float intersectionSize = .5f;
    [SerializeField] 
    private bool drawLines;
    [SerializeField]
    private GeneratedIntersectionDataObject generatedData;
    
    private void OnDrawGizmos()
    {
        if (!drawLines)
            return;
        var splines = generatedData.splines;
        var intersections = generatedData.intersections;
        for (int i = 0; i < splines.Count; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(splines[i].startPoint, splines[i].endPoint);
            Gizmos.DrawSphere(splines[i].startPoint, 0.02f);
            Gizmos.DrawSphere(splines[i].endPoint, 0.02f);
        }
        
        for (int i = 0; i < intersections.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(intersections[i].position, 0.02f);
        }
        
        Gizmos.color = new Color(.2f, .2f, 1f);
        for (int i = 0; i < intersections.Count; i++)
        {
            if (intersections[i].normal != Vector3Int.zero)
            {
                Gizmos.DrawWireMesh(intersectionPreviewMesh, 0, intersections[i].position,
                    Quaternion.LookRotation(intersections[i].normal),
                    new Vector3(intersectionSize, intersectionSize, 0f));
                Gizmos.DrawRay(intersections[i].position, intersections[i].normal);
            }
        }
    }
}
