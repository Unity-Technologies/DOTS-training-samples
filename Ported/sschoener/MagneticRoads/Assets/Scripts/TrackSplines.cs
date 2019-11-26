using System;
using System.Collections.Generic;
using UnityEngine;

public static class TrackSplines
{
    public static List<QueueEntry>[][] waitingQueues;
    
    // each spline has four lanes
    public static List<QueueEntry> GetQueue(SplinePosition splinePos)
    {
        int index = (splinePos.Direction + 1) + (splinePos.Side + 1) / 2;
        return waitingQueues[splinePos.Spline][index];
    }
}
