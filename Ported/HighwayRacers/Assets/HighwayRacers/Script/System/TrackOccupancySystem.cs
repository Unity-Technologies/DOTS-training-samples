using Unity.Entities;

// Update the track-tile occupancy after CarMovementSystem so that we have the latest car offsets.
// CarMovementSystem will use last frame's tile knowledge to avoid collisions.
[UpdateAfter(typeof(CarMovementSystem))]
public class TrackOccupancySystem : SystemBase
{
    public float TrackSize = 20;
    public uint LaneCount = 4;

    
    
    protected override void OnUpdate()
    {
// todo read the current 'Offset' and 'Lane' of each vehicle entitiy.
// todo based on that determine the 'tiles' that the car is in and block that tile for other cars.
// todo store this data on '4' "Lane" enitities that use a DynamicBuffer in its component?
// todo for cars NOT switching lanes, we only have to check our lane and the 'tile' in front our current tile.
// todo we can use last frames 'tile' information in "CarMovementSystem"
// todo cars that are going slower than desired (blocked) want to switch lanes and need to check the lane to the right
//      or to the left. We will alternative right and left every other frame so we don't ahve to worry about
//      two cars merging into the same lane.

    }
}
