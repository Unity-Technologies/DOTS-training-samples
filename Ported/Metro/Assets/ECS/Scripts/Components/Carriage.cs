using Unity.Entities;
using Unity.Mathematics;
public enum CarriageState
{
    DOORS_OPEN,
    UNLOADING,
    LOADING,
    DOORS_CLOSE
}
struct Carriage : IComponentData
{
    public Entity Train;
    public int Index;
    
    public CarriageState State;

    // These entities will reference the CarriageDoors (where doors are located on the carriage).
    public Entity CarriageDoorLeft, CarriageDoorRight;

}
