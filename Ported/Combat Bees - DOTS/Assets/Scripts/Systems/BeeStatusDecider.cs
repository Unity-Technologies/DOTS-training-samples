using Unity.Entities;

public partial class BeeStatusDecider : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BeeTag>().ForEach((ref RandomState randomState, ref Agression agression, ref BeeStatus beeStatus, in BeeDead beeDead) => {
            if (beeStatus.Value == Status.Idle && !beeDead.Value)
            {
                agression.Value = randomState.Value.NextFloat(0, 10);
                if (randomState.Value.NextFloat() < agression.Value)
                {
                    beeStatus.Value = Status.Attacking; 
                }
                else
                {
                    beeStatus.Value = Status.Gathering;// TODO: Change back to Status.Gathering
                }
            }
        }).Schedule();
    }
}
