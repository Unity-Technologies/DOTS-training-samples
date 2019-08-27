using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWinner : MonoBehaviour
{
    public GameObject Canvas;

    public Text WinnerText;
    public Text WinText;

    public void ShowWinners(List<Players> winners, float time)
    {
        Canvas.SetActive(true);

        if (winners.Count == 4)
        {
            WinnerText.text = "The game is tied!";
            WinText.enabled = false;
        }
        else
        {
            string text = "";
            for (int i = 0; i < winners.Count; ++i)
            {
                if (i != 0)
                    text += ", ";
                text += winners[i];
            }

            WinnerText.text = text;
            WinText.enabled = true;
        }

        Invoke("Hide", time);
    }

    private void Hide()
    {
        Canvas.SetActive(false);
    }
}
