using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(GameObjectBeforeConversionGroup))]
class TransformConversion : GameObjectConversionSystem
{
    private void Convert(Transform transform)
    {
        var entity = GetPrimaryEntity(transform);

        //@TODO: This is not a great dependency to introduce as it results in everything getting rebuilt when moving the root,
        // although we only need to recompute localtoworld... (I predict this won't scale on megacity...)
        DeclareDependency(transform, transform.parent);

        DstEntityManager.AddComponentData(entity, new LocalToWorld { Value = transform.localToWorldMatrix });
        if (DstEntityManager.HasComponent<Static>(entity))
            return;

        var hasParent = HasPrimaryEntity(transform.parent);
        if (hasParent)
        {
            DstEntityManager.AddComponentData(entity, new Translation { Value = transform.localPosition });
            DstEntityManager.AddComponentData(entity, new Rotation { Value = transform.localRotation });

            if (transform.localScale != Vector3.one)
                DstEntityManager.AddComponentData(entity, new NonUniformScale { Value = transform.localScale });

            DstEntityManager.AddComponentData(entity, new Parent { Value = GetPrimaryEntity(transform.parent) });
            DstEntityManager.AddComponentData(entity, new LocalToParent());
        }
        else
        {
            DstEntityManager.AddComponentData(entity, new Translation { Value = transform.position });
            DstEntityManager.AddComponentData(entity, new Rotation { Value = transform.rotation });
            if (transform.lossyScale != Vector3.one)
                DstEntityManager.AddComponentData(entity, new NonUniformScale { Value = transform.lossyScale });
        }
    }
    protected override void OnUpdate()
    {
        Entities.ForEach((Transform transform) =>
        {
            Convert(transform);
        });

        //@TODO: Remove this again once we add support for inheritance in queries
        Entities.ForEach((RectTransform transform) =>
        {
            Convert(transform);
        });
    }
}