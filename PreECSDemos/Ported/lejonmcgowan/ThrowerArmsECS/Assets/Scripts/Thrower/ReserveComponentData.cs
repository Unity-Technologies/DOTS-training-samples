
using Unity.Entities;

public struct ReserveComponentData: IComponentData
{
    public Entity reserver;
    
    public static implicit operator Entity(ReserveComponentData r) { return r.reserver; }
    public static implicit operator ReserveComponentData(Entity entity)
    {
        var r  = new ReserveComponentData();
        r.reserver = entity;
        return r;
    }
}