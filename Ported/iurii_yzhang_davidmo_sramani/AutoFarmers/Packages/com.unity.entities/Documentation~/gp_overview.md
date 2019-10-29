---
uid: gameplay-overview
---

# Creating Gameplay

This section discusses authoring DOTS-based games and other applications in the Unity Editor and the systems and components provided by ECS to help you implement game features.

The systems include:

* Unity.Transforms — provides components for defining world-space transforms, 3D object hierarchies, and systems to manage them.
* Unity.Hybrid.Renderer — provides components and systems for rendering ECS entities in the Unity runtime.

## Authoring Overview

You can use the Unity Editor (with the required DOTS packages) to create DOTS-based games. In the Editor, you author a scene using GameObjects as normal and the ECS code converts the scene GameObjects to entities. The biggest difference when using DOTS is that instead of writing your own MonoBehaviours to store instance data and implement custom game logic, you would typically define ECS components to store the data at runtime and write systems for the custom logic. 

### GameObject conversion

During GameObject conversion, various conversion systems handle the MonoBehaviour components that they recognize and convert them into ECS-based components. For example, one of the the Unity.Transforms conversion systems examines the UnityEngine.Transform component and adds ECS components, such as [LocalToWorld](xref:Unity.Transforms.LocalToWorld), to replace it. You can implement an [IConvertGameObjectToEntity](Unity.Entities.IConvertGameObjectToEntity) MonoBehaviour component to specify custom conversion steps. There often will not be a one-to-one relationship between the number of GameObjects converted and the number of entities created; nor between the number of MonoBehaviours on a GameObject and the number of ECS components added to an entity. 

![](images/CreatingGameplay.png)

The ECS conversion code converts a GameObject if it either has a [ConvertToEntity](xref:Unity.Entities.Hybrid.ConvertToEntity) MonoBehaviour component or it is part of a SubScene. In either case, the conversion systems provided for various DOTS features, such as Unity.Transforms and Unity.Hybrid.Render, process the GameObject or the Scene Asset and any of their child GameObjects. One difference between converting GameObjects with ConvertToEntity and converting with a SubScene is that ECS serializes and saves to disk the entity data generated from converting a SubScene. This serialized data can be loaded or streamed very quickly at run time. In contrast, GameObjects with ConvertToEntity MonoBehaviours are always converted at runtime.

Unity recommends using standard MonoBehaviours for authoring and using IConvertGameObjectToEntity to apply the values of those authoring components to [IComponentData](xref:Unity.Entities.IComponentData) structs for run-time use. Often, the most convenient data layout for authoring is not the most efficient data layout at run time. You can use IConvertGameObjectToEntity to customize the conversion of any GameObject in a SubScene, or that has a ConvertToEntity MonoBehaviour, or that is a child of a GameObject that has a ConvertToEntity MonoBehaviour.

**Note:** at this time, the authoring workflow for DOTS-based applications is an area of active development. The general outlines are in place, but you should anticipate many changes in this area in the near future.



