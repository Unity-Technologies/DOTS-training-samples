using UnityEngine;

/// <summary>
/// The main logic that controls the Parade and related Test conditions. Responsible for Instantiating City objects, test book keeping, etc.
/// </summary>
public class CityStreamManager : MonoBehaviour {

    private static CityStreamManager _instance;
    public static CityStreamManager Instance {
        get { return _instance; }
    }

    private float maxSpeed = 500.0f;
    private float speedChangeStep = 5.0f;
    private bool started = false;
    public bool Started {
        get { return started; }
    }

    // City Stream Parameters //
    private float cityMovementSpeed = 5.0f;
    public float CityMovementSpeed {

        get {

            if (started)
            {
                return cityMovementSpeed;
            }
            else
            {
                return 0.0f;
            }

        }

    }

    private int bufferLeadDistanceInBlocks = 5;
    private float bufferCleanupDistance = 50.0f;
    public float BufferCleanupDistance {
        get { return bufferCleanupDistance; }
    }
    private float bufferPosition = 0.0f;
    private float bufferOffset = 0.0f;
    private float nextBufferTarget = 0.0f;

    private int currentCityBlock = 0;
    public int CurrentCityBlock {
        get { return currentCityBlock; }
    }

    [Header("Configuration Preset (Optional)")]
    [SerializeField, Tooltip("Place a preset here to quickly swap settings. A preset will override the Test Configuration.")]
    private ParadeConfiguration configurationPreset;
    public ParadeConfiguration ConfigurationPreset {
        set { configurationPreset = value; }
    }

    private string currentConfigName = "";
    public string CurrentConfigName {
        get { return currentConfigName; }
    }

    [Header("Test Configuration (Only works if no preset is specified)")]

    // GENERAL //
    [SerializeField, Tooltip("")]
    private int testBlocksToTravel = 10;
    public int TestBlocksToTravel {
        get { return testBlocksToTravel; }
    }
    [Range(0.5f, 500.0f), SerializeField, Tooltip("")]
    private float testCameraSpeed = 5.0f;

    [SerializeField]
    private bool enableTestUI = true;
    public bool EnableTestUI {
        get { return enableTestUI; }
    }

    [SerializeField]
    private bool generateRoads = true;
    [SerializeField]
    private bool generateBuildings = true;
    [SerializeField]
    private bool generateCrowds = true;

    // CROWD //
    [Header("Crowd")]
    [Range(1,6),SerializeField]
    private int crowdDensityFactor = 6;
    public int CrowdDensityFactor {
        get { return crowdDensityFactor; }
    }

    [Range(1, 50), SerializeField]
    private int balloonChance = 50;
    public int BalloonChance {
        get { return balloonChance; }
    }

    [Range(1, 500), SerializeField]
    private int balloonLetGoChance = 500;
    public int BalloonLetGoChance {
        get { return balloonLetGoChance; }
    }

    [Range(0, 50), SerializeField]
    private int talkChance = 5;
    public int TalkChance {
        get { return talkChance; }
    }

    [SerializeField]
    private bool randomizedPersonTimeOffset = true;
    public bool RandomizedPersonTimeOffset {
        get { return randomizedPersonTimeOffset; }
    }

    public float RandomPersonTimeOffset {
        get { return Random.Range(0.0f, 3.0f); }
    }

    [MinMaxRange(0.0f, 10.0f), SerializeField]
    private MinMaxRange distractionCheckFrequency = new MinMaxRange(0.25f, 1.5f);
    public float DistractionCheckFrequency {
        get { return distractionCheckFrequency.GetRandomValue(); }
    }

    [MinMaxRange(0.0f, 10.0f), SerializeField]
    private MinMaxRange distractionDuration = new MinMaxRange(1.5f, 3.0f);
    public float DistractionDuration {
        get { return distractionDuration.GetRandomValue(); }
    }

    [MinMaxRange(0.0f, 10.0f), SerializeField]
    private MinMaxRange behaviourChangeInterval = new MinMaxRange(1.0f, 5.0f);
    public float BehaviourChangeInterval {
        get { return behaviourChangeInterval.GetRandomValue(); }
    }

    // BUILDINGS //
    [Header("Building Configuration")]

    [SerializeField]
    private bool makeFancyBuildings = false;
    public bool MakeFancyBuildings {
        get { return makeFancyBuildings; }
    }

    [MinMaxRange(1, 10), SerializeField]
    private MinMaxRange numberOfBuildingStoreys = new MinMaxRange(1, 3);
    public MinMaxRange NumberOfBuildingStoreys {
        get { return numberOfBuildingStoreys; }
    }


    [Header("Debug")]
    [SerializeField, Tooltip("If true, applicable debug gizmos will be drawn. Has no effect for production builds.")]
    protected bool drawDebugGizmos = false;

    // Internal Benchmarking 
    private bool runningCappedTest = false;
    private float testElapsedTime = 0.0f;
    private int testElapsedFrames = 0;
    private int testFramesAtStartOfTest = 0;

    void Awake()
    {

        if (_instance != null)
        {
            Debug.Log("CityStreamManager:: Duplicate instance of CityStreamManager, deleting second instance.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    void Update ()
    {

        if (started)
        {

            bufferPosition = gameObject.transform.position.z + bufferOffset;

            if (bufferPosition >= nextBufferTarget)
            {

                nextBufferTarget += ParadeConstants.CityBlockSize;
                currentCityBlock += 1;
                generateNewCityBlock(currentCityBlock);

            }

            if (runningCappedTest)
            {

                testElapsedTime += Time.deltaTime;

                if(currentCityBlock >= testBlocksToTravel)
                {
                    testElapsedFrames = Time.frameCount - testFramesAtStartOfTest;
                    endTest(testElapsedTime, testElapsedFrames, false);
                }

            }

        }

        #region DEBUG_KEYS

        if (runningCappedTest == false)
        {

            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                cityMovementSpeed = Mathf.Min(maxSpeed, cityMovementSpeed + speedChangeStep);
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                cityMovementSpeed = Mathf.Max(0.0f, cityMovementSpeed - speedChangeStep);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                beginTest();
                started = true;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                Camera.main.gameObject.GetComponent<MouseLook>().MouseLookEnabled = !Camera.main.gameObject.GetComponent<MouseLook>().MouseLookEnabled;
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            endTest(0.0f, 0, true);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            demoSanityCheck();
        }

        #endregion

    }

    /// <summary>
    /// Generates a single city block of content at the specified index, according to the parameters of the test's Parade Configuration
    /// </summary>
    /// <param name="index">Index at which to generate the new block</param>
    private void generateNewCityBlock(int index)
    {

        Vector3 newCityBlockOrigin = new Vector3(0.0f, 0.0f, index * ParadeConstants.CityBlockSize);

        // Road Segments
        if (generateRoads)
        {
            GameObject roadSegment = Instantiate(Resources.Load(ParadeConstants.CityObjectsPath + "BasicRoadSegment", typeof(GameObject)), CityObjectContainer.Instance.gameObject.transform) as GameObject;
            roadSegment.transform.position = newCityBlockOrigin;
        }

        // Buildings: One for each side of the street
        if (generateBuildings)
        {

            Vector3 buildingOffset = new Vector3(16.0f, 0.0f, 0.0f);

            // Left side
            GameObject testBuilding = Instantiate(Resources.Load(ParadeConstants.CityObjectsPath + "BasicBuilding", typeof(GameObject)), CityObjectContainer.Instance.gameObject.transform) as GameObject;
            testBuilding.transform.position = newCityBlockOrigin - buildingOffset;
            testBuilding.GetComponent<CityBuilding>().LeftSide = true;
            testBuilding.GetComponent<CityBuilding>().FancyBuilding = MakeFancyBuildings;
            testBuilding.GetComponent<CityBuilding>().StoreyCount = NumberOfBuildingStoreys.GetRandomValueAsInt();
            testBuilding.GetComponent<CityBuilding>().constructBuilding();

            // Right Side
            GameObject testBuilding2 = Instantiate(Resources.Load(ParadeConstants.CityObjectsPath + "BasicBuilding", typeof(GameObject)), CityObjectContainer.Instance.gameObject.transform) as GameObject;
            testBuilding2.transform.position = newCityBlockOrigin + buildingOffset;
            testBuilding2.GetComponent<CityBuilding>().FancyBuilding = MakeFancyBuildings;
            testBuilding2.GetComponent<CityBuilding>().StoreyCount = NumberOfBuildingStoreys.GetRandomValueAsInt();
            testBuilding2.GetComponent<CityBuilding>().constructBuilding();

        }

        // Crowds of people: One for each side of the street
        if (generateCrowds)
        {

            Vector3 buildingOffset = new Vector3(8.0f, 0.0f, 0.0f);

            // Left side
            GameObject leftCrowd = Instantiate(Resources.Load(ParadeConstants.CityObjectsPath + "Crowd", typeof(GameObject)), CityObjectContainer.Instance.gameObject.transform) as GameObject;
            leftCrowd.transform.position = newCityBlockOrigin - buildingOffset;
            leftCrowd.GetComponent<Crowd>().LeftSide = true;
            leftCrowd.GetComponent<Crowd>().CreateCrowd();

            // Right Side
            GameObject rightCrowd = Instantiate(Resources.Load(ParadeConstants.CityObjectsPath + "Crowd", typeof(GameObject)), CityObjectContainer.Instance.gameObject.transform) as GameObject;
            rightCrowd.transform.position = newCityBlockOrigin + buildingOffset;
            rightCrowd.GetComponent<Crowd>().CreateCrowd();

        }

    }

    public void beginTest()
    {

        loadConfigurationPreset();

        bufferOffset = ParadeConstants.CityBlockSize * bufferLeadDistanceInBlocks;
        bufferPosition = gameObject.transform.position.z + bufferOffset;
        nextBufferTarget = bufferPosition + ParadeConstants.CityBlockSize;

        // Since we'll always be behind the buffer when we start, catch up and generate blocks between start position and buffer lead position.
        if (currentCityBlock < bufferLeadDistanceInBlocks)
        {
            for (int i = 0; i < (bufferLeadDistanceInBlocks + 1); i++)
            {
                generateNewCityBlock(i);
            }
        }

        currentCityBlock = bufferLeadDistanceInBlocks;

        if (runningCappedTest)
        {

            started = true;
            testElapsedTime = 0.0f;
            testElapsedFrames = 0;
            testFramesAtStartOfTest = Time.frameCount;

        }

    }

    private void endTest(float elapsedTime, int frameCount, bool aborted)
    {

        started = false;
        cityMovementSpeed = 0.0f;

        // We do best effort here to allow for tests to be run directly in the test scene, rather than forcing main menu use
        if (TestManager.Instance != null)
        {
            TestManager.Instance.giveTestResults(elapsedTime, frameCount, aborted);
        }

    }

    /// <summary>
    /// Loads in the selected Parade Configuration preset
    /// </summary>
    private void loadConfigurationPreset()
    {

        if(configurationPreset != null)
        {

            currentConfigName = configurationPreset.ConfigName;
            Camera.main.gameObject.GetComponent<MouseLook>().MouseLookEnabled = configurationPreset.EnableMouseLook;
            enableTestUI = configurationPreset.EnableTestUI;
            testBlocksToTravel = configurationPreset.NumberOfTestBlocks;
            bufferLeadDistanceInBlocks = configurationPreset.LeadingBlocks;
            bufferCleanupDistance = configurationPreset.RearCleanupDistance;
            testCameraSpeed = configurationPreset.CameraSpeed;

            generateRoads = configurationPreset.GenerateRoads;
            generateBuildings = configurationPreset.GenerateBuildings;
            generateCrowds = configurationPreset.GenerateCrowds;

            crowdDensityFactor = configurationPreset.CrowdDensity;
            balloonChance = configurationPreset.BalloonChance;
            balloonLetGoChance = configurationPreset.BalloonLetGoChance;
            randomizedPersonTimeOffset = configurationPreset.RandomizedTimeOffset;
            distractionCheckFrequency = configurationPreset.DistractionCheckFrequency;
            distractionDuration = configurationPreset.DistractionDuration;
            behaviourChangeInterval = configurationPreset.BehaviourChangeInterval;

            makeFancyBuildings = configurationPreset.MakeFancyBuildings;
            numberOfBuildingStoreys = configurationPreset.NumberOfBuildingStoreys;

        }
        else
        {
            currentConfigName = "[Test Configuration]";
        }

        runningCappedTest = (testBlocksToTravel == 0) ? false : true;
        cityMovementSpeed = testCameraSpeed;

    }

    /// <summary>
    /// Just a simple way to sanity check no unwanted physics or animation stuff made it into the scene or its resources
    /// </summary>
    private void demoSanityCheck()
    {

        Collider[] allColliders = GameObject.FindObjectsOfType<Collider>();
        Collider2D[] allCollider2Ds = GameObject.FindObjectsOfType<Collider2D>();
        Rigidbody[] rigidBodies = GameObject.FindObjectsOfType<Rigidbody>();
        Animator[] allAnimators = GameObject.FindObjectsOfType<Animator>();
        Animation[] allAnimations = GameObject.FindObjectsOfType<Animation>();

        int unwantedSum = allColliders.Length + allCollider2Ds.Length + rigidBodies.Length + allAnimators.Length + allAnimations.Length;

        if(unwantedSum == 0)
        {
            Debug.Log("<color=Green>Found ZERO unwanted components in scene: PASS.</color>");
        }
        else
        {
            Debug.Log("<color=Red>Found "+unwantedSum+" unwanted components in scene: FAIL.</color>");
        }

    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void OnDrawGizmos()
    {

        if (drawDebugGizmos)
        {

            Color c = Color.cyan;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(10.0f, 10.0f, 10.0f));

            c = Color.magenta;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f, bufferPosition), new Vector3(10.0f, 10.0f, 10.0f));

            if (started)
            {
                c = Color.red;
                c.a = 0.5f;
                Gizmos.color = c;

                Gizmos.DrawCube(gameObject.transform.position - new Vector3(0.0f, 0.0f, bufferCleanupDistance), new Vector3(50.0f, 1.0f, 10.0f));
            }

        }

    }

#endif

}
