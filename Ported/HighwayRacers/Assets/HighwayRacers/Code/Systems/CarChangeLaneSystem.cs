using Unity.Entities;

[UpdateAfter(typeof(CarMoveSystem))]
public partial class CarChangeLaneSystem : SystemBase
{
    protected override void OnCreate()
    {
        
    }

    protected override void OnUpdate()
    {
        /*
		CarChangeLaneSystem
		Query all cars wanting to change lanes (ChangeLaneState).
		If possible to change lanes,
            DISABLE 'ChangeLaneState'
			ENABLE 'ChangingLaneState'
			create a "ghost" car in target lane
		*/
    }
}
