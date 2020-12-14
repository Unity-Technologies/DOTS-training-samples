using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketChain {

    public int chainID;
    public Bot scooper;
    public Bot thrower;
    public int total_passers_EMPTY = 3;
    public int total_passers_FULL = 3;
    public List<Bot> chain_EMPTY;
    public List<Bot> chain_FULL;

    private Water targetWater;
    private FlameCell targetFlame;
    public Vector3 chain_START, chain_END;

    // assessment timer
    public float rate_assessChain = 2f;
    private float timer_assessChain;

    public BucketChain(int _passers_EMPTY, int _passers_FULL, int _chainID)
    {
        chainID = _chainID;
        // make a scooper
        scooper = FireSim.INSTANCE.AddBot(BotType.SCOOP);
        // make a thrower
        thrower = FireSim.INSTANCE.AddBot(BotType.THROW);
        // make (n) empty passers
        total_passers_EMPTY = _passers_EMPTY;
        chain_EMPTY = new List<Bot>();
        for (int i = 0; i < total_passers_EMPTY; i++)
        {
            Bot _tempBOT = FireSim.INSTANCE.AddBot(BotType.PASS_EMPTY);
            _tempBOT.botID = i;
            chain_EMPTY.Add(_tempBOT);

            // Setup Bucket Provider
            if(i==0){
                _tempBOT.bucketProvider = thrower;
            }else{
                _tempBOT.bucketProvider = chain_EMPTY[i - 1];   
            }
        }

        // make (n) full passers
        total_passers_FULL = _passers_FULL;
        chain_FULL = new List<Bot>();
        for (int i = 0; i < total_passers_FULL; i++)
        {
            Bot _tempBOT = FireSim.INSTANCE.AddBot(BotType.PASS_FULL);
            _tempBOT.botID = i;
            chain_FULL.Add(_tempBOT);

            // Setup Bucket Provider
            if (i == 0)
            {
                _tempBOT.bucketProvider = scooper;
            }
            else
            {
                _tempBOT.bucketProvider = chain_FULL[i - 1];
            }
        }
        // Perform first chain ASSESS (which will also start the assess timer)
        AssessChain();
    }
    public void UpdateChain()
    {
        timer_assessChain -= Time.deltaTime;
        if (timer_assessChain < 0)
        {
            AssessChain();
            timer_assessChain = rate_assessChain;
        }
    }
    void AssessChain()
    {
        // Set scooper's nearest water source
        if(scooper.targetWater==null || scooper.targetWater.volume<=0){
            scooper.targetWater = scooper.FindNearestWater();
            scooper.location_PICKUP = scooper.location_DROPOFF = scooper.targetWater.transform.position;
            chain_START = scooper.location_PICKUP;
        }

        // pick closest fire to SCOOPER
        thrower.targetFlameCell = scooper.FindNearestFlame();
        // only proceed if there's a fire to put out
        if (thrower.targetFlameCell != null)
        {
            thrower.location_PICKUP = thrower.location_DROPOFF = thrower.targetFlameCell.transform.position;
            chain_END = thrower.location_DROPOFF;

            // space out the PASSING chains
            // EMPTY
            for (int i = 0; i < total_passers_EMPTY; i++)
            {
                Bot _tempPasser_EMPTY = chain_EMPTY[i];
                _tempPasser_EMPTY.location_PICKUP = GetChainPosition(i, total_passers_EMPTY, chain_END, chain_START);
                _tempPasser_EMPTY.location_DROPOFF = GetChainPosition(i + 1, total_passers_EMPTY, chain_END, chain_START);
            }
            // FULL
            for (int i = 0; i < total_passers_FULL; i++)
            {
                Bot _tempPasser_FULL = chain_FULL[i];
                _tempPasser_FULL.location_PICKUP = GetChainPosition(i, total_passers_FULL, chain_START, chain_END);
                _tempPasser_FULL.location_DROPOFF = GetChainPosition(i + 1, total_passers_FULL, chain_START, chain_END);
            }
            thrower.bucketProvider = (chain_FULL.Count>0) ? chain_FULL[chain_FULL.Count - 1] : scooper;
        }
    }
    Vector3 GetChainPosition(int _index, int _chainLength, Vector3 _startPos, Vector3 _endPos){
        // adds two to pad between the SCOOPER AND THROWER
        float progress = (float) _index / _chainLength;
        float curveOffset = Mathf.Sin(progress * Mathf.PI) * 1f;

        // get Vec2 data
        Vector2 heading = new Vector2(_startPos.x, _startPos.z) -  new Vector2(_endPos.x, _endPos.y);
        float distance = heading.magnitude;
        Vector2 direction = heading / distance;
        Vector2 perpendicular = new Vector2(direction.y, -direction.x);

        //Debug.Log("chain progress: " + progress + ",  curveOffset: " + curveOffset);
        return Vector3.Lerp(_startPos, _endPos, (float)_index / (float)_chainLength) + (new Vector3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
    }
}

[System.Serializable]
public class BucketChain_Config {
    public int passers_EMPTY = 3;
    public int passers_FULL = 3;
    public BucketChain_Config(int _passers_EMPTY, int _passers_FULL){
        passers_EMPTY = _passers_EMPTY;
        passers_FULL = _passers_FULL;
    }
}

