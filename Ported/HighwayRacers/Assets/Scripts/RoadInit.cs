using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadInit : MonoBehaviour
{
    void Start()
    {
        const float straightLength = 12.0f;
        const float curvedLength = 40.0f;   // todo get real measurement
        
        var infos = new RoadSegmentInfo[transform.childCount];
        var prev = transform.GetChild(0);
        infos[0] = new RoadSegmentInfo()
        {
            position = prev.position,
            length = straightLength,
            id = 0,
            curved = false,
        };
        for (int i = 1; i < transform.childCount; i++)
        {
            // move start of child to endpoint of prev (in world coords)
            var child = transform.GetChild(i);
            child.position = prev.Find("EndPoint").position;

            var curved = (i % 2 == 1); 
            infos[i] = new RoadSegmentInfo()
            {
                position = prev.position,
                length = curved ? curvedLength : straightLength,
                id = (short)i,
                curved = curved,
            };
            
            prev = child;
        }
    }
}

public struct RoadSegmentInfo
{
    public Vector3 position;
    public float length;
    public short id;
    public bool curved;
}