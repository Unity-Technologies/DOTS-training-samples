using Unity.Entities;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

public class PlayerSystem : SystemBase
{
    //TODO: Do we need authoring? or can we create the entity here, it didn't seem to work that way...
    // protected override void OnCreate()
    // {
        // var player = EntityManager.CreateEntity();
        // var newInputs = new PlayerInput() { Speed = 1, NeedsReset = false};
        // SetComponent(player, newInputs);
    // }

    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<PlayerInput>();

        var inputs = GetComponent<PlayerInput>(player);

        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha0)) inputs.Speed = 0;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha1)) inputs.Speed = 1;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha2)) inputs.Speed = 2;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha3)) inputs.Speed = 3;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha4)) inputs.Speed = 4;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha5)) inputs.Speed = 5;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha6)) inputs.Speed = 6;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha7)) inputs.Speed = 7;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha8)) inputs.Speed = 8;
        if (UnityInput.GetKeyDown(UnityKeyCode.Alpha9)) inputs.Speed = 9;

        if (UnityInput.GetKeyDown(UnityKeyCode.R))
        {
            inputs.Speed = 1;
            inputs.NeedsReset = true;
        }

        SetComponent(player, inputs);
    }
}