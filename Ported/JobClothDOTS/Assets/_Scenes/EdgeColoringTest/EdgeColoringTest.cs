using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EdgeColoringTest : MonoBehaviour
{
    private void Start()
    {
        // var mesh = GetComponent<MeshFilter>().sharedMesh;

        int edgeNum = 18;
        int vetices = 8;
        
        List<int2> edges = new List<int2>();
        edges.Add(new int2(0,1));
        edges.Add(new int2(0,2));
        edges.Add(new int2(0,4));
        edges.Add(new int2(1,2));
        edges.Add(new int2(1,3));
        edges.Add(new int2(1,4));
        edges.Add(new int2(1,5));
        edges.Add(new int2(1,7));
        edges.Add(new int2(2,3));
        edges.Add(new int2(2,4));
        edges.Add(new int2(2,6));
        edges.Add(new int2(2,7));
        edges.Add(new int2(3,7));
        edges.Add(new int2(4,6));
        edges.Add(new int2(4,5));
        edges.Add(new int2(4,7));
        edges.Add(new int2(5,7));
        edges.Add(new int2(6,7));
        
        List<List<int>>gv = new List<List<int>>();
        for (var i = 0; i < vetices; i++) {
            gv.Add(new List<int>());
        }
        
        for (var i = 0; i < edges.Count; i++)
        {
            gv[edges[i][0]].Add(i);
            gv[edges[i][1]].Add(i);
        }

        var edgeColoring = EdgeColoringUtil.MisraGriesEdgeColoring(edges.Count, vetices, edges, gv);

        foreach (var colorId in edgeColoring)
        {
            Debug.Log($"color id = {colorId}");
        }
    }
}
