using Unity.Entities;

public class ColorUpdaterSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Create queries and static data only here.

    }
    protected override void OnUpdate()
    {
        Entities
           .WithStructuralChanges()
           .ForEach((Entity entity, ref Color color, in Temperature temperature, in Tile tile) =>
           {

           }).Run();
    }
}