using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


struct PlayerInput : IComponentData
{
    //public string horizontalAxis;
    //public string verticalAxis;
    public bool trigger;
    
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;


}
