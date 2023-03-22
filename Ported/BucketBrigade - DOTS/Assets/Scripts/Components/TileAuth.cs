using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class TileAuth : MonoBehaviour
{
    [Range(-1.0f, 1.0f)]
    public float Temperature = 0.0f;

    public class TileBaker : Baker<TileAuth>
    {
       
        public override void Bake(TileAuth authoring)
        {
            AddComponent(new Tile
            {
                Temperature = authoring.Temperature
            });
            AddComponent<OnFire>();
            AddComponent<URPMaterialPropertyBaseColor>();
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