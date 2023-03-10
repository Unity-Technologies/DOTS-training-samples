using Unity.Entities;

//[UpdateAfter(typeof(LaneChangeSystem))]
public partial class CarOvertakeTimerSystem : SystemBase
{
    protected override void OnCreate()
    {
        
    }

    protected override void OnUpdate()
    {
        /*
        CarOvertakeTimerSystem
		Query all cars in 'OvertakeTimerState' and NOT ('ChangeLaneState' or 'ChangingLaneState')
		If car in front
            DISABLE 'OvertakeTimerState'
		If fixed timer is hit
            DISABLE 'OvertakeTimerState'
			TAG 'ChangeLaneState' with desired Lane to change back to original lane
		*/
    }
}
