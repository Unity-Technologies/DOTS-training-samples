using UnityEngine;

/// <summary>
/// Core Building class from which all other Building types are derived
/// </summary>
public class CityBuilding : CityEntity {

    private bool fancyBuilding = false;
    public bool FancyBuilding {
        set { fancyBuilding = value; }
    }

    private int storeyCount = 1;
    public int StoreyCount {
        set { storeyCount = value; }
    }

    [Header("Building Parts")]
    [SerializeField]
    private GameObject buildingStoreySimplePrefab;
    [SerializeField]
    private GameObject buildingStoreyFancyPrefab;
    [SerializeField]
    private GameObject buildingWindowPrefab;
    [SerializeField]
    private GameObject[] buildingWindowClusterPrefabs;
    [SerializeField]
    private GameObject buildingDoorPrefab;

    private Vector3 storeyOffset = new Vector3(0.0f, 10.0f, 0.0f);
    private Vector3 doorOffset = new Vector3(-4.7f,3.5f,0.0f);
    private Vector3 firstStoreyWindowOffset = new Vector3(-4.7f, 5.0f, -3.25f);

    /// <summary>
    /// Constructs a randomized building based on selected parade test configuration
    /// </summary>
    public void constructBuilding()
    {

        GameObject tempPart = null;
        GameObject tempStorey = null;
        Vector3 tempPosition = Vector3.zero;

        if (leftSide)
        {
            gameObject.transform.rotation *= Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
        }

        for (int i = 0; i < storeyCount; i++)
        {

            if (fancyBuilding)
            {

                tempStorey = GameObject.Instantiate(buildingStoreyFancyPrefab, gameObject.transform);
                tempPosition = (i * storeyOffset);
                tempStorey.transform.localPosition = tempPosition;

                // First floor has good chance at getting a door
                if (i == 0)
                {

                    if ((Random.Range(1, 3) == 1))
                    {

                        tempPart = GameObject.Instantiate(buildingDoorPrefab, tempStorey.transform);
                        tempPosition = doorOffset;
                        tempPart.transform.localPosition = tempPosition;

                    }

                    tempPart = GameObject.Instantiate(buildingWindowPrefab, tempStorey.transform);
                    tempPosition = firstStoreyWindowOffset;
                    tempPart.transform.localPosition = tempPosition;

                }
                else
                {
                    tempPart = GameObject.Instantiate(buildingWindowClusterPrefabs[Random.Range(0, buildingWindowClusterPrefabs.Length)], tempStorey.transform);
                }

            }
            else
            {

                tempStorey = GameObject.Instantiate(buildingStoreySimplePrefab, gameObject.transform);
                tempPosition = (i * storeyOffset);
                tempStorey.transform.localPosition = tempPosition;

            }

        }

    }

    protected override void handleCleanup()
    {
    }

}
