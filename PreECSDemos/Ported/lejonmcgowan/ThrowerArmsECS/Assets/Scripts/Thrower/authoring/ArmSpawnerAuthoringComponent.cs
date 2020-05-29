using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
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
    public float[] fingerLengths = {0.2f,0.22f,0.2f,0.16f};
    public float thumbLength = 0.13f;

    private float fingerThickness = 0.05f;
    private float armThickness = 0.15f;
    private float thumbThickness = 0.06f;
    
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
                typeof(Thickness),
                typeof(ArmBasesUp),
                typeof(ArmBasesForward),
                typeof(ArmBasesRight),
                typeof(ArmIdleTarget),
                typeof(ArmIKTarget),
                typeof(JointElementData),
                typeof(ArmAnchorPos),
                typeof(ArmGrabTarget),
                typeof(ArmGrabTimer),
                typeof(ArmIdleSeed),
                typeof(ArmLastRockRecord),
                typeof(Wrist)
        }
            
        );
        
        ComponentTypes fingerComponents = new ComponentTypes(
            typeof(FingerParent),
            typeof(JointElementData),
            typeof(Thickness)
        );
        
        for (int i = 0; i < numArms; i++)
        {

            // Create the Arm entity
            Entity armEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            dstManager.AddComponents(armEntity, armComponents);



            var wristEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            
            dstManager.SetComponentData(armEntity,new Wrist()
            {
                value = wristEntity 
            });
            dstManager.AddComponentData(wristEntity,new Translation());
            dstManager.AddComponentData(wristEntity,new Rotation
            {
                Value = quaternion.identity
            });
            
            dstManager.AddComponent<LocalToWorld>(wristEntity);

            float3 right = transform.right;

            float3 anchorPos = right * i * armSpacing;
            
            SetupArmEntities(dstManager, armEntity,anchorPos);
            

            // Create the simulated finger joint entities (4 per Arm)
            for (int fingerIndex = 0; fingerIndex < 4; fingerIndex++)
            {
                Entity fingerEntity = conversionSystem.CreateAdditionalEntity(gameObject);
#if UNITY_EDITOR            
                dstManager.SetName(fingerEntity, "Simulated Finger");
#endif
                
                dstManager.AddComponents(fingerEntity, fingerComponents);
                dstManager.SetComponentData<FingerParent>(fingerEntity, armEntity);
                dstManager.SetComponentData(fingerEntity, new Thickness()
                {
                    value = fingerThickness
                });
                dstManager.AddComponentData(fingerEntity, new FingerLength()
                {
                    value =  fingerLengths[fingerIndex]
                });
                
                SetupFingerEntities(dstManager, fingerEntity, armEntity, fingerIndex);
            }
            
            //create thumb render (just a finger with different bases vector)
            Entity thumbEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            dstManager.AddComponents(thumbEntity,fingerComponents);
            dstManager.SetComponentData<FingerParent>(thumbEntity,armEntity);
            dstManager.SetComponentData(thumbEntity, new Thickness()
            {
                value = thumbThickness,
            });
            SetupThumbEntity(dstManager,thumbEntity,armEntity);
            
#if UNITY_EDITOR            
            dstManager.SetName(armEntity, "Simulated Arm");
            dstManager.SetName(wristEntity,"Simulated Wrist");
            dstManager.SetName(thumbEntity, "Simulated Thumb");
#endif
        }
        
        
        //kill spanwner entity, it's job is done
        dstManager.DestroyEntity(entity);
        
    }
    
    private void SetupArmEntities(EntityManager dstManager, Entity armEntity, float3 anchorPos)
    {
        var armJoints = dstManager.GetBuffer<JointElementData>(armEntity);
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
        dstManager.SetComponentData<ArmIdleSeed>(armEntity, 0);
        
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
            
            dstManager.AddComponent<LocalToWorld>(armJointEntity);
            dstManager.AddComponent<RenderComponentData>(armJointEntity);
            dstManager.AddComponent<Translation>(armJointEntity);
            dstManager.AddComponent<Rotation>(armJointEntity);
            dstManager.AddComponent<NonUniformScale>(armJointEntity);
            
            dstManager.AddComponent<NonUniformScale>(armJointEntity);
            dstManager.AddComponentData(armJointEntity, new RenderComponentData()
            {
                jointIndex = armJoint,
                entityRef = armEntity
            });
            
            dstManager.AddComponentData(armJointEntity, new Thickness
            {
                value = armThickness
            });
            
#if UNITY_EDITOR            
            dstManager.SetName(armJointEntity, "Renderable Arm Joint");
#endif
        }
    }
    
    private void SetupFingerEntities(EntityManager dstManager,
        Entity fingerEntity, Entity armParentEntity, int fingerIndex)
    {
        var fingerJointBuffer = dstManager.GetBuffer<JointElementData>(fingerEntity);
        fingerJointBuffer.Capacity = 4;        
        int numFingerJoints = fingerJointBuffer.Capacity;
        
        for (int i = 0; i < fingerJointBuffer.Capacity; i++)
        {
            fingerJointBuffer.Add(float3.zero);
        }
        
        dstManager.AddComponentData<FingerParent>(fingerEntity, armParentEntity);
        dstManager.AddComponentData<FingerIndex>(fingerEntity,fingerIndex);
        
        dstManager.AddComponent<FingerGrabTimer>(fingerEntity);
        
        Assert.AreEqual(fingerLengths.Length,4);
        
        // Create the renderable finger joint entities (4 per Arm by default, not counting thumb)
        for (int fingerJoint = 0; fingerJoint < numFingerJoints - 1; fingerJoint++)
        {
            //todo same note as in SetupArmEntities
            
            var fingerJointEntity = dstManager.Instantiate(prefab);
            
            dstManager.AddComponent<LocalToWorld>(fingerJointEntity);
            dstManager.AddComponent<Translation>(fingerJointEntity);
            dstManager.AddComponent<Rotation>(fingerJointEntity);
            dstManager.AddComponent<NonUniformScale>(fingerJointEntity);

            dstManager.AddComponentData(fingerJointEntity,  new RenderComponentData()
            {
                jointIndex = fingerJoint,
                entityRef = fingerEntity
            });
            
            dstManager.AddComponentData(fingerJointEntity, new Thickness()
            {
                value = fingerThickness,
            });
            
#if UNITY_EDITOR            
            dstManager.SetName(fingerJointEntity, "Renderable Finger Joint");
#endif
        }
        
    }
    
    private void SetupThumbEntity(EntityManager dstManager,
        Entity thumbEntity, Entity armParentEntity)
    {
        var thumbJointBuffer = dstManager.GetBuffer<JointElementData>(thumbEntity);
        thumbJointBuffer.Capacity = 4;        
        int numThumbJoints = thumbJointBuffer.Capacity;
        
        for (int i = 0; i < thumbJointBuffer.Capacity; i++)
        {
            thumbJointBuffer.Add(float3.zero);
        }
        
        dstManager.AddComponentData<FingerParent>(thumbEntity, armParentEntity);
     
        
        dstManager.AddComponent<FingerGrabTimer>(thumbEntity);

        
        // Create the renderable finger joint entities (4 per Arm by default, not counting thumb)
        for (int fingerJoint = 0; fingerJoint < numThumbJoints - 1; fingerJoint++)
        {
            //todo same note as in SetupArmEntities
            
            var thumbJointEntity = dstManager.Instantiate(prefab);
            
            dstManager.AddComponent<LocalToWorld>(thumbJointEntity);
            dstManager.AddComponent<Translation>(thumbJointEntity);
            dstManager.AddComponent<Rotation>(thumbJointEntity);
            dstManager.AddComponent<NonUniformScale>(thumbJointEntity);

            dstManager.AddComponentData(thumbJointEntity,  new RenderComponentData()
            {
                entityRef = thumbEntity,
                jointIndex = fingerJoint
            });
            
            dstManager.AddComponentData(thumbJointEntity, new Thickness()
            {
                value = thumbThickness
            });
            
            dstManager.AddComponentData(thumbJointEntity, new FingerLength()
            {
                value = thumbLength
            });
            
#if UNITY_EDITOR            
            dstManager.SetName(thumbJointEntity, "Renderable Thumb Joint");
#endif
            
        }
        
    }

}