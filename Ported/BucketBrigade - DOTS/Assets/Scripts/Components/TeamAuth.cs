using Unity.Entities;


class TeamAuth : UnityEngine.MonoBehaviour
{
    class TeamBaker : Baker<TeamAuth>
    {
        public override void Bake(TeamAuth authoring)
        {
            AddComponent<Team>();
        }
    }
   
}


struct Team : IComponentData
{
}