using Unity.Entities;

public partial class BeeStatusDecider : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BeeTag>().ForEach((ref RandomState randomState, ref Agression agression, ref BeeStatus beeStatus) => {
            if (beeStatus.Value == Status.Idle)
            {
                if (randomState.Random.NextFloat() < agression.Value)
                {
                    beeStatus.Value = Status.Attacking; 
                }
                else
                {
                    beeStatus.Value = Status.Attacking;// TODO: Change back to Status.Gathering
                }
            }
        }).Schedule();
    }
}
