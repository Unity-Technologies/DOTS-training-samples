using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayersCursors : MonoBehaviour
{
    public GameObject cursorPrefab;
    public Transform UIRoot;
    public static RectTransform[] players;
    public static Canvas canvas;
    private Color[] colors =
    {
        new Color(0f,0f,0f),
        new Color(1f,0f,0f),
        new Color(0f,1f,0f),
        new Color(0f,0f,1f),
    };

    void Start()
    {
        players = new RectTransform[4];
        for (int i = 0; i < 4; i++)
        {
            var player = Instantiate(cursorPrefab, UIRoot);
            players[i] = (RectTransform)player.transform;
            player.GetComponentInChildren<Image>().color = colors[i];
        }

        canvas = UIRoot.GetComponent<Canvas>();
    }
}