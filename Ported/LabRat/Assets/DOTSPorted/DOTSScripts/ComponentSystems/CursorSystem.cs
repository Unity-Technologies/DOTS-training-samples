using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(InitBoardSystem))]
public class CursorSystem : SystemBase
{
	protected override void OnCreate()
	{
		RequireSingletonForUpdate<TileMap>();
	}
	

	protected override void OnUpdate()
	{
		Entity tilemapEntity = GetSingletonEntity<TileMap>();
		TileMap tileMap = EntityManager.GetComponentObject<TileMap>(tilemapEntity);
		NativeArray<byte> tiles = tileMap.tiles;
		
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
		        bool validTile = false;
		        if ((centerPosition.x < boardInfo.width)
			        && (centerPosition.z < boardInfo.height)
			        && (centerPosition.x > 0)
			        && (centerPosition.z > 0))
		        {
			        byte tile = TileUtils.GetTile(tiles, (int)(centerPosition.x - 0.5f), (int)(centerPosition.z - 0.5f), boardInfo.width);
			        var notHole = !TileUtils.IsHole(tile);
			        var notBase = TileUtils.BaseId(tile) == -1;
			        if (notHole && notBase)
			        {
				        validTile = true;
				        translation.Value = centerPosition - (new float3(0.5f, 0.0f, 0.5f));
				        rotation.Value = arrowRotation;
			        }
		        }
		        if (!validTile)
	            {
	                translation.Value = new float3(1f, -0.5f, 1f);
	            }
	        }).Schedule();
	}
}
