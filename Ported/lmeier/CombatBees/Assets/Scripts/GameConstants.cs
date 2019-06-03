using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering;
using Unity.Entities;

public class GameConstants : MonoBehaviour
{
    public static GameConstants S { get; private set; }

    private void Awake()
    {
        if (S == null) { S = this; }
        else { Debug.LogError("Attempt to create multiple Constant Managers."); }

        BloodEnt = GameObjectConversionUtility.ConvertGameObjectHierarchy(BloodPrefab, World.Active);
        SmokeEnt = GameObjectConversionUtility.ConvertGameObjectHierarchy(SmokePrefab, World.Active);

        BloodRenderMesh = World.Active.EntityManager.GetSharedComponentData<RenderMesh>(BloodEnt);
        SmokeRenderMesh = World.Active.EntityManager.GetSharedComponentData<RenderMesh>(SmokeEnt);
    }

    public float Gravity;

    public GameObject BloodPrefab;
    public GameObject SmokePrefab;

    public Entity BloodEnt;
    public Entity SmokeEnt;

    [HideInInspector]
    public RenderMesh BloodRenderMesh;
    [HideInInspector]
    public RenderMesh SmokeRenderMesh;


}
