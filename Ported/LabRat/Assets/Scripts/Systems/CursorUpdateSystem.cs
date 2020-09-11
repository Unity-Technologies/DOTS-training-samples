using Assets.Scripts.Hybrid;

using System;

using Unity.Entities;

namespace Assets.Scripts.Systems
{
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
                .ForEach((in MousePosition position, in PlayerIndex index, in Color color) =>
            {
                cursorController.SetPosition(index.Value, position.Value, color.Value);
            }).Run();
        }
    }
}