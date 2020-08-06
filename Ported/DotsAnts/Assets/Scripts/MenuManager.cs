using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    Text text;

    static bool showText = true;
    AntDefaults defaults;
    float lastSpeed = 1f;

    void Start()
    {
        text = GetComponent<Text>();
        text.enabled = showText;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            showText = !showText;
            text.enabled = showText;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 1f / lastSpeed;
            lastSpeed = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 2f / lastSpeed;
            lastSpeed = 2f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 3f / lastSpeed;
            lastSpeed = 3f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 4f / lastSpeed;
            lastSpeed = 4f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 5f / lastSpeed;
            lastSpeed = 5f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 6f / lastSpeed;
            lastSpeed = 6f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 7f / lastSpeed;
            lastSpeed = 7f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 8f / lastSpeed;
            lastSpeed = 8f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GameObject.Find("Default values").GetComponent<AntDefaults>().antSpeed *= 9f / lastSpeed;
            lastSpeed = 9f;
        }
    }
}
