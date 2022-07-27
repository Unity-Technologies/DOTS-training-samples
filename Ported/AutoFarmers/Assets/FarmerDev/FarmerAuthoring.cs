using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FarmerAuthoring : MonoBehaviour
{
    // Start is called before the first frame update
}

public class FarmerComponentBaker : Baker<FarmerAuthoring>
{
    public override void Bake(FarmerAuthoring authoring)
    {
        AddComponent(new FarmerSpeed());
        AddComponent(new Farmer());

    }
}
