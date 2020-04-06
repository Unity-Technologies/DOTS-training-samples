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

        
        
        ComponentTypes armComponents = new ComponentTypes(new ComponentType[]
            {
                typeof(ArmBasesUp),
                typeof(ArmBasesForward),
                typeof(ArmBasisRight),
                typeof(ArmIdleTarget),
                typeof(ArmIKTarget),
                typeof(ArmJointElementData),
                typeof(ArmAnchorPos),
                typeof(ArmGrabTarget),
                typeof(ArmGrabTimer),
                typeof(IdleArmSeed),
                typeof(ArmLastRockRecord),
                typeof(Wrist)
        }
            
        );
        
        ComponentTypes fingerComponents = new ComponentTypes(
            typeof(FingerParent),
            typeof(FingerIdleTarget),
            typeof(FingerJointElementData),
            typeof(FingerThickness)
        );
        
        for (int i = 0; i < numArms; i++)
        {

            // Create the Arm entity
            Entity armEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            dstManager.AddComponents(armEntity, armComponents);
#if UNITY_EDITOR            
            dstManager.SetName(armEntity, "Simulated Arm");
#endif


            var wristEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            dstManager.SetName(wristEntity,"Simulated Wrist");
            dstManager.SetComponentData(armEntity,new Wrist()
            {
                value = wristEntity 
            });
            dstManager.AddComponentData(wristEntity,new Translation());
            
            dstManager.AddComponent<LocalToWorld>(wristEntity);

            float3 right = transform.right;

            float3 anchorPos = right * i * armSpacing;
            
            SetupArmEntities(dstManager, armEntity,anchorPos);
            
            // Create the renderable finger joint entities (5 per Arm)
            for (int fingerIndex = 0; fingerIndex < 4; fingerIndex++)
            {
                Entity fingerEntity = conversionSystem.CreateAdditionalEntity(gameObject);
                dstManager.AddComponents(fingerEntity, fingerComponents);
                dstManager.SetName(fingerEntity, "Simulated Finger");
                dstManager.SetComponentData<FingerParent>(fingerEntity, armEntity);
                dstManager.SetComponentData(fingerEntity, new FingerThickness()
                {
                    value = 0.05f
                });
                
                SetupFingerEntities(dstManager, fingerEntity, armEntity, fingerIndex);
            }
            
            //create thumb render (just a finger with different bases vector
            Entity thumbEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            dstManager.AddComponents(thumbEntity,fingerComponents);
            dstManager.SetName(thumbEntity, "Simulated Thumb");
            dstManager.SetComponentData<FingerParent>(thumbEntity,armEntity);
            dstManager.SetComponentData(thumbEntity, new FingerThickness()
            {
                value = 0.06f
            });
            SetupThumbEntity(dstManager,thumbEntity,armEntity);
        }
        
        
        //kill spanwner entity, it's job is done
        dstManager.DestroyEntity(entity);
        
    }
    
    private void SetupArmEntities(EntityManager dstManager, Entity armEntity, float3 anchorPos)
    {
        var armJoints = dstManager.GetBuffer<ArmJointElementData>(armEntity);
        armJoints.Capacity = 3; 
        
        float3 target = new float3(-0.3f, 1.0f, 1.5f);

        for (int i = 0; i < armJoints.Capacity; i++)
        {
            armJoints.Add(float3.zero);
        }
        
        int numArmJoints = armJoints.Length;
        
        dstManager.SetComponentData<ArmAnchorPos>(armEntity, anchorPos);

        dstManager.SetComponentData<ArmIdleTarget>(armEntity, target);
        dstManager.SetComponentData<ArmIKTarget>(armEntity, target);
        dstManager.SetComponentData<IdleArmSeed>(armEntity, 0);
        
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
    private void SetupFingerEntities(EntityManager dstManager,
        Entity fingerEntity, Entity armParentEntity, int fingerIndex)
    {
        var fingerJointBuffer = dstManager.GetBuffer<FingerJointElementData>(fingerEntity);
        fingerJointBuffer.Capacity = 4;        
        int numFingerJoints = fingerJointBuffer.Capacity;
        
        for (int i = 0; i < fingerJointBuffer.Capacity; i++)
        {
            fingerJointBuffer.Add(float3.zero);
        }
        
        dstManager.AddComponentData<FingerParent>(fingerEntity, armParentEntity);
        dstManager.AddComponentData<FingerIndex>(fingerEntity,fingerIndex);
        
        dstManager.AddComponent<FingerIdleTarget>(fingerEntity);
        dstManager.AddComponent<FingerGrabTimer>(fingerEntity);

        
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
            
            dstManager.AddComponentData(fingerJointEntity, new FingerThickness()
            {
                value = 0.05f
            });
        }
        
    }
    
    private void SetupThumbEntity(EntityManager dstManager,
        Entity thumbEntity, Entity armParentEntity)
    {
        var thumbJointBuffer = dstManager.GetBuffer<FingerJointElementData>(thumbEntity);
        thumbJointBuffer.Capacity = 4;        
        int numThumbJoints = thumbJointBuffer.Capacity;
        
        for (int i = 0; i < thumbJointBuffer.Capacity; i++)
        {
            thumbJointBuffer.Add(float3.zero);
        }
        
        dstManager.AddComponentData<FingerParent>(thumbEntity, armParentEntity);
     
        
        dstManager.AddComponent<FingerIdleTarget>(thumbEntity);
        dstManager.AddComponent<FingerGrabTimer>(thumbEntity);

        
        // Create the renderable finger joint entities (4 per Arm by default, not counting thumb)
        for (int fingerJoint = 0; fingerJoint < numThumbJoints - 1; fingerJoint++)
        {
            //todo same note as in SetupArmEntities
            
            var thumbJointEntity = dstManager.Instantiate(prefab);

            dstManager.SetName(thumbJointEntity, "Renderable Thumb Joint");
            
            dstManager.AddComponent<LocalToWorld>(thumbJointEntity);
            dstManager.AddComponent<Translation>(thumbJointEntity);
            dstManager.AddComponent<Rotation>(thumbJointEntity);
            dstManager.AddComponent<NonUniformScale>(thumbJointEntity);

            dstManager.AddComponentData(thumbJointEntity,  new FingerRenderComponentData()
            {
                fingerEntity = thumbEntity,
                jointIndex = fingerJoint
            });
            
            dstManager.AddComponentData(thumbJointEntity, new FingerThickness()
            {
                value = 0.06f
            });
        }
        
    }

}