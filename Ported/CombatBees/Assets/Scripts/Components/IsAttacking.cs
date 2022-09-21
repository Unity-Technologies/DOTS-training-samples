using Unity.Entities;

struct IsAttacking : IComponentData, IEnableableComponent {
    public bool Value;
}