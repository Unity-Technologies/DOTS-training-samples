using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScores : MonoBehaviour
{
    public Text[] playerScores;
    public static Text[] staticPlayerScores;

    // Start is called before the first frame update
    void Start()
    {
        staticPlayerScores = new Text[4];

        for (int i = 0; i < 4; i++)
        {
            staticPlayerScores[i] = playerScores[i];
        }
    }

}
