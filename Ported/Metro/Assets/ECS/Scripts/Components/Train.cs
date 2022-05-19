using Unity.Entities;
public enum TrainState
{
    EN_ROUTE,
    ARRIVING,
    DOORS_OPEN,
    UNLOADING,
    LOADING,
    DOORS_CLOSE,
    DEPARTING,
    EMERGENCY_STOP
}
struct Train : IComponentData
{
    public TrainState State;
    public Entity trainAheadOfMe;

}
