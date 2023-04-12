using Unity.Entities;


class TeamAuth : UnityEngine.MonoBehaviour
{
    public int Value = 0;
    class TeamBaker : Baker<TeamAuth>
    {
        public override void Bake(TeamAuth authoring)
        {
            AddSharedComponent(new Team
            {
                Value = authoring.Value
            });
        }
    }
   
}


public struct Team : ISharedComponentData
{
    public int Value;
}
