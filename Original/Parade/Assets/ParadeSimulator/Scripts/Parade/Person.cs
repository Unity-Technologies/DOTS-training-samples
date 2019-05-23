using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

/// <summary>
/// Core logic for handling Person behaviours via state machine. Controls all Person logic during parade.
/// </summary>
public class Person : CityEntity, IEquatable<Person> {

    private Transform lookTarget = null;
    private Person chatPartner = null;

    private Transform myHead = null;
    public Transform Head {
        get { return myHead; }
    }

    private enum ePersonState
    {
        IDLE = 0,
        WATCH_PARADE = 1,
        TALK_TO_NEIGHBOUR = 2,
        DISTRACTED = 3
    }
    private ePersonState currentState = ePersonState.IDLE;

    private float nextBehaviourChangeCounter = 0.0f;

    private int myBlockIndex = 0;
    public int BlockIndex {
        get { return myBlockIndex; }
        set { myBlockIndex = value; }
    }

    private float distractionRadius = 10.0f;
    private float distractionCheckCounter = 0.0f;
    private Transform distractedLookTarget = null;

    private float randomDelayCountdown = 0.0f;
    public float RandomDelayCountdown {
        set { randomDelayCountdown = value; }
    }
    private bool personStarted = false;

    void Start () {

        myHead = gameObject.transform.Find("Head");
        if (!myHead)
        {
            Debug.LogError("Person:: Person '"+gameObject.name+"' has no Head child. Did someone break a prefab?");
        }

        gameObject.transform.localScale = new Vector3(Random.Range(0.9f,1.1f), Random.Range(0.8f, 1.25f), Random.Range(0.9f, 1.1f));
        changeToRandomBehaviour();
        distractionCheckCounter = CityStreamManager.Instance.DistractionCheckFrequency;

    }
	
	protected override void Update ()
    {

        base.Update();

        if (personStarted)
        {
            
            if (distractionCheckCounter > 0.0f)
            {
                distractionCheckCounter -= Time.deltaTime;
            }
            else
            {
                distractionCheckCounter = 0.0f;
                checkForDistractions();
            }
            
            if (nextBehaviourChangeCounter > 0.0f)
            {
                nextBehaviourChangeCounter -= Time.deltaTime;
            }
            else
            {
                nextBehaviourChangeCounter = 0.0f;
                changeToRandomBehaviour();
            }

            if (currentState != ePersonState.IDLE)
            {
                myHead.LookAt(lookTarget);
            }

        }
        else
        {

            randomDelayCountdown -= Time.deltaTime;
            if(randomDelayCountdown <= 0.0f)
            {
                personStarted = true;
            }

        }

    }

    /// <summary>
    /// Changes Person to do any random behaviour except DISTRACTED
    /// </summary>
    private void changeToRandomBehaviour()
    {

        int behaviourIndex = Random.Range(0, 3);
        changeToSpecificBehaviour(behaviourIndex);

    }

    /// <summary>
    /// Changes to specified behaviour, or a random one if no valid behaviour is specified
    /// </summary>
    /// <param name="behaviour">Valid ePersonState to switch to, or -1 to change to random behaviour.</param>
    private void changeToSpecificBehaviour(int behaviour)
    {

        chatPartner = null;
        nextBehaviourChangeCounter = CityStreamManager.Instance.BehaviourChangeInterval;

        switch (behaviour)
        {

            case (int)ePersonState.TALK_TO_NEIGHBOUR:

                Transform tempNeighbour = findNearestNeighbour(5.0f);

                if(tempNeighbour != null && tempNeighbour.gameObject.GetComponent<Person>().willStartConversation(this))
                {
                    startConversationWithNeighbour(tempNeighbour.gameObject.GetComponent<Person>());
                    currentState = ePersonState.TALK_TO_NEIGHBOUR;
                }
                else
                {
                    // Look at my shoes and be idle for a bit
                    myHead.transform.localRotation = Quaternion.Euler(new Vector3(30.0f, Random.Range(-5.0f, 5.0f), 0.0f));
                    currentState = ePersonState.IDLE;
                }
                
                break;

            case (int)ePersonState.DISTRACTED:

                lookTarget = distractedLookTarget;
                distractedLookTarget = null;
                nextBehaviourChangeCounter = CityStreamManager.Instance.DistractionDuration;
                endConversationWithNeighbour();
                currentState = ePersonState.DISTRACTED;
                break;

            case (int)ePersonState.WATCH_PARADE:
            default:

                lookTarget = Camera.main.gameObject.transform;
                // When watching parade, it keeps focus for a little longer than other behaviours
                nextBehaviourChangeCounter *= 1.5f;
                currentState = ePersonState.WATCH_PARADE;
                break;

        }

    }

    /// <summary>
    /// Finds closest other person in the crowd within search radius.
    /// </summary>
    /// <param name="searchRadius">Radius around person to find closest neighbour in crowd.</param>
    /// <returns>Transform of closest other person in crowd, or null if no such person was found.</returns>
    private Transform findNearestNeighbour(float searchRadius)
    {

        Transform nearestNeighbour = null;
        List<Person> allPeople = EntityManager.Instance.AllThePeople;
        List<Person> peopleOnMyBlock = new List<Person>();

        for(int i = 0; i < allPeople.Count; i++)
        {

            // Note: Instance IDs are compared so that a Person doesn't talk to themselves ^_^*
            if(allPeople[i].BlockIndex == myBlockIndex && (allPeople[i].gameObject.GetInstanceID() != this.gameObject.GetInstanceID()))
            {
                peopleOnMyBlock.Add(allPeople[i]);
            }

        }

        float distanceToClosestPerson = ParadeConstants.CityBlockSize * 2.0f;
        float tempDistance = 0.0f;
        Person closestPerson = null;

        for(int j = 0; j < peopleOnMyBlock.Count; j++)
        {

            tempDistance = Vector3.Distance(peopleOnMyBlock[j].transform.position, gameObject.transform.position);

            if (tempDistance < distanceToClosestPerson)
            {
                if (tempDistance <= searchRadius)
                {
                    closestPerson = peopleOnMyBlock[j];
                }
            }

        }

        if(closestPerson != null)
        {
            nearestNeighbour = closestPerson.gameObject.transform;
        }

        return nearestNeighbour;

    }

    private void checkForDistractions()
    {

        distractionCheckCounter = CityStreamManager.Instance.DistractionCheckFrequency;
        List<Distraction> allDistractions = EntityManager.Instance.AllTheDistractions;
        List<Distraction> nearbyDistractions = new List<Distraction>();

        for (int i = 0; i < allDistractions.Count; i++)
        {

            // Only consider distractions valid if they are "in front of" and close enough to me
            if ((allDistractions[i].transform.position.z > gameObject.transform.position.z) && allDistractions[i].IsCurrentADistraction && Vector3.Distance(allDistractions[i].transform.position, gameObject.transform.position) < distractionRadius)
            {
                nearbyDistractions.Add(allDistractions[i]);
            }

        }

        // Choose a random distraction from the set we found
        if(nearbyDistractions.Count > 0)
        {

            distractedLookTarget = nearbyDistractions[Random.Range(0, nearbyDistractions.Count)].transform;
            changeToSpecificBehaviour((int)ePersonState.DISTRACTED);

        }

    }

    /// <summary>
    /// Tries to start a conversation with another Person. Might not succeed.
    /// </summary>
    /// <returns>True if this person wants to talk. False if they don't seem interested.</returns>
    public bool willStartConversation(Person neighbour)
    {

        bool canTalkRightNow = false;

        if(personStarted && currentState != ePersonState.TALK_TO_NEIGHBOUR && currentState != ePersonState.DISTRACTED)
        {

            if (Random.Range(1, CityStreamManager.Instance.TalkChance) == 1)
            {

                canTalkRightNow = true;
                startConversationWithNeighbour(neighbour);

            }

        }

        return canTalkRightNow;

    }

    private void startConversationWithNeighbour(Person neighbour)
    {

        chatPartner = neighbour;
        lookTarget = chatPartner.Head;

    }

    /// <summary>
    /// Tell chat partner that conversation has ended
    /// </summary>
    private void endConversationWithNeighbour()
    {

        if (chatPartner != null)
        {
            chatPartner.handleEndedConversation();
            chatPartner = null;
        }

    }

    /// <summary>
    /// The neighbour I was talking to stopped talking for some reason
    /// </summary>
    public void handleEndedConversation()
    {

        chatPartner = null;

        if (currentState == ePersonState.TALK_TO_NEIGHBOUR)
        {
            changeToRandomBehaviour();
        }

    }

    public bool Equals(Person other)
    {
        return (other.gameObject.GetInstanceID() == this.gameObject.GetInstanceID());
    }

    protected override void handleCleanup()
    {
        EntityManager.Instance.RemovePerson(this);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void OnDrawGizmos()
    {

        Color c = Color.white;

        if (drawDebugGizmos && myHead != null && lookTarget != null)
        {

            if (currentState == ePersonState.DISTRACTED)
            {

                c = Color.yellow;
                Gizmos.color = c;

                Gizmos.DrawLine(myHead.transform.position, lookTarget.transform.position);
                Gizmos.DrawWireSphere(lookTarget.transform.position, 0.5f);

            }
            else if (currentState == ePersonState.TALK_TO_NEIGHBOUR)
            {

                c = Color.magenta;
                Gizmos.color = c;

                Gizmos.DrawLine(myHead.transform.position, chatPartner.Head.position);
                Gizmos.DrawWireSphere(chatPartner.Head.position, 0.5f);

            }
            else if(currentState == ePersonState.WATCH_PARADE)
            {

                c = Color.green;
                Gizmos.color = c;

                Gizmos.DrawLine(myHead.transform.position, lookTarget.transform.position);

            }
            else
            {

                c = Color.red;
                c.a = 0.5f;
                Gizmos.color = c;

                Gizmos.DrawSphere(myHead.transform.position, 1.0f);

                c.a = 1.0f;
                Gizmos.color = c;

                Gizmos.DrawLine(myHead.transform.position, lookTarget.transform.position);

            }

        }

    }

#endif

}
