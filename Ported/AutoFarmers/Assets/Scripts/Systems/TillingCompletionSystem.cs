using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TillingCompletionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, ref TillRect tillRect, in Translation translation) =>
            {
                int GridX = (int)translation.Value.x;
                int GridY = (int)translation.Value.z;

                // Should be smarter than this, but for now we're moving in scanline order
                // Visually, we can *move* over the edge of the farm, but we're not tilling over there
                int destX = tillRect.X + tillRect.Width - 1;
                int destY = tillRect.Y + tillRect.Height - 1;
                if (GridX > destX || GridY > destY)
                {
                    EntityManager.RemoveComponent<TillRect>(entity);
                }
            }).Run();
    }
}
