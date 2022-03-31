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
    protected override void OnUpdate()
    {
		var terrainData = GetSingleton<TerrainData>();
		var camera = this.GetSingleton<GameObjectRefs>().Camera;
		var gridEntity = GetSingletonEntity<OccupiedElement>();
		var occupiedGrid = GetBuffer<OccupiedElement>(gridEntity);
		var brickGrid = GetBuffer<EntityElement>(gridEntity);
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		var pressed = Input.GetKey(KeyCode.Space);

		Entities
			.ForEach((ref ParabolaData parabola, ref NormalizedTime time, in Translation translation, in PlayerTag tag) =>
			{
				// getting local world position of mouse.  Is where camera ray intersects xz plane with y = 
				float y = (terrainData.MinTerrainHeight + terrainData.MaxTerrainHeight) / 2;

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

					Entity brickEntity = brickGrid[moveIndex];

					time.value = 0f;
					// Recompute a new parabola

					float brickHeight = GetComponent<Brick>(brickEntity).height;
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
