using Unity.Entities;

[UpdateAfter(typeof(CarChangeLaneSystem))]
public partial class CarChangingLaneSystem : SystemBase
{
    protected override void OnCreate()
    {
        
    }

    protected override void OnUpdate()
    {
        /*
        CarChangingLaneSystem
		Query all cars in 'ChangingLaneState'
		Move cars in direction of lane
		Once desired lane has been reached,
			Remove car from previous lane
            DISABLE 'ChangingLaneState'
			ENABLE 'OvertakeTimerState'
		*/
    }
}
