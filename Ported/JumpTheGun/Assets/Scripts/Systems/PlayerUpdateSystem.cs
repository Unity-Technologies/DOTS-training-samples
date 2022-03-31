using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Color = UnityEngine.Color;

[UpdateAfter(typeof(TerrainAreaSystem))]
public partial class PlayerUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
		RequireSingletonForUpdate<TerrainData>();
		RequireSingletonForUpdate<GameObjectRefs>();
		RequireSingletonForUpdate<OccupiedElement>();
	}
    protected override void OnUpdate()
    {
		var terrainData = GetSingleton<TerrainData>();
		var camera = this.GetSingleton<GameObjectRefs>().Camera;
		var gridEntity = GetSingletonEntity<OccupiedElement>();
		var occupiedGrid = GetBuffer<OccupiedElement>(gridEntity);
		var heightBuffer = this.GetBuffer<HeightElement>(gridEntity, true);
#if NORAY
		float2 refPoint = new float2(Screen.width/2, Screen.height/2);
		float2 inputMousePosition = new float2(Input.mousePosition.x, Input.mousePosition.y);
#else
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
#endif
		var pressed = true; //Input.GetKey(KeyCode.Space);
		
		Entities
			.ForEach((ref ParabolaData parabola, ref NormalizedTime time, in Translation translation, in PlayerTag tag) =>
			{		
				// getting local world position of mouse.  Is where camera ray intersects xz plane with y = 
				float y = (terrainData.MinTerrainHeight + terrainData.MaxTerrainHeight) / 2;

#if NORAY
				float2 diff = inputMousePosition - refPoint;
				int2 boxDiff;
				boxDiff.x = (int)math.round(diff.x/(refPoint.x*0.25f));
				boxDiff.y = (int)math.round(diff.y/(refPoint.y*0.25f));
				boxDiff.x = math.max(-1, math.min(boxDiff.x, 1));
				boxDiff.y = math.max(-1, math.min(boxDiff.y, 1));
				int2 currentPos = TerrainUtility.BoxFromLocalPosition(translation.Value, terrainData.TerrainWidth, terrainData.TerrainLength);
				var movePos = currentPos + boxDiff;
				movePos.x = math.max(0, math.min(movePos.x, terrainData.TerrainWidth-1));
				movePos.y = math.max(0, math.min(movePos.y, terrainData.TerrainLength-1));
#else
				float3 mouseWorldPos = new float3(0, y, 0);
				float t = (y - ray.origin.y) / ray.direction.y;
				mouseWorldPos.x = ray.origin.x + t * ray.direction.x;
				mouseWorldPos.z = ray.origin.z + t * ray.direction.z;
				float3 mouseLocalPos = mouseWorldPos;
				int2 mouseBoxPos = TerrainUtility.BoxFromLocalPosition(mouseLocalPos, terrainData.TerrainWidth, terrainData.TerrainLength);

				// get position targeted by mouse
				int2 movePos = mouseBoxPos;
				int2 currentPos = TerrainUtility.BoxFromLocalPosition(translation.Value, terrainData.TerrainWidth, terrainData.TerrainLength);
				if (math.abs(mouseBoxPos.x - currentPos.x) > 1 || math.abs(mouseBoxPos.y - currentPos.y) > 1)
				{
					// mouse position is too far away.  Find closest position
					movePos = currentPos;
					if (mouseBoxPos.x != currentPos.x)
					{
						movePos.x += mouseBoxPos.x > currentPos.x ? 1 : -1;
					}
					if (mouseBoxPos.y != currentPos.y)
					{
						movePos.y += mouseBoxPos.y > currentPos.y ? 1 : -1;
					}
				}
#endif
				// Did we finish bouncing?
				if (time.value >= 1f)
				{
					int moveIndex = movePos.x + movePos.y * terrainData.TerrainWidth;

					// don't move if target is occupied
					if (occupiedGrid[moveIndex] || pressed == false)
					{
						moveIndex = currentPos.x + currentPos.y * terrainData.TerrainWidth;
						movePos = currentPos;
					}

					time.value = 0f;
					// Recompute a new parabola

                    float brickHeight = heightBuffer[moveIndex];
					float maxHeight = math.max(translation.Value.y, brickHeight) + Constants.BOUNCE_HEIGHT;
					parabola = Parabola.Create(translation.Value.y + Constants.PLAYER_Y_OFFSET, maxHeight + Constants.PLAYER_Y_OFFSET, brickHeight + Constants.PLAYER_Y_OFFSET);
					parabola.startPoint = translation.Value.xz;
					parabola.endPoint = TerrainUtility.LocalPositionFromBox(movePos.x, movePos.y).xz;
					parabola.duration = math.max(1f, math.length(parabola.startPoint - parabola.endPoint)) * Constants.BOUNCE_BASE_DURATION;
				}
			})
			.Schedule();

	}
}
