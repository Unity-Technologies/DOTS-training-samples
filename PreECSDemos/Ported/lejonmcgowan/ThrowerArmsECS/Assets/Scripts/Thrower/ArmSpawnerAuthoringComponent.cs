using System.Collections.Generic;
using System.Transactions;
using TreeEditor;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("ECS Thrower/Hand Renderer")]
public class ArmSpawnerAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject meshPrefab;
    public int numArms;

    private void Solve(float3[] chain, float boneLength, float3 anchor, float3 target, float3 bendHint)
    {
        chain[chain.Length - 1] = target;
        for (int i = chain.Length - 2; i >= 0; i--)
        {
            chain[i] += bendHint;
            float3 delta = chain[i] - chain[i + 1];
            chain[i] = chain[i + 1] + math.normalize(delta) * boneLength;
        }

        chain[0] = anchor;
        for (int i = 1; i < chain.Length; i++)
        {
            float3 delta = chain[i] - chain[i - 1];
            chain[i] = chain[i - 1] + math.normalize(delta) * boneLength;
        }
    }

    void createEntities(float3[] chain, float thickness, float3 up, ref EntityManager dstManager, Entity prefab)
    {
        // find the rendering matrices for an IK chain
        // (each pair of neighboring points is connected by a beam)
        for (int i = 0; i < chain.Length - 1; i++)
        {
            float3 delta = chain[i + 1] - chain[i];

            Entity entity = dstManager.Instantiate(prefab);
            NonUniformScale scale = new NonUniformScale()
            {
                Value = new float3(thickness, thickness, math.length(delta))
            };
            Translation translation = new Translation()
            {
                Value = chain[i] + delta * .5f
            };
            Rotation rotation = new Rotation()
            {
                Value = quaternion.LookRotation(delta, up)
            };

            dstManager.SetComponentData(entity, translation);
            dstManager.SetComponentData(entity, rotation);
            dstManager.AddComponentData(entity, scale);
        }
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        float3 position = transform.position;
        quaternion rotation = transform.rotation;

        MeshSpawnComponentData spawnComponent = new MeshSpawnComponentData()
        {
            prefab = conversionSystem.GetPrimaryEntity(meshPrefab),
            transform = new LocalToWorld()
            {
                Value = float4x4.TRS(position, rotation, new float3(1, 1, 1))
            },
            numToSpawn = numArms
        };

        dstManager.AddComponentData(entity, spawnComponent);

        for (int armIter = 0; armIter < spawnComponent.numToSpawn; armIter++)
        {
            float3 offset = new float3(spawnComponent.numToSpawn / 2.0f - armIter, 0, 0);
            //spawn arms here
            float3[] armChain = new float3[3];
            float3 target = position - offset + new float3(0, 1, 1.5f);
            Solve(armChain, 1f, position - offset, target, float3.zero);

            float3 handForward = math.normalize(armChain[armChain.Length - 1] - armChain[armChain.Length - 2]);
            float3 handUp = math.normalize(math.cross(handForward, transform.right));
            float3 handRight = math.cross(handUp, handForward);

            createEntities(armChain, 0.15f, handUp, ref dstManager, spawnComponent.prefab);

            //spawn fingers
            for (int i = 0; i < 4; i++)
            {
                float3 armPos = armChain[armChain.Length - 1];

                float3 fingerPos = armPos + handRight * (-0.12f + i * 0.08f);
                float3 fingerTtarget = fingerPos + handForward * 0.5f;
                float3[] fingerChain = new float3[4];
                for (int j = 0; j < fingerChain.Length; j++) fingerChain[j] = float3.zero;

                Solve(fingerChain, 0.22f, fingerPos, fingerTtarget, 0.2f * handUp);

                for (int j = 0; j < fingerChain.Length - 1; j++)
                {
                    float3 delta = fingerChain[j + 1] - fingerChain[j];

                    var posDebug = new Vector3(fingerChain[j].x, fingerChain[j].y, fingerChain[j].z);

                    Debug.DrawLine(posDebug, posDebug + new Vector3(delta.x, delta.y, delta.z), Color.red, 240f);
                    Debug.DrawLine(posDebug, posDebug + new Vector3(handUp.x, handUp.y, handUp.z), Color.green, 240f);
                }

                createEntities(fingerChain, 0.05f, handUp, ref dstManager, spawnComponent.prefab);
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}