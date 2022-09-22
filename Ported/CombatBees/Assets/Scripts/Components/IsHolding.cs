using Unity.Entities;

struct IsHolding : IComponentData, IEnableableComponent {
    public bool Value;
}
