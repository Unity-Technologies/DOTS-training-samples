using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct ObstaclePresentationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleArcPrimitive>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        
        var buffer = SystemAPI.GetSingletonBuffer<ObstacleArcPrimitive>();

        var obstacleDrawer = MonoBehaviour.FindObjectOfType<ObstacleDrawer>();
        
        int PrimIndex = -1;
        for (int i = 0; i < buffer.Length; i++)
        {
            List<Vector3> centerPoints = new List<Vector3>();
            ObstacleArcPrimitive prim = buffer[i];

            float Angle = prim.AngleStart;
            float AngleEnd = prim.AngleEnd + ((prim.AngleStart < prim.AngleEnd) ? 0.0f : 2.0f * Mathf.PI);
            Vector2 PrevPoint = prim.Position;
            PrevPoint.x += Mathf.Cos(Angle) * prim.Radius;
            PrevPoint.y += Mathf.Sin(Angle) * prim.Radius;            
            centerPoints.Add(PrevPoint);
            
            while (Angle < AngleEnd)
            {
                Angle += 0.1f;
                Vector2 NextPoint = prim.Position;
                NextPoint.x += Mathf.Cos(Angle) * prim.Radius;
                NextPoint.y += Mathf.Sin(Angle) * prim.Radius;
                centerPoints.Add(NextPoint);
            }
            
            obstacleDrawer.AddObstacle(centerPoints);
        }
    }
}
