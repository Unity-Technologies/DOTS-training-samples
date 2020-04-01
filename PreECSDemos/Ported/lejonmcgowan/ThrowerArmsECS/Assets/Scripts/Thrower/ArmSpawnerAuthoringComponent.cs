using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("ECS Thrower/Thrower Spawner")]
public class ArmSpawnerAuthoringComponent : MonoBehaviour,IConvertGameObjectToEntity,IDeclareReferencedPrefabs
{
    public GameObject meshPrefab;
    public int numArms = 10;
    public float armSpacing = 1f;
    private Entity prefab;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(meshPrefab, settings);
        
        
        EntityArchetype armArchetype = dstManager.CreateArchetype(
            typeof(ArmUpComponentData),
            typeof(ArmForwardComponentData),
            typeof(ArmRightComponentData),
            typeof(ArmIdleTargetComponentData),
            typeof(ArmIKTargetComponentData),
            typeof(ArmJointElementData),
            typeof(AnchorPosComponentData),
            typeof(IdleArmSeedComponentData)
        );
        
        EntityArchetype fingerArchetype = dstManager.CreateArchetype(
            typeof(FingerParentComponentData),
            typeof(FingerIdleTargetComponentData),
            typeof(FingerBaseIdleTargetComponentData),
            typeof(FingerIKTargetComponentData),
            typeof(FingerJointElementData)
        );


        for (int i = 0; i < numArms; i++)
        {

            // Create the Arm entity
            Entity armEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            dstManager.SetArchetype(armEntity, armArchetype);
            dstManager.SetName(armEntity, "Simulated Arm");

            float3 right = transform.right;

            float3 anchorPos = right * i * armSpacing;
            
            SetupArmEntities(dstManager, conversionSystem, armEntity,anchorPos);
            
            // Create the renderable finger joint entities (5 per Arm)
            for (int fingerIndex = 0; fingerIndex < 4; fingerIndex++)
            {
                Entity fingerEntity = conversionSystem.CreateAdditionalEntity(gameObject);
                dstManager.SetArchetype(fingerEntity, fingerArchetype);
                dstManager.SetName(fingerEntity, "Simulated Finger");
                dstManager.SetComponentData<FingerParentComponentData>(fingerEntity, armEntity);

                SetupFingerEntities(dstManager, conversionSystem, fingerEntity, armEntity, fingerIndex);
            }

            //create thumb render (just a finger with different bases vector

        }

        //kill spanwner entity, it's job is done
        dstManager.DestroyEntity(entity);
        
    }
    
    private void SetupArmEntities(EntityManager dstManager, GameObjectConversionSystem conversionSystem, Entity armEntity, float3 anchorPos)
    {
        var armJoints = dstManager.GetBuffer<ArmJointElementData>(armEntity);
        armJoints.Capacity = 3; 
        
        float3 target = new float3(-0.3f, 1.0f, 1.5f);

        for (int i = 0; i < armJoints.Capacity; i++)
        {
            armJoints.Add(float3.zero);
        }
        
        int numArmJoints = armJoints.Length;
        
        dstManager.SetComponentData<AnchorPosComponentData>(armEntity, anchorPos);

        dstManager.SetComponentData<ArmIdleTargetComponentData>(armEntity, target);
        dstManager.SetComponentData<ArmIKTargetComponentData>(armEntity, target);
        dstManager.SetComponentData<IdleArmSeedComponentData>(armEntity, 0);
        
        // Create the renderable Arm joint entities (2 per Arm)
        for (int armJoint = 0; armJoint < numArmJoints - 1; armJoint++)
        {
            /*todo inefficient, but the best I can do with the API. Ideally there'd be something like
                        var armJointEntity = converssionSystem.CreateAdditionalEntity();
                        var hybridArchetype - dstManager.GetArchetype(prefab)
                        var armRenderArchetype = { ... }
                        armRenderArchetype.Add(hybridArchetype
                        
                        dstManager.AddARchetype(armRenderArchetype)
                        
                        OR 
                        
                        var armJointEntity = converssionSystem.InstantiateFromEntity(prefab);
                        var armRenderArchetype = { ... }
                        dstManager.AddArchetype(armRenderArchetype) 
             */
            
            var armJointEntity = dstManager.Instantiate(prefab);

            dstManager.SetName(armJointEntity, "Renderable Arm Joint");
             
            dstManager.AddComponent<LocalToWorld>(armJointEntity);
            dstManager.AddComponent<ArmRenderComponentData>(armJointEntity);
            dstManager.AddComponent<Translation>(armJointEntity);
            dstManager.AddComponent<Rotation>(armJointEntity);
            dstManager.AddComponent<NonUniformScale>(armJointEntity);
            
            dstManager.AddComponentData(armJointEntity, new ArmRenderComponentData()
            {
                jointIndex = armJoint,
                armEntity = armEntity
            });
        }
    }
    private void SetupFingerEntities(EntityManager dstManager, GameObjectConversionSystem conversionSystem,
        Entity fingerEntity, Entity armParentEntity, int fingerIndex)
    {
        var fingerJointBuffer = dstManager.GetBuffer<FingerJointElementData>(fingerEntity);
        fingerJointBuffer.Capacity = 4;        
        int numFingerJoints = fingerJointBuffer.Capacity;
        
        for (int i = 0; i < fingerJointBuffer.Capacity; i++)
        {
            fingerJointBuffer.Add(float3.zero);
        }
        
        dstManager.AddComponentData<FingerParentComponentData>(fingerEntity, armParentEntity);
        dstManager.AddComponentData<FingerIndexComponentData>(fingerEntity,fingerIndex);
        
        dstManager.AddComponent<FingerBaseIdleTargetComponentData>(fingerEntity);
        dstManager.AddComponent<FingerIdleTargetComponentData>(fingerEntity);
        dstManager.AddComponent<FingerIKTargetComponentData>(fingerEntity);
        dstManager.AddComponent<FingerGrabTimerComponentData>(fingerEntity);

        
        // Create the renderable finger joint entities (4 per Arm by default, not counting thumb)
        for (int fingerJoint = 0; fingerJoint < numFingerJoints - 1; fingerJoint++)
        {
            //todo same note as in SetupArmEntities
            
            var fingerJointEntity = dstManager.Instantiate(prefab);

            dstManager.SetName(fingerJointEntity, "Renderable Finger Joint");
            
            dstManager.AddComponent<LocalToWorld>(fingerJointEntity);
            dstManager.AddComponent<Translation>(fingerJointEntity);
            dstManager.AddComponent<Rotation>(fingerJointEntity);
            dstManager.AddComponent<NonUniformScale>(fingerJointEntity);

            dstManager.AddComponentData(fingerJointEntity,  new FingerRenderComponentData()
            {
                jointIndex = fingerJoint,
                fingerEntity = fingerEntity
            });
        }
        
    }

}