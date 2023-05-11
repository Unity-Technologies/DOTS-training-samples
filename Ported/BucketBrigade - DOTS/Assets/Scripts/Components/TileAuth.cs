using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class TileAuth : MonoBehaviour
{
    float Temperature = 0.0f;

    public class TileBaker : Baker<TileAuth>
    {
       
        public override void Bake(TileAuth authoring)
        {
            AddComponent(new Tile
            {
                Temperature = authoring.Temperature,
                rowIndex = -1,
                columnIndex = -1,
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
    public int rowIndex;
    public int columnIndex;
}

public struct OnFire : IComponentData, IEnableableComponent
{}