using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.
partial struct PlayerMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MazeConfig>();
        state.RequireForUpdate<TileBufferElement>();
        

    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {

        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
        DynamicBuffer<TileBufferElement> tiles = SystemAPI.GetSingletonBuffer<TileBufferElement>();
        float dt = state.Time.DeltaTime;
        float3 charPos = new float3(0, 0, 0);
        foreach (var character in SystemAPI.Query<CharacterAspect>())
        {
            float3 tempPos = new float3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")) * character.speed * dt;

            
            int2 pos = new int2((int)character.position.x, (int)character.position.z);

            if (tempPos.x>=0.5f)
            {
                if (tiles[mazeConfig.Get1DIndex(pos.x,pos.y)].RightWall)
                {

                    tempPos.x = 0;
                }
            }
            if (tempPos.x <= -0.5f)
            {
                if (tiles[mazeConfig.Get1DIndex(pos.x, pos.y)].LeftWall)
                {
                    tempPos.x = 0;
                }
            }
            if (tempPos.z >= 0.5f)
            {
                if (tiles[mazeConfig.Get1DIndex(pos.x, pos.y)].UpWall)
                {
                    tempPos.z = 0;
                }
            }
            if (tempPos.x <= -0.5f)
            {
                if (tiles[mazeConfig.Get1DIndex(pos.x, pos.y)].DownWall)
                {
                    tempPos.z = 0;
                }
            }
            character.position += tempPos;
        }

        

        /*Entities.ForEach((in DynamicBuffer<TileBufferElement> myBuff) => {
            for (int i = 0; i < myBuff.Length; i++)
            {
                if(charPos.x-myBuff[i].worldPos.x<1)
                {
                    if (charPos.z-myBuff[i].worldPos.z<1)
                    {
                        //it is on a tile
                        if (myBuff[i].RightWall)
                        {
                            return;
                        }
                        
                        if (myBuff[i].LeftWall)
                        {
                            return;
                        }

                        if (myBuff[i].UpWall)
                        {
                            return;
                        }

                        if (myBuff[i].DownWall)
                        {
                            return;
                        }
                    }
                }
                
            }
        }).Schedule();*/

    }
}

