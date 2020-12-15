using Unity.Entities;
using UnityEngine;

public class ScoreUpdate : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Score>();
    }

    protected override void OnUpdate()
    {
        var scoreEntity = GetSingletonEntity<Score>();
        var score = EntityManager.GetComponentData<Score>(scoreEntity);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            score.Value++;
            Debug.Log("SCORE: " + score.Value);
            SetSingleton(score);
        }
    }
}
