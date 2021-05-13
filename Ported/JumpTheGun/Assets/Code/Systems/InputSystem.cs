using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityInput = UnityEngine.Input;


[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        Entity boardEntity;
        if (!TryGetSingletonEntity<BoardSize>(out boardEntity))
            return;

        var minMaxHeight = GetComponent<MinMaxHeight>(boardEntity);
        var offsets = GetBuffer<OffsetList>(boardEntity);
        var boardSize = GetComponent<BoardSize>(boardEntity);
        var radiusProperty = GetComponent<Radius>(boardEntity);

        var player = GetSingletonEntity<Player>();
        var playerData = GetComponent<Player>(player);
        float currentTime = (float)Time.ElapsedTime;
        
        //check if we are at the end of a cycle. If not we just exit
        if (currentTime < GetComponent<Time>(player).EndTime)
            return;

        //ray trace mouse position for the board, and figure out a destination position for the ball
        float halfHeight = (minMaxHeight.Value.x + minMaxHeight.Value.y) * 0.5f;            
        Ray ray = Camera.main.ScreenPointToRay(UnityInput.mousePosition);
        Vector3 mouseWorldPos = new Vector3(0, halfHeight, 0);
        float t = (halfHeight - ray.origin.y) / ray.direction.y;
        mouseWorldPos.x = ray.origin.x + t * ray.direction.x;
        mouseWorldPos.z = ray.origin.z + t * ray.direction.z;
        mouseWorldPos.y = halfHeight;
        
        var boardPos = GetComponent<BoardPosition>(player);

        int2 boardSrc = boardPos.Value;
        int2 boardTarget = CoordUtils.WorldToBoardPosition(mouseWorldPos);
        int2 displacement = boardTarget - boardSrc;
        displacement.x = displacement.x > 0 ? 1 : (displacement.x < 0 ? -1 : 0);
        displacement.y = displacement.y > 0 ? 1 : (displacement.y < 0 ? -1 : 0);
        boardTarget = boardSrc + displacement;

        //safety
        boardSrc = CoordUtils.ClampPos(boardSrc, boardSize.Value);
        boardTarget = CoordUtils.ClampPos(boardTarget, boardSize.Value);

        float3 sourcePosition = CoordUtils.BoardPosToWorldPos(boardPos.Value, offsets[CoordUtils.ToIndex(boardSrc, boardSize.Value.x, boardSize.Value.y)].Value + radiusProperty.Value);
        float3 dstPosition    = CoordUtils.BoardPosToWorldPos(boardTarget, offsets[CoordUtils.ToIndex(boardTarget, boardSize.Value.x, boardSize.Value.y)].Value + radiusProperty.Value);

        SetComponent(player, new BallTrajectory { Source = sourcePosition, Destination = dstPosition });
        SetComponent(player, new BoardTarget { Value = boardTarget });
        SetComponent(player, new BoardPosition { Value = boardTarget });
        SetComponent(player, new Time { StartTime = currentTime, EndTime = currentTime + playerData.BounceTime + playerData.CooldownTime });
        SetComponent(player, TraceUtils.GetPlayerArchMovement(boardSrc, boardTarget, offsets, boardSize));
    }
}
