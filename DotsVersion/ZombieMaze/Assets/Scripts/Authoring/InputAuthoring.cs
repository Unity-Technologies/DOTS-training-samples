using Unity.Entities;
using UnityEngine;


public class InputAuthoring : UnityEngine.MonoBehaviour
{
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
}

class InputBaker : Baker<InputAuthoring>
{
    public override void Bake(InputAuthoring authoring)
    {
        AddComponent(new PlayerInput
        {
            upKey = authoring.upKey,
            downKey = authoring.downKey,
            leftKey = authoring.upKey,
            rightKey = authoring.upKey,
            trigger =false,
        }) ;
    }
}
