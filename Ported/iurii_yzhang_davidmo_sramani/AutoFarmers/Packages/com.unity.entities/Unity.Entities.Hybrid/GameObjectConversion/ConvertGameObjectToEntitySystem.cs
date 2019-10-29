using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities
{
    public interface IConvertGameObjectToEntity
    {
        void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem);
    }

    public interface IDeclareReferencedPrefabs
    {
        void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresEntityConversionAttribute : Attribute {  }
}

namespace Unity.Entities.Conversion
{
    class ConvertGameObjectToEntitySystem : GameObjectConversionSystem
    {
        void Convert(Transform transform, List<IConvertGameObjectToEntity> convertibles)
        {
            try
            {
                transform.GetComponents(convertibles);

                foreach (var c in convertibles)
                {
                    var behaviour = c as Behaviour;
                    if (behaviour != null && !behaviour.enabled) continue;

#if UNITY_EDITOR
                    if (!ShouldRunConversionSystem(c.GetType()))
                        continue;
#endif

                    var entity = GetPrimaryEntity((Component)c);
                    c.Convert(entity, DstEntityManager, this);
                }
            }
            catch (Exception x)
            {
                Debug.LogException(x, transform);
            }
        }

        protected override void OnUpdate()
        {
            var convertibles = new List<IConvertGameObjectToEntity>();

            Entities.ForEach((Transform transform) => Convert(transform, convertibles));
            convertibles.Clear();

            //@TODO: Remove this again once we add support for inheritance in queries
            Entities.ForEach((RectTransform transform) => Convert(transform, convertibles));
        }
    }

    [UpdateInGroup(typeof(GameObjectBeforeConversionGroup))]
    class ComponentDataProxyToEntitySystem : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Transform transform) =>
            {
                GameObjectConversionMappingSystem.CopyComponentDataProxyToEntity(DstEntityManager, transform.gameObject, GetPrimaryEntity(transform));
            });
            //@TODO: Remove this again once KevinM adds support for inheritance in queries
            Entities.ForEach((RectTransform transform) =>
            {
                GameObjectConversionMappingSystem.CopyComponentDataProxyToEntity(DstEntityManager, transform.gameObject, GetPrimaryEntity(transform));
            });
        }
    }
}
