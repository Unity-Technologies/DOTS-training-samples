using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UpdateUISystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BoardInitializedTag>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var gameObjectRefs = this.GetSingleton<GameObjectRefs>();
        gameObjectRefs.GameTime.text = "0:00";

        var boardEntity = GetSingletonEntity<BoardInitializedTag>();
        var gameTime = GetComponent<GameTime>(boardEntity);

        float previousAccumulatedTime = gameTime.AccumulatedTime;
        gameTime.AccumulatedTime += deltaTime;
        EntityManager.SetComponentData(boardEntity, gameTime);

        if (gameTime.AccumulatedTime < -2.5f)
        {
            gameObjectRefs.IntroText.enabled = true;
            gameObjectRefs.IntroText.text = "Ready...";
        }
        else if (gameTime.AccumulatedTime < 0.0f)
        {
            gameObjectRefs.IntroText.text = "Set...";
        }
        else
        {
            if (previousAccumulatedTime < 0.0f)
            {
                gameObjectRefs.IntroText.text = "Go!";
                EntityManager.AddComponent<GameStartedTag>(boardEntity);
            }

            if (gameTime.AccumulatedTime > 0.75f)
            {
                gameObjectRefs.IntroText.text = "";
                gameObjectRefs.IntroText.enabled = false;
            }
            gameObjectRefs.GameTime.text = $"0:{(int)gameTime.AccumulatedTime}";
        }

        var playerReferences = EntityManager.GetBuffer<PlayerReference>(boardEntity);
        var scorePlayer1 = EntityManager.GetComponentData<Score>(playerReferences[0].Player).Value;
        var scorePlayer2 = EntityManager.GetComponentData<Score>(playerReferences[1].Player).Value;
        var scorePlayer3 = EntityManager.GetComponentData<Score>(playerReferences[2].Player).Value;
        var scorePlayer4 = EntityManager.GetComponentData<Score>(playerReferences[3].Player).Value;
        gameObjectRefs.Player1ScoreText.text = $"{scorePlayer1}";
        gameObjectRefs.Player2ScoreText.text = $"{scorePlayer2}";
        gameObjectRefs.Player3ScoreText.text = $"{scorePlayer3}";
        gameObjectRefs.Player4ScoreText.text = $"{scorePlayer4}";
        

    }
}
