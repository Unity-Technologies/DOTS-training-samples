using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public int carriageCount, platformIndex;
    public MetroLine parentMetroLine;
    public List<Walkway> walkways;
    public Walkway walkway_FRONT_CROSS, walkway_BACK_CROSS;
    public BezierPoint point_platform_START, point_platform_END;
    public Platform oppositePlatform;
    public Platform nextPlatform;
    public List<Platform> adjacentPlatforms;
    public Queue<Commuter>[] platformQueues;
    public CommuterNavPoint[] queuePoints;
    public Train currentTrainAtPlatform;

    public int temporary_routeDistance = 0;
    public Platform temporary_accessedViaPlatform;
    public CommuterState temporary_connectionType;

    public void SetupPlatform(MetroLine _parentMetroLine, BezierPoint _start, BezierPoint _end)
    {
        parentMetroLine = _parentMetroLine;
        point_platform_START = _start;
        point_platform_END = _end;
        carriageCount = parentMetroLine.carriagesPerTrain;
        adjacentPlatforms = new List<Platform>();
        SetColour();
        
        // setup queue lists and spacing
        platformQueues = new Queue<Commuter>[carriageCount];
        for (int i = 0; i < carriageCount; i++)
        {
            platformQueues[i] = new Queue<Commuter>();
        }
        Setup_Walkways();
    }

    public void Setup_Walkways()
    {
        foreach (Walkway _WALKWAY in GetComponentsInChildren<Walkway>())
        {
            _WALKWAY.connects_FROM = this;
        }
    }
    
    public void PairWithOppositePlatform(Platform _oppositePlatform)
    {
        oppositePlatform = _oppositePlatform;
        walkway_FRONT_CROSS.connects_TO = oppositePlatform;
        walkway_BACK_CROSS.connects_TO = oppositePlatform;
    }

    public void Add_AdjacentPlatform(Platform _platform)
    {
        AddAdjacentIfNotPresent(_platform);
        AddAdjacentIfNotPresent(_platform.oppositePlatform);
        foreach (Platform _ADJ in _platform.adjacentPlatforms)
        {
            AddAdjacentIfNotPresent(_ADJ);
            _ADJ.AddAdjacentIfNotPresent(this);
            _ADJ.AddAdjacentIfNotPresent(this.oppositePlatform);
        }
        foreach (Platform _ADJ in _platform.oppositePlatform.adjacentPlatforms)
        {
            AddAdjacentIfNotPresent(_ADJ);
            _ADJ.AddAdjacentIfNotPresent(this);
            _ADJ.AddAdjacentIfNotPresent(this.oppositePlatform);
        }
    }

    public void AddAdjacentIfNotPresent(Platform _platform)
    {
        if (!adjacentPlatforms.Contains(_platform) && _platform != this)
        {
            adjacentPlatforms.Add(_platform);
        }
    }

    public void SetColour()
    {
        Color _LINE_COLOUR = parentMetroLine.lineColour;
        Colour.RecolourChildren(walkway_FRONT_CROSS.transform, _LINE_COLOUR);
        Colour.RecolourChildren(walkway_BACK_CROSS.transform, _LINE_COLOUR);
    }

    public int Get_ShortestQueue()
    {
        int shortest = 0;
        // if all are the same length, go for the second
        int queueLength = 999;
        for (int i = 0; i < carriageCount; i++)
        {
            if (platformQueues[i].Count < queueLength)
            {
                queueLength = platformQueues[i].Count;
                shortest = i;
            }
        }
        
        if (queueLength == 0)
        {
            // all are zero - do something random maybe?
            shortest =  Mathf.FloorToInt(Random.Range(0, carriageCount));
        }

        return shortest;
    }

    public string GetFullName()
    {
        return parentMetroLine.lineName + "_" + platformIndex;
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.black;
        Handles.Label(transform.position, ""+GetFullName());
    }
}
