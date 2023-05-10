using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDrawer : MonoBehaviour
{
    readonly List<List<Vector3>> ObstacleList = new List<List<Vector3>>();

    public void AddObstacle(List<Vector3> obstacle)
    {
        ObstacleList.Add(obstacle);
    }


    private void OnDrawGizmos()
    {
        foreach (var obstacle in ObstacleList)
        {
            for (int i = 0; i < obstacle.Count - 1; i++)
            {
                Debug.DrawLine(obstacle[i], obstacle[i + 1], Color.gray, 0.05f);
            }
        }
    }
}
