using System;
using System.Collections.Generic;
using UnityEngine;

public static class TrackSplines
{
    public static List<QueueEntry>[][] waitingQueues;
    
    // each spline has four lanes
    public static List<QueueEntry> GetQueue(int track, int direction, int side)
    {
        int index = (direction + 1) + (side + 1) / 2;
        return waitingQueues[track][index];
    }
}
