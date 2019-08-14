using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrackSplineData", menuName = "Scriptable Objects/TrackSplineDataObject", order = 1)]
public class TrackSplineDataObject : ScriptableObject
{
    [SerializeField]
    public List<SplineData> trackSplines = new List<SplineData>();
}
