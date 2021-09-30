using dots_src.Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EntityBufferAuthor : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
        {
            dstManager.AddBuffer<EntityBufferElement>(entity);
        }
    }
}
