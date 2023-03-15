using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TileAuth : MonoBehaviour
{
    public float Temperature = 0.5f;
    public class TileBaker : Baker<TileAuth>
    {
       
        public override void Bake(TileAuth authoring)
        {
            AddComponent(new Tile
            {
                Temperature = authoring.Temperature
            });
            AddComponent<OnFire>();
        }
    }
}

//This is a tag
public struct Tile : IComponentData
{   
    public float Temperature;
}

// This is a tag component that is also an "enableable component".
// Such components can be toggled on and off while remaining present on the entity.
// Doing so is a lot more efficient than adding and removing the component.
// An Enableable component is initially enabled.
public struct OnFire : IComponentData, IEnableableComponent
{}