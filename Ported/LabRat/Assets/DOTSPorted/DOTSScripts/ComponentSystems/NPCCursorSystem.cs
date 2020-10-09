using System;
using System.Timers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine.SocialPlatforms.GameCenter;

[UpdateAfter(typeof(InitPlayerSystem))]
public class NPCCursorSystem : SystemBase
{
	private float m_Timer = 0.0f;
	private float2[] m_NpcScreenPos = 
	{
		new float2(0.0f, 0.0f),
		new float2(0.0f, 0.0f), 
		new float2(0.0f, 0.0f),
		new float2(0.0f, 0.0f)
	};
	private float2[] m_NpcScreenPosGoal = 
	{
		new float2(0.0f, 0.0f),
		new float2(0.0f, 0.0f), 
		new float2(0.0f, 0.0f),
		new float2(0.0f, 0.0f)
	};
	private float3[] m_NpcPosGoal = 
	{
		new float3(0.0f, 0.0f, 0.0f),
		new float3(0.0f, 0.0f, 0.0f), 
		new float3(0.0f, 0.0f, 0.0f),
		new float3(0.0f, 0.0f, 0.0f)
	};
	
	float K = 2.0f;
	float maxSpeed = 2.0f;
	protected override void OnCreate()
	{
		RequireSingletonForUpdate<TileMap>();
	}

	private EntityDirection CalculateDirection(float3 position, BoardInfo boardInfo)
	{
		var center = new float3(boardInfo.width / 2, 0.0f, boardInfo.height / 2);
		var direction = math.normalize(center - position);
		if (direction.z > direction.x)
		{
			return (direction.z > -direction.x) ? EntityDirection.Up : EntityDirection.Left;
		}
		else
		{
			return (direction.z > -(direction.x)) ? EntityDirection.Right : EntityDirection.Down;
		}
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

	    var directionValues = Enum.GetValues(typeof(EntityDirection));
	    uint seed = (uint)DateTime.UtcNow.Millisecond + 1;
	    Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);


	    for (var playerNumber = 1; playerNumber < 4; playerNumber++)
	    {
		    if (math.distance(m_NpcScreenPos[playerNumber], m_NpcScreenPosGoal[playerNumber]) < 0.5)
		    {
			    var playerManager = GetSingleton<PlayerManager>();
			    var arrowPosition = new int2((int)(m_NpcPosGoal[playerNumber].x - 0.5f), (int)(m_NpcPosGoal[playerNumber].z - 0.5f));
			    var playerTag = playerManager.Player1;
			    switch (playerNumber)
			    {
				    case 1:
					    playerTag = playerManager.Player1;
					    break;
				    case 2:
					    playerTag = playerManager.Player2;
					    break;
				    case 3:
					    playerTag = playerManager.Player3;
					    break;
			    }

			    var arrowDirection = CalculateDirection(m_NpcPosGoal[playerNumber], boardInfo);
			    PlayerManager.AddBoardArrow(EntityManager, playerTag, arrowDirection, arrowPosition);

			    var foo = random.NextInt(0, boardInfo.width - 1);
			    var centerPosition = new float3(random.NextInt(0, boardInfo.width - 1) + 0.5f, 0, random.NextInt(0, boardInfo.height - 1) + 0.5f);
			    bool validTile = false;
			    if ((centerPosition.x < boardInfo.width)
				    && (centerPosition.z < boardInfo.height)
				    && (centerPosition.x > 0)
				    && (centerPosition.z > 0))
			    {
				    byte tile = TileUtils.GetTile(tiles, (int)(centerPosition.x - 0.5f), (int)(centerPosition.z - 0.5f), boardInfo.width);
				    var notHole = !TileUtils.IsHole(tile);
				    var notBase = TileUtils.BaseId(tile) == -1;
				    var hasArrow = PlayerManager.HasArrow(EntityManager, (int)(centerPosition.x - 0.5f), (int)(centerPosition.z - 0.5f));
				    if (notHole && notBase && !hasArrow)
				    {
					    validTile = true;
				    }
			    }

			    if (validTile)
			    {
				    m_NpcScreenPosGoal[playerNumber] = new float2(camera.WorldToScreenPoint(centerPosition).x, camera.WorldToScreenPoint(centerPosition).y);
				    m_NpcPosGoal[playerNumber] = centerPosition;
			    }
				    
		    }

		    m_NpcScreenPos[playerNumber] += K * Time.DeltaTime * (m_NpcScreenPosGoal[playerNumber] - m_NpcScreenPos[playerNumber]);
		    GameObject.Find("CursorManager").GetComponent<NpcCursorManager>().NpcScreenPos[playerNumber] = m_NpcScreenPos[playerNumber];
		    
	    }
	}
}
