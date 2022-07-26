using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FarmerPrefabCreator : MonoBehaviour
{
    public GameObject m_prefab;
    // Start is called before the first frame update
}

public class PrefabCreatorBaker : Baker<FarmerPrefabCreator>
{
    public override void Bake(FarmerPrefabCreator authoring)
    {
        AddComponent(new Ecsprefabcreator
        {
            prefab = GetEntity(authoring.m_prefab)
        });
        AddComponent(new FarmerSpeed());
        AddComponent(new Farmer());

    }
}

public struct Ecsprefabcreator: IComponentData
{
    public Entity prefab;
}
