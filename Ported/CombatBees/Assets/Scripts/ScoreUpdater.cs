using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdater : MonoBehaviour
{
    public Text text;

    void Update()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        using (var queryA = entityManager.CreateEntityQuery(typeof(TeamA)))
        {
            var count = queryA.CalculateEntityCount();
            text.text = $"Team A : {count}\n";
        }
        using (var queryB = entityManager.CreateEntityQuery(typeof(TeamB)))
        {
            var count = queryB.CalculateEntityCount();
            text.text += $"Team B : {count}\n";
        }
        using (var queryR = entityManager.CreateEntityQuery(typeof(Resource)))
        {
            var count = queryR.CalculateEntityCount();
            text.text += $"Resources : {count}\n";
        }
        using (var queryBlood = entityManager.CreateEntityQuery(typeof(Blood)))
        {
            var count = queryBlood.CalculateEntityCount();
            GameStats.BloodCount.Sample(count);
        }
    }
}
