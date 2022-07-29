using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FarmerPrefabCreator : MonoBehaviour
{
    public GameObject FarmerPrefab; 
    public int FarmerCost;
    // Start is called before the first frame update
}

public class PrefabCreatorBaker : Baker<FarmerPrefabCreator>
{
    public override void Bake(FarmerPrefabCreator authoring)
    {
        AddComponent(new Ecsprefabcreator
        {
            FarmerPrefab = GetEntity(authoring.FarmerPrefab),
            FarmerCost = authoring.FarmerCost
        });
    }
}

public struct Ecsprefabcreator: IComponentData
{
    public Entity FarmerPrefab;
    public int FarmerCost;
}
