using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum BotType
{
    SCOOP,
    PASS_FULL,
    PASS_EMPTY,
    THROW,
    OMNIBOT
}
public enum BotAction
{
    GET_BUCKET,
    DROP_BUCKET,
    FILL_BUCKET,
    THROW_BUCKET,
    GOTO_PICKUP_LOCATION,
    GOTO_DROPOFF_LOCATION,
    GOTO_FIRE,
    GOTO_WATER,
    PASS_BUCKET
}
public struct BotCommand
{
    public BotAction action;
    public bool appendWhenComplete;

    public BotCommand(BotAction _action, bool _appendWhenComplete)
    {
        action = _action;
        appendWhenComplete = _appendWhenComplete;
    }
}

public class Bot : MonoBehaviour
{

    public BotType botType;
    public Bucket carrying;
    public Bucket targetBucket;
    public Water targetWater;
    public FlameCell targetFlameCell;
    public Vector3 location_PICKUP;
    public Vector3 location_DROPOFF;
    public bool isFillingBucket;
    public float decisionRate = 1f;
    private float decisionTimer;
    public int botID;
    public Bot bucketProvider;
    public bool isPartOfChain;
    public BucketChain parentChain;

    private Transform t;
    private Material mat;
    private FireSim fireSim;
    private float arriveThreshold;

    private BotCommand currentCommand;
    private Queue<BotCommand> commandQueue;

    public void Init(BotType _botType, int _botID, float _x, float _y)
    {

        t = transform;
        mat = GetComponent<Renderer>().material;
        fireSim = FireSim.INSTANCE;
        arriveThreshold = t.localScale.x * 1f;
        t.position = new Vector3(_x, t.localScale.y * 0.5f, _y);
        decisionTimer = decisionRate;

        botID = _botID;
        carrying = null;
        targetBucket = null;
        isFillingBucket = false;
        Setup_BotType(_botType);
        name = _botType.ToString();
    }
    #region ----------------------------------------> SETUP
    void Setup_BotType(BotType _botType)
    {

        botType = _botType;
        mat.color = FireSim.GetBotTypeColour(botType);
        commandQueue = new Queue<BotCommand>();
        // Create command lists for each bot type
        switch (botType)
        {
            default:
                break;
            case BotType.SCOOP:
                // pickup empty bucket
                commandQueue.Enqueue(new BotCommand(BotAction.GET_BUCKET, true));
                // goto filling location
                commandQueue.Enqueue(new BotCommand(BotAction.GOTO_PICKUP_LOCATION, true));
                // fill bucket
                commandQueue.Enqueue(new BotCommand(BotAction.FILL_BUCKET, true));
                // goto bucket store location
                commandQueue.Enqueue(new BotCommand(BotAction.GOTO_DROPOFF_LOCATION, true));
                // put full bucket down in direction of fire
                commandQueue.Enqueue(new BotCommand(BotAction.DROP_BUCKET, true));
                break;
            case BotType.PASS_FULL:
                // pickup nearest inactive bucket to station
                commandQueue.Enqueue(new BotCommand(BotAction.GET_BUCKET, true));
                // goto next station in chain and DROP bucket
                commandQueue.Enqueue(new BotCommand(BotAction.PASS_BUCKET, true));
                break;
            case BotType.PASS_EMPTY:
                // pickup nearest inactive bucket to station
                commandQueue.Enqueue(new BotCommand(BotAction.GET_BUCKET, true));
                // goto next station in chain and DROP bucket
                commandQueue.Enqueue(new BotCommand(BotAction.PASS_BUCKET, true));
                break;
            case BotType.THROW:
                // look for nearest WAITING full bucket closest to your station
                commandQueue.Enqueue(new BotCommand(BotAction.GET_BUCKET, true));
                // goto fire location
                commandQueue.Enqueue(new BotCommand(BotAction.GOTO_DROPOFF_LOCATION, true));
                // throw water
                commandQueue.Enqueue(new BotCommand(BotAction.THROW_BUCKET, true));
                // go to end of chain and drop off empty bucket location
                commandQueue.Enqueue(new BotCommand(BotAction.GOTO_PICKUP_LOCATION, true));
                // go back to station, put empty bucket down
                commandQueue.Enqueue(new BotCommand(BotAction.DROP_BUCKET, true));
                break;
            case BotType.OMNIBOT:
                // pick up empty bucket
                commandQueue.Enqueue(new BotCommand(BotAction.GET_BUCKET, false));
                // Choose fill point
                commandQueue.Enqueue(new BotCommand(BotAction.GOTO_WATER, true));
                // fill bucket
                commandQueue.Enqueue(new BotCommand(BotAction.FILL_BUCKET, true));
                // walk to nearest fire point
                commandQueue.Enqueue(new BotCommand(BotAction.GOTO_FIRE, true));
                // throw water
                commandQueue.Enqueue(new BotCommand(BotAction.THROW_BUCKET, true));
                // repeat from command two
                break;
        }
        BeginCommand(commandQueue.Peek());
    }
    #endregion ---------------------------------> SETUP

    #region ----------------------------------------> BEGIN_COMMAND
    // Kick off the command - choose targets, set required vars etc
    private void BeginCommand(BotCommand _command)
    {
        currentCommand = _command;
        switch (currentCommand.action)
        {
            case BotAction.DROP_BUCKET:
                DropBucket();
                CompleteCommand();
                break;
            case BotAction.FILL_BUCKET:
                isFillingBucket = true;
                break;
            case BotAction.THROW_BUCKET:
                ThrowBucket();
                CompleteCommand();
                break;
            case BotAction.GET_BUCKET:
                if(botType == BotType.SCOOP || botType == BotType.OMNIBOT){
                    targetBucket = FindNearestBucket(false);
                }else{
                    if(bucketProvider!=null && bucketProvider.carrying !=null){
                        targetBucket = bucketProvider.carrying;    
                    }
                }
                break;
            case BotAction.GOTO_WATER:
                targetWater = FindNearestWater();
                break;
            case BotAction.GOTO_FIRE:
                targetFlameCell = FindNearestFlame();
                break;
        }
    }
    #endregion ---------------------------------> BEGIN_COMMAND


    #region ----------------------------------------> UPDATE COMMAND
    public void UpdateBot()
    {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer < 0)
        {
            decisionTimer = decisionRate;
            RestartCommand();
        }
        switch (currentCommand.action)
        {
            case BotAction.PASS_BUCKET:
                if (MoveTowards(location_DROPOFF))
                {
                    DropBucket();
                    CompleteCommand();
                }
                break;
            case BotAction.FILL_BUCKET:
                carrying.volume = Mathf.Clamp(carrying.volume + fireSim.bucketFillRate, 0f, fireSim.bucketCapacity);
                targetWater.Subtract(fireSim.bucketFillRate);
                carrying.bucketFull = (carrying.volume >= fireSim.bucketCapacity);

                // Update bucket Scale (reparenting trick to preserve scale)
                carrying.transform.SetParent(fireSim.transform, true);
                carrying.UpdateBucket();
                carrying.transform.SetParent(t, true);

                if (carrying.bucketFull)
                {
                    CompleteCommand();
                }
                break;
            case BotAction.GET_BUCKET:
                if (targetBucket != null)
                {
                    if (!targetBucket.bucketActive){
                        if(MoveTowards(targetBucket.transform.position)){
                            PickupBucket(targetBucket);
                            CompleteCommand();
                        }
                    }
                }else{
                    if (botType == BotType.PASS_EMPTY || botType == BotType.PASS_FULL || botType == BotType.THROW)
                        {
                            MoveTowards(location_PICKUP);

                        }
                            RestartCommand();
                }
                break;
            case BotAction.GOTO_PICKUP_LOCATION:
                if (MoveTowards(location_PICKUP))
                {
                    CompleteCommand();
                }
                break;
            case BotAction.GOTO_DROPOFF_LOCATION:
                if (MoveTowards(location_DROPOFF))
                {
                    CompleteCommand();
                }
                break;
            case BotAction.GOTO_WATER:
                if (targetWater != null)
                {
                    if (targetWater.volume <= 0)
                    {
                        RestartCommand();
                    }
                    if (MoveTowards(targetWater.transform.position))
                    {
                        CompleteCommand();
                    }
                }
                break;
            case BotAction.GOTO_FIRE:
                if (fireSim.cellsOnFire > 0)
                {
                    if (targetFlameCell != null)
                    {

                        if (!targetFlameCell.onFire)
                        {
                            RestartCommand();
                        }
                        if (MoveTowards(targetFlameCell.transform.position))
                        {
                            CompleteCommand();
                        }
                    }
                    else
                    {
                        RestartCommand();
                    }
                }else{
                    Debug.Log("NO FIRES LEFT");
                    targetFlameCell = null;
                }

                break;

        }
    }
    #endregion ---------------------------------> UPDATE COMMAND


    private void RestartCommand()
    {
        BeginCommand(currentCommand);
    }
    private void CompleteCommand()
    {
        BotCommand _tempCommand = commandQueue.Dequeue();
        if (_tempCommand.appendWhenComplete)
        {
            commandQueue.Enqueue(_tempCommand);
        }
        BeginCommand(commandQueue.Peek());
    }
    void PickupBucket(Bucket _b)
    {
        carrying = _b;
        _b.bucketActive = true;
        _b.transform.SetParent(t, true);
        _b.transform.localPosition = new Vector3(0f, t.transform.localScale.y + _b.transform.localScale.y * 0.5f, 0f);
    }
    void DropBucket()
    {
        if (carrying != null)
        {
            carrying.transform.localPosition = new Vector3(0f, 0f, 0f);
            carrying.transform.SetParent(fireSim.transform, true);
            carrying.bucketActive = false;
            targetBucket = null;
            carrying = null;
        }
    }
    void ThrowBucket()
    {
        if(targetFlameCell != null){
        fireSim.DowseFlameCell(targetFlameCell.index);
        carrying.bucketFull = false;
        carrying.volume = 0;

            // Update bucket Scale (reparenting trick to preserve scale)
            carrying.transform.SetParent(fireSim.transform, true);
            carrying.UpdateBucket();
            carrying.transform.SetParent(t, true);
        }
    }
    #region ----------------------------------------> FIND NEAREST X
    public Bucket FindNearestBucket(bool _want_FULL_Bucket)
    {
        Bucket _bucketChoice = null;
            List<Bucket> bucketList = fireSim.allBuckets;
            float distance = 999f;
            Vector3 targetLocation = (botType == BotType.OMNIBOT) ? t.position : location_PICKUP;
            foreach (Bucket _b in bucketList)
            {
                if (!_b.bucketActive && _b.bucketFull == _want_FULL_Bucket)
                {
                    float testDistance = Vector3.Distance(targetLocation, _b.transform.position);
                    if (testDistance < distance)
                    {
                        _bucketChoice = _b;
                        distance = testDistance;
                    }
                }

            }

        return _bucketChoice;
    }

    public Water FindNearestWater()
    {
        Water nearestWater = null;
        List<Water> waterList = fireSim.allWater;
        float distance = 999f;
        foreach (Water _w in waterList)
        {
            if (_w.volume > 0)
            {
                float testDistance = Vector3.Distance(t.position, _w.transform.position);
                if (testDistance < distance)
                {
                    nearestWater = _w;
                    distance = testDistance;
                }
            }
        }
        return nearestWater;
    }
    public FlameCell FindNearestFlame()
    {
        FlameCell nearestFlame = null;
        Vector3 targetLocation = (botType == BotType.OMNIBOT) ? t.position : location_PICKUP;
        float distance = 999f;
        for (int i = 0; i < fireSim.totalCells; i++)
        {
            FlameCell _FC = fireSim.flameCells[i];
            if (_FC.onFire)
            {
                float testDistance = Vector3.Distance(targetLocation, _FC.transform.position);
                if (testDistance < distance)
                {
                    nearestFlame = _FC;
                    distance = testDistance;
                }
            }
        }
        return nearestFlame;
    }
    #endregion ---------------------------------> FIND NEAREST X

    private bool MoveTowards(Vector3 _dest)
    {
        Vector3 _POS = t.position;
        bool arrivedX = false;
        bool arrivedZ = false;
        float movementSpeed = fireSim.botSpeed;
        if (carrying != null)
        {
            if (carrying.bucketFull)
            {
                movementSpeed *= fireSim.waterCarryAffect;
            }
        }
        // X POSITION
        if (_POS.x < _dest.x - arriveThreshold)
        {
            _POS.x += movementSpeed;
        }
        else if (_POS.x > _dest.x + arriveThreshold)
        {
            _POS.x -= movementSpeed;
        }
        else
        {
            arrivedX = true;
        }

        // Z POSITION
        if (_POS.z < _dest.z - arriveThreshold)
        {
            _POS.z += movementSpeed;
        }
        else if (_POS.z > _dest.z + arriveThreshold)
        {
            _POS.z -= movementSpeed;
        }
        else
        {
            arrivedZ = true;
        }

        if (arrivedX && arrivedZ)
        {
            return true;
        }
        else
        {
            t.position = _POS;
            return false;
        }
    }

	#region ----------------------------------------> GIZMOS - delete
	private void OnDrawGizmos()
	{
        BotAction currentAction = currentCommand.action;

        if (location_PICKUP != Vector3.zero && currentAction == BotAction.GET_BUCKET)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(t.position, location_PICKUP);
        }
        if (location_DROPOFF != Vector3.zero && currentAction == BotAction.GET_BUCKET)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(t.position, location_DROPOFF);
        }

        if (targetWater != null)
        {
            if (currentAction == BotAction.GOTO_DROPOFF_LOCATION || currentAction == BotAction.GET_BUCKET || currentAction == BotAction.GOTO_WATER)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(t.position, targetWater.transform.position);
            }
        }
        if (targetBucket != null)
        {
            if (currentAction == BotAction.GET_BUCKET)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(t.position, targetBucket.transform.position);
            }
        }
        if (targetFlameCell != null)
        {
            if (currentAction == BotAction.GOTO_DROPOFF_LOCATION || currentAction == BotAction.GET_BUCKET || currentAction == BotAction.GOTO_FIRE)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(t.position, targetFlameCell.transform.position);
            }
        }   


	}
	#endregion ---------------------------------> GIZMOS - delete
}
