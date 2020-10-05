using UnityEngine;

public class PathManager : MonoBehaviour
{
    #region Public Members
    public Material LineRendererMaterial;

    [Header("Pink Paths")]
    public SWS.PathManager PinkToPinkPathPoints2;
    public SWS.PathManager PinkToPurplePathPoints2;
    public SWS.PathManager PinkToBluePathPoints2;

    [Header("Red Paths")]
    public SWS.PathManager RedToRedPathPoints2;
    public SWS.PathManager RedToBluePathPoints2;
    public SWS.PathManager RedToPurplePathPoints2;

    [Header("Purple Paths")]
    public SWS.PathManager PurpleToPurplePathPoints2;
    public SWS.PathManager PurpleToRedPathPoints2;
    public SWS.PathManager PurpleToPinkPathPoints2;

    [Header("Blue Paths")]
    public SWS.PathManager BlueToBluePathPoints2;
    public SWS.PathManager BlueToPinkPathPoints2;
    public SWS.PathManager BlueToRedPathPoints2;

    [Header("Smoothed Paths")]
    public Vector3[] PinkToPinkSmoothedPath;
    public Vector3[] PinkToPurpleSmoothedPath;
    public Vector3[] PinkToBlueSmoothedPath;

    public Vector3[] RedtoRedSmoothedPath;
    public Vector3[] RedtoBlueSmoothedPath;
    public Vector3[] RedtoPurpleSmoothedPath;

    public Vector3[] PurpleToPurpleSmoothedPath;
    public Vector3[] PurpleToRedSmoothedPath;
    public Vector3[] PurpleToPinkSmoothedPath;

    public Vector3[] BlueToBlueSmoothedPath;
    public Vector3[] BlueToPinkSmoothedPath;
    public Vector3[] BlueToRedSmoothedPath;
    #endregion

    #region Constants
    private const int WAY_POINT_RESOLUTION = 50;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        PinkToPinkSmoothedPath = PinkToPinkPathPoints2.GetPathPoints();
        PinkToPurpleSmoothedPath = PinkToPurplePathPoints2.GetPathPoints();
        PinkToBlueSmoothedPath = PinkToBluePathPoints2.GetPathPoints();

        RedtoRedSmoothedPath = RedToRedPathPoints2.GetPathPoints();
        RedtoBlueSmoothedPath = RedToBluePathPoints2.GetPathPoints();
        RedtoPurpleSmoothedPath = RedToPurplePathPoints2.GetPathPoints();

        PurpleToPurpleSmoothedPath = PurpleToPurplePathPoints2.GetPathPoints();
        PurpleToRedSmoothedPath = PurpleToRedPathPoints2.GetPathPoints();
        PurpleToPinkSmoothedPath = PurpleToPinkPathPoints2.GetPathPoints();

        BlueToBlueSmoothedPath = BlueToBluePathPoints2.GetPathPoints();
        BlueToPinkSmoothedPath = BlueToPinkPathPoints2.GetPathPoints();
        BlueToRedSmoothedPath = BlueToRedPathPoints2.GetPathPoints();
    }
    #endregion

    #region Public API
    public Vector3[] GetPath(Destination start, Destination end, ref bool[] leftTurns)
    {
        //Go to right lane at any point during journey
        var allRightTurns = new bool[] { false, false, false };
        
        //Do one left turn  only
        var oneLeftTurn = new bool[] { true };
        
        //Do Left, Right and another Right turn here
        var leftRightRightTurn = new bool[] { true, false, false };

        switch (start)
        {
            case Destination.Pink:
                switch (end)
                {
                    case Destination.Pink:
                        leftTurns = allRightTurns;
                        return PinkToPinkSmoothedPath;
                    case Destination.Purple:
                        leftTurns = oneLeftTurn;
                        return PinkToPurpleSmoothedPath;
                    case Destination.Blue:
                        leftTurns = leftRightRightTurn;
                        return PinkToBlueSmoothedPath;
                    default:
                        Debug.LogErrorFormat("Not path found for Start: {0} - End: {1} ", start, end);
                        return PinkToPinkSmoothedPath;
                }
            case Destination.Red:
                switch (end)
                {
                    case Destination.Red:
                        leftTurns = allRightTurns;
                        return RedtoRedSmoothedPath;
                    case Destination.Blue:
                        leftTurns = oneLeftTurn;
                        return RedtoBlueSmoothedPath;
                    case Destination.Purple:
                        leftTurns = leftRightRightTurn;
                        return RedtoPurpleSmoothedPath;
                    default:
                        Debug.LogErrorFormat("Not path found for Start: {0} - End: {1} ", start, end);
                        return RedtoRedSmoothedPath;
                }
            case Destination.Purple:
                switch (end)
                {
                    case Destination.Purple:
                        leftTurns = allRightTurns;
                        return PurpleToPurpleSmoothedPath;
                    case Destination.Red:
                        leftTurns = oneLeftTurn;
                        return PurpleToRedSmoothedPath;
                    case Destination.Pink:
                        leftTurns = leftRightRightTurn;
                        return PurpleToPinkSmoothedPath;
                    default:
                        Debug.LogErrorFormat("Not path found for Start: {0} - End: {1} ", start, end);
                        return PurpleToPurpleSmoothedPath;
                }
            case Destination.Blue:
                switch (end)
                {
                    case Destination.Blue:
                        leftTurns = allRightTurns;
                        return BlueToBlueSmoothedPath;
                    case Destination.Pink:
                        leftTurns = oneLeftTurn;
                        return BlueToPinkSmoothedPath;
                    case Destination.Red:
                        leftTurns = leftRightRightTurn;
                        return BlueToRedSmoothedPath;
                    default:
                        Debug.LogErrorFormat("Not path found for Start: {0} - End: {1} ", start, end);
                        return BlueToBlueSmoothedPath;
                }
            default:
                Debug.LogErrorFormat("Not path found for Start: {0} - End: {1} ", start, end);
                return BlueToBlueSmoothedPath;
        }
    }
    #endregion
}