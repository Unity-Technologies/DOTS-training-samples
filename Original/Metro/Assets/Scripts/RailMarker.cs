using UnityEngine;

public enum RailMarkerType
{
	PLATFORM_START,
	PLATFORM_END,
	ROUTE
}

public class RailMarker : MonoBehaviour
{
	public RailMarkerType railMarkerType;
}
