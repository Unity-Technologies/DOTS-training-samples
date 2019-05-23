/// <summary>
/// Core class for all Distractions (e.g. Balloons, etc.)
/// </summary>
public class Distraction : CityEntity
{

    private bool isCurrentlyADistraction = false;
    public bool IsCurrentADistraction {
        get { return isCurrentlyADistraction; }
        set { isCurrentlyADistraction = value; }
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool Equals(Distraction other)
    {
        return (other.gameObject.GetInstanceID() == this.gameObject.GetInstanceID());
    }

    protected override void handleCleanup()
    {
        EntityManager.Instance.RemoveDistraction(this);
    }

}
