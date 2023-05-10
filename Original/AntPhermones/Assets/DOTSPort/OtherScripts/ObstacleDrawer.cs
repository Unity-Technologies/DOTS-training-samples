//#define USE_GIZMO
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDrawer : MonoBehaviour
{
    [SerializeField] 
    Material m_WallsMaterial;

#if USE_GIZMO
    readonly List<List<Vector3>> m_ObstacleList = new List<List<Vector3>>();
#endif
    
    public void AddObstacle(List<Vector3> centerLine, float thickness)
    {
        if (centerLine.Count <= 1)
        {
            Debug.LogError($"Obstacle length is too small. PointsCount is: {centerLine.Count}, but should be more than 1");
            return;
        }
        
#if USE_GIZMO
        m_ObstacleList.Add(centerLine);
#endif
        // Create child gameObject
        GameObject go = new GameObject();
        go.transform.SetParent(transform);
        go.AddComponent<MeshRenderer>().material = m_WallsMaterial;
        var meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateMesh(centerLine, thickness);
    }

    Mesh CreateMesh(List<Vector3> centerLine, float wallThickness)
    {
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < centerLine.Count - 1; i++)
        {
            int id = i * 2;
            var rightDirection = RightDirection(centerLine[i], centerLine[i + 1]) * wallThickness;
            vertices.Add(centerLine[i] + rightDirection);
            vertices.Add(centerLine[i] - rightDirection);
        }
        
        var lastRightDirection = RightDirection(centerLine[^2], centerLine[^1]) * wallThickness;
        vertices[^2] = centerLine[^1] + lastRightDirection;
        vertices[^1] = centerLine[^1] - lastRightDirection;
        
        
        List<int> triangles = new List<int>();
        for (int i = 0; i < vertices.Count - 3; i+= 2)
        {
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
            
            triangles.Add(i + 1);
            triangles.Add(i + 3);
            triangles.Add(i + 2);
        }

        int endPoint0 = vertices.Count - 1;
        int endPoint1 = vertices.Count - 2;
        
        // Add start cap
        var startCap = CapPoints(centerLine[0], centerLine[1]);
        vertices.Add(centerLine[0] + startCap.Item1 * wallThickness);
        vertices.Add(centerLine[0] + startCap.Item2 * wallThickness);
        
        triangles.Add(vertices.Count - 1);
        triangles.Add(1);
        triangles.Add(0);
        
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(0);
        
        // Add end cap
        var endCap = CapPoints(centerLine[^1], centerLine[^2]);
        vertices.Add(centerLine[^1] + endCap.Item1 * wallThickness);
        vertices.Add(centerLine[^1] + endCap.Item2 * wallThickness);
        
        triangles.Add(vertices.Count - 1);
        triangles.Add(endPoint1);
        triangles.Add(endPoint0);
        
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(endPoint0);
        

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        return mesh;
    }
    
    static Vector3 RightDirection(Vector3 firstPoint, Vector3 nextPoint)
    {
        var dir = (nextPoint - firstPoint).normalized;
        return Vector3.Cross(dir, Vector3.forward);
    }

    static (Vector3, Vector3) CapPoints(Vector3 innerPoint, Vector3 outerPoint)
    {
        var dir = (innerPoint - outerPoint).normalized;
        var rotatedVectorA = Quaternion.AngleAxis(35, Vector3.forward) * dir;
        var rotatedVectorB = Quaternion.AngleAxis(-35, Vector3.forward) * dir;
        
        return (rotatedVectorA, rotatedVectorB);
    }

#if USE_GIZMO
    private void OnDrawGizmos()
    {
        float wallThickness = 0.02f;
        
        foreach (var obstacle in m_ObstacleList)
        {
            for (int i = 0; i < obstacle.Count - 1; i++)
            {
                Debug.DrawLine(obstacle[i], obstacle[i + 1], Color.white, 0.05f);
                Debug.DrawRay(obstacle[i], RightDirection(obstacle[i], obstacle[i + 1]) * wallThickness);
                Debug.DrawRay(obstacle[i], -RightDirection(obstacle[i], obstacle[i + 1]) * wallThickness);
            }
            var startCap = CapPoints(obstacle[0], obstacle[1]);
            Debug.DrawRay(obstacle[0], startCap.Item1 * wallThickness, Color.white);
            Debug.DrawRay(obstacle[0], startCap.Item2 * wallThickness, Color.white);
            
            var endCap = CapPoints(obstacle[^1], obstacle[^2]);
            Debug.DrawRay(obstacle[^1], endCap.Item1 * wallThickness, Color.white);
            Debug.DrawRay(obstacle[^1], endCap.Item2 * wallThickness, Color.white);
        }
    }
#endif
}
