using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rebooter : MonoBehaviour
{
    public float waitTime = 15.0f;
    float time;

    void Start()
    {
	time = Time.time;
    }

    void Update()
    {
	if (Time.time - time > waitTime)
	{
	    SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
    }
}
