using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;
using Component = UnityEngine.Component;
using UnityObject = UnityEngine.Object;

//@TODO
//namespace Unity.Entities
//{
partial class GameObjectConversionSystem
{
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
    
    [Obsolete("GameObjectConversionSystem.DstWorld has been deprecated, it is not recommended to access the world from a conversion system. If you really need it you can use DstEntityManager.World but generally it is not recommended to use it.")]
    [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
    public World DstWorld => m_MappingSystem.DstEntityManager.World;

}
//}
