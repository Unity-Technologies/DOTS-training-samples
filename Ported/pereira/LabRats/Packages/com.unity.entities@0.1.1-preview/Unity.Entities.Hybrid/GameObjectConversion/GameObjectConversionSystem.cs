using System;
using System.ComponentModel;
using System.IO;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;
using Component = UnityEngine.Component;
using UnityObject = UnityEngine.Object;

[WorldSystemFilter(WorldSystemFilterFlags.GameObjectConversion)]
public abstract class GameObjectConversionSystem : ComponentSystem
{
    public World DstWorld;
    public EntityManager DstEntityManager;

    GameObjectConversionMappingSystem m_MappingSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_MappingSystem = World.GetExistingSystem<GameObjectConversionMappingSystem>();
        DstWorld = m_MappingSystem.DstWorld;
        DstEntityManager = DstWorld.EntityManager;
    }

    public Entity TryGetPrimaryEntity(Component component)
    {
        return m_MappingSystem.TryGetPrimaryEntity(component != null ? component.gameObject : null);
    }

    public Entity GetPrimaryEntity(Component component)
    {
        return m_MappingSystem.GetPrimaryEntity(component != null ? component.gameObject : null);
    }

    public Entity CreateAdditionalEntity(Component component)
    {
        return m_MappingSystem.CreateAdditionalEntity(component != null ? component.gameObject : null);
    }

    public EntitiesEnumerator GetEntities(Component component)
    {
        return m_MappingSystem.GetEntities(component != null ? component.gameObject : null);
    }

    public Entity TryGetPrimaryEntity(UnityObject uobject)
    {
        return m_MappingSystem.TryGetPrimaryEntity(uobject);
    }

    public Entity GetPrimaryEntity(UnityObject uobject)
    {
        return m_MappingSystem.GetPrimaryEntity(uobject);
    }

    /// <summary>
    /// Returns true if the `uobject` is included in the set of converted objects.
    /// </summary>
    public bool HasPrimaryEntity(UnityObject uobject)
    {
        return m_MappingSystem.HasPrimaryEntity(uobject);
    }

    /// <summary>
    /// Returns true if the GameObject owning `component` is included in the set of converted objects.
    /// </summary>
    public bool HasPrimaryEntity(Component component)
    {
        return m_MappingSystem.HasPrimaryEntity(component != null ? component.gameObject : null);
    }

    public Entity CreateAdditionalEntity(GameObject gameObject)
    {
        return m_MappingSystem.CreateAdditionalEntity(gameObject);
    }

    public EntitiesEnumerator GetEntities(GameObject gameObject)
    {
        return m_MappingSystem.GetEntities(gameObject);
    }

    public void DeclareDependency(GameObject target, GameObject dependsOn)
    {
        m_MappingSystem.DeclareDependency(target, dependsOn);
    }

    public void DeclareDependency(Component target, Component dependsOn)
    {
        if (target != null && dependsOn != null)
            m_MappingSystem.DeclareDependency(target.gameObject, dependsOn.gameObject);
    }

    /// <summary>
    /// Configures rendering data for picking in the editor.
    /// </summary>
    /// <param name="entity">The entity to which we apply the configuration</param>
    /// <param name="pickableObject">The game object that should be picked when clicking on an entity</param>
    /// <param name="hasGameObjectBasedRenderingRepresentation">If there is a game object based rendering representation, like MeshRenderer this should be true. If the only way to render the object is through entities it should be false</param>
    public void ConfigureEditorRenderData(Entity entity, GameObject pickableObject, bool hasGameObjectBasedRenderingRepresentation)
        => m_MappingSystem.ConfigureEditorRenderData(entity, pickableObject, hasGameObjectBasedRenderingRepresentation);

    /// <summary>
    /// DeclareReferencedPrefab includes the referenced Prefab in the conversion process.
    /// Once it has been declared, you can use GetPrimaryEntity to find the Entity for the Prefab.
    /// If the object is a Prefab, all Entities in it will be made part of a LinkedEntityGroup, thus Instantiate will clone the whole group.
    /// All Entities in the Prefab will also be tagged with the Prefab component thus will not be picked up by an EntityQuery by default.
    /// </summary>
    /// <param name="prefab"></param>
    public void DeclareReferencedPrefab(GameObject prefab)
        => m_MappingSystem.DeclareReferencedPrefab(prefab);

    public void DeclareReferencedAsset(UnityObject asset)
        => m_MappingSystem.DeclareReferencedAsset(asset);
    
    public Guid GetGuidForAssetExport(UnityObject asset)
        => m_MappingSystem.GetGuidForAssetExport(asset);

    public Stream TryCreateAssetExportWriter(UnityObject asset)
        => m_MappingSystem.TryCreateAssetExportWriter(asset);

    /// <summary>
    /// Adds a LinkedEntityGroup to the primary Entity of this GameObject for all Entities that are created from this GameObject and its descendants.
    /// As a result, EntityManager.Instantiate and EntityManager.SetEnabled will work on those Entities as a group.
    /// </summary>
    public void DeclareLinkedEntityGroup(GameObject gameObject)
        => m_MappingSystem.DeclareLinkedEntityGroup(gameObject);

    
    // OBSOLETE
    
    [Obsolete("AddReferencedPrefab has been renamed. Use DeclareReferencedPrefab instead (RemovedAfter 2019-10-17) (UnityUpgradable) -> DeclareReferencedPrefab(*)", true)]
    [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
    public void AddReferencedPrefab(GameObject gameObject) => DeclareReferencedPrefab(gameObject);

    [Obsolete("AddDependency has been renamed. Use DeclareDependency instead (RemovedAfter 2019-10-17) (UnityUpgradable) -> DeclareDependency(*)", true)]
    [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
    public void AddDependency(Component target, Component dependsOn) => DeclareDependency(target, dependsOn);

    [Obsolete("AddDependency has been renamed. Use DeclareDependency instead (RemovedAfter 2019-10-17) (UnityUpgradable) -> DeclareDependency(*)", true)]
    [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
    public void AddDependency(GameObject target, GameObject dependsOn) => DeclareDependency(target, dependsOn);

    [Obsolete("AddLinkedEntityGroup has been renamed. Use DeclareLinkedEntityGroup instead (RemovedAfter 2019-10-17) (UnityUpgradable) -> DeclareLinkedEntityGroup(*)", true)]
    [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
    public void AddLinkedEntityGroup(GameObject gameObject) => DeclareLinkedEntityGroup(gameObject);
}
