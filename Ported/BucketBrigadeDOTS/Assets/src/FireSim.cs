using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSim : MonoBehaviour
{
    public static FireSim INSTANCE;

    [Header("WATER")]
    [Range(1,5)]
    [Tooltip("Number of cells affected by a bucket of water")]
    public int splashRadius = 3;
    [Tooltip("Water bucket reduces fire temperature by this amount")]
    public float coolingStrength = 1f;
    [Tooltip("Splash damage of water bucket. (1 = no loss of power over distance)")]
    public float coolingStrength_falloff = 0.75f;
    [Tooltip("Water sources will refill by this amount per second")]
    public float refillRate = 0.0001f;
    [Range(0, 100)]
    public int totalBuckets = 3;
    [Tooltip("How much water does a bucket hold?")]
    public float bucketCapacity = 3f;
    [Tooltip("Buckets fill up by this much per second")]
    public float bucketFillRate = 0.01f;
    [Tooltip("Visual scale of bucket when EMPTY (no effect on water capacity)")]
    public float bucketSize_EMPTY= 0.2f;
    [Tooltip("Visual scale of bucket when FULL (no effect on water capacity)")]
    public float bucketSize_FULL= 0.4f;

    [Header("FIRE")]
    [Tooltip("Prefabs / FlameCell")]
    public GameObject prefab_flameCell;
    [Tooltip("How many random fires do you want to battle?")]
    public int startingFireCount = 1;
    [Tooltip("How high the flames reach at max temperature")]
    public float maxFlameHeight = 0.1f;
    [Tooltip("Size of an individual flame. Full grid will be (rows * cellSize)")]
    public float cellSize = 0.05f;
    [Tooltip("How many cells WIDE the simulation will be")]
    public int rows = 20;
    [Tooltip("How many cells DEEP the simulation will be")]
    public int columns = 20;
    private float simulation_WIDTH, simulation_DEPTH;
    [Tooltip("When temperature reaches *flashpoint* the cell is on fire")]
    public float flashpoint = 0.5f;
    [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usafge")]
    public int heatRadius = 1;
    [Tooltip("How fast will adjascent cells heat up?")]
    public float heatTransferRate = 0.7f;

    [Range(0.0001f,2f)]
    [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
    public float fireSimUpdateRate = 0.5f;

    [Header("Colours")]
    // cell colours
    public Color colour_fireCell_neutral;
    public Color colour_fireCell_cool;
    public Color colour_fireCell_hot;
    // bot colours
    public Color colour_bot_SCOOP;
    public Color colour_bot_PASS_FULL;
    public Color colour_bot_PASS_EMPTY;
    public Color colour_bot_THROW;
    public Color colour_bot_OMNIBOT;
    // bucket Colours
    public Color colour_bucket_empty;
    public Color colour_bucket_full;
    [HideInInspector]
    public  FlameCell[] flameCells;
    [HideInInspector]
    public int cellsOnFire;
    [HideInInspector]
    public int totalCells;
    private float timeUntilFireUpdate;

    [Header("BOTS")]
    public GameObject prefab_bucket;
    public GameObject prefab_bot;
    [HideInInspector]
    public List<BucketChain> bucketChains;
    public List<BucketChain_Config> bucketChain_Configs;
    [Range(0.0001f, 1f)]
    public float botSpeed = 0.1f;
    [Range(0.001f, 1f)]
    public float waterCarryAffect = 0.5f;
    public int total_OMNIBOTS = 0;
    private int botCount = 0;
    private List<Bot> allBots;
    [HideInInspector]
    public List<Bucket> allBuckets;
    [HideInInspector]
    public List<Water> allWater;
   
	void Start()
    {
        INSTANCE = this;

        Setup_Water();
        Setup_Fire();
        Setup_Buckets();
        Setup_OMNI_Bots();
        Setup_BucketChains();
    }
    #region ----------------------------------------> WATER
    void Setup_Water(){
        allWater = new List<Water>(FindObjectsOfType<Water>());
    }
    #endregion ---------------------------------> WATER

    #region ----------------------------------------> FIRE
    void Setup_Fire()
    {
        totalCells = rows * columns;
        simulation_WIDTH = rows * cellSize;
        simulation_DEPTH = cellSize * columns;
        timeUntilFireUpdate = fireSimUpdateRate;
        PopulateFireGrid();
        cellsOnFire = 0;

        // start random fire(s)
        for (int i = 0; i < startingFireCount; i++)
        {
            flameCells[Mathf.FloorToInt(Random.Range(0, totalCells))].Scorch();
        }
    }

    void PopulateFireGrid()
    {
        flameCells = new FlameCell[totalCells];
        int cellIndex = 0;
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                // add flame cell prefab
                GameObject _tempFlame = (GameObject)Instantiate(prefab_flameCell, transform.position, transform.rotation);
                flameCells[cellIndex] = _tempFlame.GetComponent<FlameCell>();
                flameCells[cellIndex].Init(transform, cellIndex, rowIndex, columnIndex, cellSize, maxFlameHeight, colour_fireCell_neutral);
                cellIndex++;
            }
        }
    }
    void Update()
    {
        UpdateFire();
        Update_Bots();
        Update_BucketChains();

    }
    void UpdateFire()
    {
        timeUntilFireUpdate -= Time.deltaTime;
        if (timeUntilFireUpdate <= 0)
        {
            timeUntilFireUpdate = fireSimUpdateRate;
            for (int cellIndex = 0; cellIndex < totalCells; cellIndex++)
            {
                float tempChange = 0f;
                FlameCell currentCell = flameCells[cellIndex];
                currentCell.neighbourOnFire = false;
                int cellRowIndex = Mathf.FloorToInt(cellIndex / columns);
                int cellColumnIndex = cellIndex % columns;

                for (int rowIndex = -heatRadius; rowIndex <= heatRadius; rowIndex++)
                {
                    int currentRow = cellRowIndex - rowIndex;
                    if (currentRow >= 0 && currentRow < rows)
                    {
                        for (int columnIndex = -heatRadius; columnIndex <= heatRadius; columnIndex++)
                        {
                            int currentColumn = cellColumnIndex + columnIndex;
                            if (currentColumn >= 0 && currentColumn < columns)
                            {
                                FlameCell _neighbour = flameCells[(currentRow * columns) + currentColumn];
                                if (_neighbour.onFire)
                                {
                                    currentCell.neighbourOnFire = true;
                                    tempChange += _neighbour.temperature * heatTransferRate;
                                }
                            }
                        }
                    }
                }
                UpdateCell(cellIndex, tempChange);
            }
        }
    }

    void UpdateCell(int _targetCell, float _tempChange)
    {
        FlameCell _FC = flameCells[_targetCell];
        _FC.temperature = Mathf.Clamp(_FC.temperature + _tempChange, -1f,1f);
        _FC.IgnitionTest();
        
    }

    public void DowseFlameCell(int _cellIndex){
        int targetROW = Mathf.FloorToInt(_cellIndex / columns);
        int targetCOLUMN = _cellIndex % columns;
        flameCells[_cellIndex].Extinguish(coolingStrength);
        for (int rowIndex = -splashRadius; rowIndex <= splashRadius; rowIndex++)
        {
            int currentRow = targetROW - rowIndex;
            if (currentRow >= 0 && currentRow < rows)
            {
                for (int columnIndex = -splashRadius; columnIndex <= splashRadius; columnIndex++)
                {
                    int currentColumn = targetCOLUMN + columnIndex;
                    if (currentColumn >= 0 && currentColumn < columns)
                    {
                        float dowseCellStrength = 1f / (Mathf.Abs(rowIndex*coolingStrength_falloff) + Mathf.Abs(columnIndex* coolingStrength_falloff));
                        flameCells[(currentRow * columns) + currentColumn].Extinguish((coolingStrength * dowseCellStrength) * bucketCapacity);
                    }
                }
            }
        }
    }
    #endregion ---------------------------------> FIRE

    #region ----------------------------------------> BUCKETS
    void Setup_Buckets()
    {
        allBuckets = new List<Bucket>();
        for (int i = 0; i < totalBuckets; i++)
        {
            GameObject _bucketObj = (GameObject)Instantiate(prefab_bucket, transform.position, transform.rotation);
            Bucket _tempBucket = _bucketObj.GetComponent<Bucket>();
            // initialise bot to desired type
            _tempBucket.Init(allBuckets.Count, Random.Range(0f, simulation_WIDTH), Random.Range(0f, simulation_DEPTH));
            // Add bot to desired list (and master list)
            allBuckets.Add(_tempBucket);
        }
    }
    public bool BucketAvailable(bool _want_FULL_bucket)
    {
        bool result = false;
        for (int i = 0; i < allBuckets.Count; i++)
        {
            Bucket _b = allBuckets[i];
            if(!_b.bucketActive && _b.bucketFull == _want_FULL_bucket){
                result = true;
                break;
            }
        }
        return result;
    }
    #endregion ---------------------------------> BUCKETS

    #region ----------------------------------------> BOT
    void Setup_OMNI_Bots()
    {
        allBots = new List<Bot>();
     
        // Add OMNIBOTS
        for (int i = 0; i < total_OMNIBOTS; i++)
        {
            AddBot(BotType.OMNIBOT);
        }
    }
    public Bot AddBot(BotType _botType, BucketChain _parentChain = null)
    {
        // Add bot prefab to scene
            GameObject _botPrefab = (GameObject)Instantiate(prefab_bot, transform.position, transform.rotation);
            Bot _tempBot = _botPrefab.GetComponent<Bot>();
            // initialise bot to desired type
        _tempBot.Init(_botType, allBots.Count, Random.Range(0f, simulation_WIDTH), Random.Range(0f, simulation_DEPTH));

        // if a parent chain was specified, hook it up
        if(_parentChain!=null){
            _tempBot.parentChain = _parentChain;
            _tempBot.isPartOfChain = true;
        }
            // Add bot to desired list (and master list)
            allBots.Add(_tempBot);
        botCount++;
        return _tempBot;
    }
    public static Color GetBotTypeColour(BotType _botType)
    {
        Color result = INSTANCE.colour_bot_OMNIBOT;
        switch (_botType)
        {
            case BotType.SCOOP:
                result = INSTANCE.colour_bot_SCOOP;
                break;
            case BotType.PASS_FULL:
                result = INSTANCE.colour_bot_PASS_FULL;
                break;
            case BotType.PASS_EMPTY:
                result = INSTANCE.colour_bot_PASS_EMPTY;
                break;
            case BotType.THROW:
                result = INSTANCE.colour_bot_THROW;
                break;
        }
        return result;
    }
    void Update_Bots()
    {
        for (int i = 0; i < botCount; i++)
        {
            allBots[i].UpdateBot();
        }
    }
    #endregion ---------------------------------> BOT

    #region ----------------------------------------> BUCKET CHAINS
    public void AddBucketChain_CONFIG(int _passers_EMPTY, int _passers_FULL){
        bucketChain_Configs.Add(new BucketChain_Config(_passers_EMPTY, _passers_FULL));
    }
    void Setup_BucketChains(){
        // temp hard code a single bucket chain
        bucketChains = new List<BucketChain>();
        for (int i = 0; i < bucketChain_Configs.Count; i++)
        {
            BucketChain_Config _Config = bucketChain_Configs[i];
            bucketChains.Add(new BucketChain(_Config.passers_EMPTY, _Config.passers_FULL, bucketChains.Count));
        }
    }
    void Update_BucketChains(){
        for (int i = 0; i < bucketChains.Count; i++)
        {
            bucketChains[i].UpdateChain();
        }
    }
    #endregion ---------------------------------> BUCKET CHAINS
}
