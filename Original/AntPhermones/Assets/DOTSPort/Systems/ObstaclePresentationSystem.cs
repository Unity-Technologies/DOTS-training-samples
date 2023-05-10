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
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        var obstacleDrawer = MonoBehaviour.FindObjectOfType<ObstacleDrawer>();

        float MapSize = Mathf.Min((float)globalSettings.MapSizeX, (float)globalSettings.MapSizeY);

        int PrimIndex = -1;
        for (int i = 0; i < buffer.Length; i++)
        {
            List<Vector3> centerPoints = new List<Vector3>();
            ObstacleArcPrimitive prim = buffer[i];

            float Angle = prim.AngleStart;
            float AngleEnd = prim.AngleEnd + ((prim.AngleStart < prim.AngleEnd) ? 0.0f : 2.0f * Mathf.PI);
            Vector2 PrevPoint = prim.Position * MapSize;
            PrevPoint.x += Mathf.Cos(Angle) * prim.Radius * MapSize;
            PrevPoint.y += Mathf.Sin(Angle) * prim.Radius * MapSize;            
            centerPoints.Add(PrevPoint);
            
            while (Angle < AngleEnd)
            {
                Angle += 0.03f;
                Vector2 NextPoint = prim.Position * MapSize;
                NextPoint.x += Mathf.Cos(Angle) * prim.Radius * MapSize;
                NextPoint.y += Mathf.Sin(Angle) * prim.Radius * MapSize;
                centerPoints.Add(NextPoint);
            }
            
            obstacleDrawer.AddObstacle(centerPoints, buffer[i].Radius);
        }
    }
}
