using UnityEngine;

/// <summary>
/// Core object that acts as container for instantiated Person objects. Has variable density. Responsible for deciding where to place Person objects when a new city block is generated.
/// </summary>
public class Crowd : CityEntity {

    private struct PersonSlot
    {
        public Vector3 slotPosition;
        public int slotDensity;

        public PersonSlot(Vector3 pos, int density)
        {
            slotPosition = pos;
            slotDensity = density;
        }
    }

    private int personSlotX = 3;
    private int personSlotY = 5;
    private float personSlotSize = 1.0f;
    private float personSlotGap = 1.0f;
    private float personPositionNoise = 0.6f;
    private PersonSlot[][] personSlots;
    private Vector3 middlingOffset = Vector3.zero;
    private bool crowdCreated = false;

    void Awake ()
    {

        middlingOffset = new Vector3((-2.5f + personSlotGap / 2), 0.0f, (-4.5f + personSlotGap / 2));

        personSlots = new PersonSlot[personSlotX][];
        personSlots[0] = new PersonSlot[personSlotY];
        personSlots[1] = new PersonSlot[personSlotY];
        personSlots[2] = new PersonSlot[personSlotY];

        for (int i = 0; i < personSlotX; i++)
        {
            for (int j = 0; j < personSlotY; j++)
            {
                personSlots[i][j] = new PersonSlot(middlingOffset + new Vector3((i * personSlotSize) + (i * personSlotGap), 0.0f, (j * personSlotSize) + (j * personSlotGap)),0);
            }
        }

        // Slot density determines crowd "fill rate" based on test settings
        personSlots[0][0].slotDensity = 5;
        personSlots[1][0].slotDensity = 2;
        personSlots[2][0].slotDensity = 5;
        personSlots[0][1].slotDensity = 4;
        personSlots[1][1].slotDensity = 6;
        personSlots[2][1].slotDensity = 3;
        personSlots[0][2].slotDensity = 6;
        personSlots[1][2].slotDensity = 1;
        personSlots[2][2].slotDensity = 6;
        personSlots[0][3].slotDensity = 3;
        personSlots[1][3].slotDensity = 6;
        personSlots[2][3].slotDensity = 4;
        personSlots[0][4].slotDensity = 5;
        personSlots[1][4].slotDensity = 2;
        personSlots[2][4].slotDensity = 5;

    }

    public void CreateCrowd()
    {

        if(crowdCreated == false)
        {

            crowdCreated = true;
            GameObject tempPerson = null;

            for (int i = 0; i < personSlotX; i++)
            {
                for (int j = 0; j < personSlotY; j++)
                {

                    if(personSlots[i][j].slotDensity <= CityStreamManager.Instance.CrowdDensityFactor)
                    {

                        tempPerson = Instantiate(Resources.Load(ParadeConstants.CityObjectsPath + "Person", typeof(GameObject)), CityObjectContainer.Instance.gameObject.transform) as GameObject;
                        tempPerson.transform.position = gameObject.transform.position + personSlots[i][j].slotPosition;
                        tempPerson.transform.position += new Vector3(Random.Range(-personPositionNoise, personPositionNoise), 0.0f, Random.Range(-personPositionNoise, personPositionNoise));

                        if (leftSide)
                        {
                            tempPerson.transform.rotation *= Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
                        }

                        tempPerson.GetComponent<Person>().BlockIndex = CityStreamManager.Instance.CurrentCityBlock;

                        if (Random.Range(1, CityStreamManager.Instance.BalloonChance) == 1)
                        {

                            GameObject tempBalloon = Instantiate(Resources.Load(ParadeConstants.CityObjectsPath + "Balloon", typeof(GameObject)), CityObjectContainer.Instance.gameObject.transform) as GameObject;
                            tempBalloon.transform.position = tempPerson.transform.position + tempPerson.transform.right * 0.2f;
                            EntityManager.Instance.AddDistraction(tempBalloon.GetComponent<Balloon>());

                        }

                        if (CityStreamManager.Instance.RandomizedPersonTimeOffset)
                        {
                            tempPerson.GetComponent<Person>().RandomDelayCountdown = CityStreamManager.Instance.RandomPersonTimeOffset;
                        }

                        EntityManager.Instance.AddPerson(tempPerson.GetComponent<Person>());

                    }

                }

            }

        }

    }

    protected override void handleCleanup()
    {
    }


#if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void OnDrawGizmos()
    {

        Color c = Color.red;
        c.a = 0.5f;
        Gizmos.color = c;

        if (drawDebugGizmos && personSlots != null)
        {
            for (int i = 0; i < personSlotX; i++)
            {
                for (int j = 0; j < personSlotY; j++)
                {
                    Gizmos.DrawSphere(transform.position + personSlots[i][j].slotPosition, 0.25f);
                    Gizmos.DrawWireSphere(transform.position + personSlots[i][j].slotPosition, 0.25f);
                }
            }
        }

    }

#endif

}

