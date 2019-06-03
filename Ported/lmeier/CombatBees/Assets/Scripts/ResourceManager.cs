using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector]
    public int2 GridCounts;
    [HideInInspector]
    public float2 GridSize;
    [HideInInspector]
    public float2 MinGridPos;

    public static ResourceManager S;

    [HideInInspector]
    public NativeArray<int> StackHeights;

    public float ResourceSize = 0;
    public int BeesPerResource = 0;
    public int ResourcesAtStart = 0;
    public float spawnRate = .1f;
    float spawnTimer = 0f;

    private void Awake()
    {
        S = this;
        Vector2Int gridCounts = Vector2Int.RoundToInt(new Vector2(Field.size.x, Field.size.z) / ResourceSize);
        GridCounts.x = gridCounts.x;
        GridCounts.y = gridCounts.y;
        GridSize = new Vector2(Field.size.x / GridCounts.x, Field.size.z / GridCounts.y);
        MinGridPos = new Vector2((GridCounts.x - 1f) * -.5f * GridSize.x, (GridCounts.y - 1f) * -.5f * GridSize.y);

        StackHeights = new NativeArray<int>(GridCounts.x * GridCounts.y, Allocator.Persistent);
        for(int i = 0; i < StackHeights.Length; ++i)
        {
            StackHeights[i] = 0;
        }
    }

    private void OnDestroy()
    {
        Sys_ResolveStacks.job.Complete();
        StackHeights.Dispose();
    }

    public GameObject ResourcePrefab;

    public void SpawnResource()
    {
            Vector3 pos = new Vector3(MinGridPos.x * .25f + BeeManager.S.Rand.NextFloat(0, 1) * Field.size.x * .25f, BeeManager.S.Rand.NextFloat(0, 1) * 10f, MinGridPos.y + BeeManager.S.Rand.NextFloat(0, 1) * Field.size.z);
            SpawnResource(pos);
    }
    public void SpawnResource(Vector3 pos)
    {
        var ent = World.Active.EntityManager.Instantiate(ResourceEnt);
        World.Active.EntityManager.SetComponentData(ent, new Unity.Transforms.Translation() { Value = pos });
        //Instantiate(ResourcePrefab, pos, Quaternion.identity, null);
    }
    Entity ResourceEnt;

    // Start is called before the first frame update
    void Start()
    {
        ResourceEnt = GameObjectConversionUtility.ConvertGameObjectHierarchy(ResourcePrefab, World.Active);

        for (int i = 0; i < ResourcesAtStart; ++i)
        {
            SpawnResource();
        }
    }

    private void Update()
    {
        if (MouseRaycaster.isMouseTouchingField)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                spawnTimer += Time.deltaTime;
                while (spawnTimer > 1f / spawnRate)
                {
                    spawnTimer -= 1f / spawnRate;
                    SpawnResource(MouseRaycaster.worldMousePosition);
                }
            }
        }
    }

}
