using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic container and management class to reduce memory use. This keeps track of all the Person and Distraction 
/// objects that Person objects need reference to when looking for things to be distracted by or neighbours to chat 
/// with. Housing all that here reduces memory management overhead and puts further emphasis on CPU-bound aspects
/// of the game objects.
/// </summary>
public class EntityManager : MonoBehaviour {

    private static EntityManager _instance;
    public static EntityManager Instance {
        get { return _instance; }
    }

    private List<Person> allThePeople;
    public List<Person> AllThePeople {
        get { return allThePeople; }
    }

    private List<Distraction> allTheDistractions;
    public List<Distraction> AllTheDistractions {
        get { return allTheDistractions; }
    }

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
            allThePeople = new List<Person>();
            allTheDistractions = new List<Distraction>();
        }

    }

    public void AddPerson(Person p)
    {
        allThePeople.Add(p);
    }

    public void RemovePerson(Person p)
    {
        allThePeople.Remove(p);
    }

    public void AddDistraction(Distraction d)
    {
        allTheDistractions.Add(d);
    }

    public void RemoveDistraction(Distraction d)
    {
        allTheDistractions.Remove(d);
    }

}
