using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class InvincibilityButton : MonoBehaviour
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
        Debug.Log("Invinsibility Button pressed");
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = entityManager.CreateEntityQuery(QueryDesc);

        using (var entities = query.ToEntityArray(Allocator.TempJob))
        {
            var entity = entities.SingleOrDefault();
            if (entity == Entity.Null)
                return;

            bool isInvincible = entityManager.HasComponent<IsInvincible>(entity);

            if (isInvincible)
                entityManager.RemoveComponent<IsInvincible>(entity);
            else
                entityManager.AddComponent<IsInvincible>(entity);

            text.text = isInvincible ? "Vulnerable" : "Invulnerable";
        }
    }
}
