using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    public int numberOfCopies = 10;
    // Start is called before the first frame update
    void Start()
    {
        var metro = GameObject.FindObjectOfType<Metro>();
        var markers = GameObject.Find("markers");
        int lineId = markers.transform.childCount;
        int originalLineCount = markers.transform.childCount;
        int nbPerRow = (int)Mathf.Sqrt(numberOfCopies);
        var originalLineColors = metro.LineColours;
        if(numberOfCopies > 0)
            metro.LineColours = new Color[originalLineCount * (numberOfCopies+1)];
        for (int i = 0; i < numberOfCopies; i++)
        {
            for(int j = 0; j < originalLineCount; j++)
            { 
                var line = markers.transform.GetChild(j);
                var newLine = Instantiate(line, markers.transform);
                var railMarkers = newLine.GetComponentsInChildren<RailMarker>();
                newLine.position += 700.0f * (( Vector3.forward * (i/nbPerRow)) + (Vector3.right *  (i%nbPerRow))) ; 
                foreach (var railMarker in railMarkers)
                {
                    railMarker.metroLineID = lineId;
                }

                metro.LineColours[lineId] = originalLineColors[lineId % originalLineCount];
                lineId++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
