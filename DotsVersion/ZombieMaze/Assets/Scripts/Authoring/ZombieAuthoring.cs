using Unity.Entities;

public class ZombieAuthoring: UnityEngine.MonoBehaviour
{
}
class ZombieBaker : Baker<ZombieAuthoring>
{
    public override void Bake(ZombieAuthoring authoring)
    {
        AddComponent<Zombie>();
    }
}