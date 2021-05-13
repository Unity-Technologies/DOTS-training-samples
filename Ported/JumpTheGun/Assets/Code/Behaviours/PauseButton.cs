using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    static public EntityQueryDesc QueryDesc => new EntityQueryDesc()
    {
        All = new ComponentType[]
        {
            ComponentType.ReadWrite<Board>()
        },
    };
    
    public Text text;

    public void ButtonPressed()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = entityManager.CreateEntityQuery(QueryDesc);

        using (var entities = query.ToEntityArray(Allocator.TempJob))
        {
            var entity = entities.SingleOrDefault();
            if (entity == Entity.Null)
                return;

            bool isPaused = entityManager.HasComponent<IsPaused>(entity);

            if (isPaused)
                entityManager.RemoveComponent<IsPaused>(entity);
            else
                entityManager.AddComponentData(entity, new IsPaused { Time = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime });

            text.text = isPaused ? "Pause" : "Paused";
        }
    }
}
