using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PathFTest : MonoBehaviour
{
    static readonly HashSet<PositionHeat> heatSet = new();

    void Start()
    {
        // DFS
        DFSSmell(new int2(0, 0), 5);
    }

    static void DFSSmell(int2 startPosition, int distance)
    {
        float heatStep = 1f / (float)distance;
        
        var startNode = new PositionHeat() { heat = 1, position = startPosition };
        
        HashSet<int2> closedSet = new HashSet<int2>();
        Queue<PositionHeat> openQueue = new Queue<PositionHeat>();

        openQueue.Enqueue(startNode);

        while (openQueue.Count > 0)
        {
            var pH = openQueue.Dequeue();
            
            if(closedSet.Contains(pH.position))
                continue;

            closedSet.Add(pH.position);
            
            if(pH.heat - heatStep <= 0)
                continue;

           // heatSet.Add(pH);
//
           // if (heatSet.Count > 100)
           // {
           //     Debug.LogError($"Count more than 100");
           //     break;
           // }

            for (int i = 0; i < 8; i++)
            {
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x - 1, pH.position.y - 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x - 1, pH.position.y) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x - 1, pH.position.y + 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x, pH.position.y + 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x + 1, pH.position.y + 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x + 1, pH.position.y) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x + 1, pH.position.y - 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x, pH.position.y - 1) } );
            }
        }
    }

    static void AddToQueue(Queue<PositionHeat> openQueue, HashSet<int2> closedSet, PositionHeat ph)
    {
        if(closedSet.Contains(ph.position))
            return;
        
        openQueue.Enqueue(ph);
    }

    class PositionHeat
    {
        public float heat;
        public int2 position;
    }


    private void OnDrawGizmos()
    {
        foreach (var pH in heatSet)
        {
            Vector3 pos = Vector3.zero;
            pos.x = pH.position.x;
            pos.y = pH.position.y;

            Gizmos.color = new Color(pH.heat,0,0,1);
            Gizmos.DrawCube(pos, Vector3.one);
        }
    }
}
