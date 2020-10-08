using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
[UpdateAfter(typeof(InitBoardSystem))]
public class CursorSystem : SystemBase
{
	protected override void OnUpdate()
	{
		// var tiles = GetSingleton<TileMap>().tiles;
	    var camera = UnityEngine.Camera.main;
	    if (camera == null)
	        return;
	    var boardInfo = GetSingleton<BoardInfo>();
	    var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
	    new UnityEngine.Plane(UnityEngine.Vector3.up, 0.6f).Raycast(ray, out var enter);
	    var hit = (float3) ray.GetPoint(enter);
	    
	    var centerPosition = new float3(math.floor(hit.x)+ 0.5f, 0.6f, math.floor(hit.z)+ 0.5f);
	    int arrowDirection;
	    if ((hit.z - centerPosition.z) > (hit.x - centerPosition.x))
	    {
	        arrowDirection = ((hit.z - centerPosition.z) > -(hit.x - centerPosition.x)) ? 2 : 1;
	    }
	    else
	    {
	        arrowDirection = ((hit.z - centerPosition.z) > -(hit.x - centerPosition.x)) ? 3 : 0;
	    }
	    var arrowRotation = quaternion.RotateY(math.radians((float)(arrowDirection * 90.0f)));
	
	    Entities
	        .WithAll<ArrowCellTag>()
	        .ForEach((ref Rotation rotation, ref Translation translation) =>
	        {
	            if ((centerPosition.x < boardInfo.width) 
	                && (centerPosition.z < boardInfo.height)
	                && (centerPosition.x > 0)
	                && (centerPosition.z > 0))
	            {
	                translation.Value = centerPosition - (new float3(0.5f, 0.0f, 0.5f));
	                rotation.Value = arrowRotation;
	            }
	            else
	            {
	                translation.Value = new float3(1f, -0.5f, 1f);
	            }
	        }).Schedule();
	}
}
