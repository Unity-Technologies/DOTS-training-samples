using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntro : MonoBehaviour
{
    public GameObject Canvas;

    public void Play(float time)
    {
        Canvas.SetActive(true);
        Invoke("Hide", time);
    }

    private void Hide()
    {
        Canvas.SetActive(false);
    }
}
