using Assets.Scripts.Hybrid;

using System;

using Unity.Entities;

public class CursorUpdateSystem : SystemBase
{
    private CursorController cursorController;

    protected override void OnCreate()
    {
        this.cursorController = UnityEngine.Object.FindObjectOfType<CursorController>();
    }

    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((in MousePosition position, in PlayerIndex index, in LabRat_Color color) =>
        {
            cursorController.SetPosition(index.Value, position.Value, color.Value);
        }).Run();
    }
}