using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardInput : MonoBehaviour {
    TextMeshProUGUI text;

    static bool showText=true;

    void Start () {
        text = GetComponent<TextMeshProUGUI>();
        text.enabled = showText;
    }
	
    void Update () {
        if (Input.GetKeyDown(KeyCode.H)) {
            showText = !showText;
            text.enabled = showText;
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            Time.timeScale = 2f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3)) {
            Time.timeScale = 3f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)) {
            Time.timeScale = 4f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5)) {
            Time.timeScale = 5f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6)) {
            Time.timeScale = 6f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            Time.timeScale = 7f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            Time.timeScale = 8f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9)) {
            Time.timeScale = 9f;
        }
    }
}