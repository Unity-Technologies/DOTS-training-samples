using Unity.Entities;


class TeamAuth : UnityEngine.MonoBehaviour
{
    public float baseHealth;
    class TeamAuthBaker : Baker<TeamAuth>
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