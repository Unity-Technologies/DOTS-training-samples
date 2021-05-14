using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class GameOverSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameStartedTag>(); // require component to remove when game ends
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // reference to our UI canvas elements
        var gameObjectRefs = this.GetSingleton<GameObjectRefs>();

        // find board entity by this tag
        var boardEntity = GetSingletonEntity<BoardInitializedTag>();

        // reference component on board entity to pull score data and check elapsed time
        var gameTime = GetComponent<GameTime>(boardEntity);
        var gameInitParams = GetComponent<GameInitParams>(boardEntity);

        if (gameTime.AccumulatedTime >= gameInitParams.LengthGame) // verify if the accumulated time has reached the defined length of the game
        {
            var ecb = CommandBufferSystem.CreateCommandBuffer();
            ecb.RemoveComponent<GameStartedTag>(boardEntity); // remove the GameStarted tag to stop game progression

            // enable the UI panel with Game Over message
            gameObjectRefs.GameOverPanel.SetActive(true);

            // find each player's score and print to game over panel label the name of the winning player
            var playerReferences = EntityManager.GetBuffer<PlayerReference>(boardEntity);
            var scorePlayer1 = EntityManager.GetComponentData<Score>(playerReferences[0].Player).Value; // red player
            var scorePlayer2 = EntityManager.GetComponentData<Score>(playerReferences[1].Player).Value; // green player
            var scorePlayer3 = EntityManager.GetComponentData<Score>(playerReferences[2].Player).Value; // blue player
            var scorePlayer4 = EntityManager.GetComponentData<Score>(playerReferences[3].Player).Value; // human player

            // if player 1 has higher score than 2, 3 and 4
            if ((scorePlayer1 > scorePlayer2) && (scorePlayer1 > scorePlayer3) && (scorePlayer1 > scorePlayer4))
            {
                gameObjectRefs.GameOverText.text = "Player Wins!";
            }
            // if player 2 has higher score than 1, 3 and 4
            else if ((scorePlayer2 > scorePlayer1) && (scorePlayer2 > scorePlayer3) && (scorePlayer2 > scorePlayer4))
            {
                gameObjectRefs.GameOverText.text = "Green Wins!";
            }
            // if player 3 has higher score than 1, 2 and 4
            else if ((scorePlayer3 > scorePlayer1) && (scorePlayer3 > scorePlayer2) && (scorePlayer3 > scorePlayer4))
            {
                gameObjectRefs.GameOverText.text = "Blue Wins!";
            }
            else if (scorePlayer4 > scorePlayer1 && scorePlayer4 > scorePlayer2 && scorePlayer4 > scorePlayer3)
            {
                gameObjectRefs.GameOverText.text = "Red Wins!";
            }
            else if (scorePlayer1 == scorePlayer2 && scorePlayer2 == scorePlayer3 && scorePlayer3 == scorePlayer4)
            {
                gameObjectRefs.GameOverText.text = "4-way Tie!";
            }
            else if (scorePlayer1 == scorePlayer2 && scorePlayer2 == scorePlayer3)
            {
                gameObjectRefs.GameOverText.text = "Red, Green and Blue Tie!";
            }
            else if (scorePlayer1 == scorePlayer2 && scorePlayer2 == scorePlayer4)
            {
                gameObjectRefs.GameOverText.text = "Red, Green and Player Tie!";
            }
            else if (scorePlayer1 == scorePlayer3 && scorePlayer3 == scorePlayer4)
            {
                gameObjectRefs.GameOverText.text = "Red, Blue and Player Tie!";
            }
            else if (scorePlayer2 == scorePlayer3 && scorePlayer3 == scorePlayer4)
            {
                gameObjectRefs.GameOverText.text = "Green, Blue and Player Tie!";
            }
            // check for tie conditions for player 1
            else if ((scorePlayer1 == scorePlayer2))
            {
                gameObjectRefs.GameOverText.text = "Red and Green Tie!";
            }
            else if ((scorePlayer1 == scorePlayer3))
            {
                gameObjectRefs.GameOverText.text = "Red and Blue Tie!";
            }
            else if ((scorePlayer1 == scorePlayer4))
            {
                gameObjectRefs.GameOverText.text = "Red and Player Tie!";
            }
            // check for tie conditions for player 2
            else if ((scorePlayer2 == scorePlayer3))
            {
                gameObjectRefs.GameOverText.text = "Green and Blue Tie!";
            }
            else if ((scorePlayer2 == scorePlayer4))
            {
                gameObjectRefs.GameOverText.text = "Green and Player Tie!";
            }
            // check for tie conditions for player 3
            else if ((scorePlayer3 == scorePlayer4))
            {
                gameObjectRefs.GameOverText.text = "Blue and Player Tie!";
            }
            // only scenario left is player win
            else
            {
                gameObjectRefs.GameOverText.text = "Red Wins!";
            }
        }
        else
        {
            return;
        }
    }
}
