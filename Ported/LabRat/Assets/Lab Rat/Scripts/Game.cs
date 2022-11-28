using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

namespace ECSExamples {

public class Game : MonoBehaviour {
	public float GameLength = 30f;
    public Text gameOverText;
    public GameObject gameOverOverlay;
    public Text introText;

    bool inIntro;
	float StartTime;
    bool _didGameOver = false;

    static List<PlayerData> _players = new List<PlayerData>();

    void OnEnable() {
        StartCoroutine(Intro());
    }

     public IEnumerator WaitForRealSeconds(float time) {
         var start = Time.realtimeSinceStartup;
         var until = start + time;
         while (Time.realtimeSinceStartup < until) {
             introText.transform.localScale = Vector3.one + Vector3.one * 0.2f * Mathf.Sin((Time.realtimeSinceStartup - start) * 1.85f);
             yield return null;
         }
     }

    IEnumerator Intro() {
        Assert.IsTrue(introText != null);

        inIntro = true;

        Time.timeScale = 0;

        introText.text = "Ready...";
        introText.enabled = true;

        yield return null;
        var player = FindObjectOfType<CursorFollowMouse>();
        player.enabled = false;

        for (int i = 0; i < 10; ++i) // let unity start stuff up
            yield return null;

        yield return WaitForRealSeconds(1.85f);

        introText.text = "Set...";
        yield return WaitForRealSeconds(0.85f);

        introText.text = "Go!";

        Time.timeScale = 1f;
        yield return WaitForRealSeconds(0.85f);
        introText.enabled = false;

        player.enabled = true;

        inIntro = false;
    }

	void Start () {
		StartTime = Time.time;
	}

    void Update() {
        if (TimeLeft() <= 0 && !_didGameOver) {
            _didGameOver = true;
            StartCoroutine(GameOver());
        }

        if (!inIntro) {
            Time.timeScale = Input.GetKey(KeyCode.BackQuote) ? 5f : 1f;
            if (Input.GetKeyDown(KeyCode.G) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
                StartCoroutine(GameOver());
            }
        }
    }

    void GetWinners(List<PlayerData> players) {
        players.Clear();
        int maxCount = -1;
        foreach (var playerData in PlayerCursor.Players.Values) {
            if (playerData.mouseCount == maxCount) {
                players.Add(playerData);
            } else if (playerData.mouseCount > maxCount) {
                players.Clear();
                players.Add(playerData);
                maxCount = playerData.mouseCount;
            }
        }
    }

    static string RichTextForPlayer(PlayerData playerData) {
        return string.Format(
            "<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(playerData.Color),
            playerData.ColorName);
    }


    IEnumerator GameOver() {
        Time.timeScale = 0;

        foreach (var obj in FindObjectsOfType<Walks>()) 
            obj.enabled = false;
        foreach (var obj in FindObjectsOfType<Spawner>()) 
            obj.enabled = false;
        foreach (var obj in FindObjectsOfType<CPUPlayerCursor>())  {
            obj.Stop();
            obj.enabled = false;
        }

        yield return WaitForRealSeconds(0.5f);

        GetWinners(_players);

        string text;
        if (_players.Count == 0)
            text = "No one wins :(";
        else if (_players.Count == 1)
            text = RichTextForPlayer(_players[0]) + " Wins!";
        else
            text = "Tie!\n" + string.Join(", ", _players.Select(p => RichTextForPlayer(p)).ToArray());

        gameOverText.text = text;
        gameOverText.gameObject.SetActive(true);
        gameOverOverlay.SetActive(true);

        yield return WaitForRealSeconds(4.0f);

        // reload scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
	
	public float TimeLeft() {
		return Mathf.Max(0, GameLength - (Time.time - StartTime));
	}
}

}
