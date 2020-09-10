using UnityEngine;
using UnityEngine.UI;

public class UIBridge : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] float tweenTime = 1f;
    [SerializeField] float tweenScale = 1.25f;
    
    [SerializeField] RectTransform readyTextTransform;
    [SerializeField] RectTransform setTextTransform;
    [SerializeField] RectTransform goTextTransform;

    [SerializeField] GameObject gameOverOverlay;
    [SerializeField] Text gameOverText;

    [SerializeField] Text timerText;
    [SerializeField] Text[] playerScoreText;
#pragma warning restore

    System.Collections.IEnumerator TweenElement(RectTransform transform, System.Action onCompleted)
    {
        transform.gameObject.SetActive(true);

        for (var elapsed = 0f; elapsed < tweenTime; elapsed += Time.deltaTime)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(1f, tweenScale, elapsed / tweenTime);
            yield return null;
        }
        
        transform.gameObject.SetActive(false);
        onCompleted?.Invoke();
    }

    public void ShowReady(System.Action onCompleted = null) => StartCoroutine(TweenElement(readyTextTransform, onCompleted));
    public void ShowSet(System.Action onCompleted = null) => StartCoroutine(TweenElement(setTextTransform, onCompleted));
    public void ShowGo(System.Action onCompleted = null) => StartCoroutine(TweenElement(goTextTransform, onCompleted));

    public void SetTimer(float timeRemaining)
    {
        timerText.text = System.TimeSpan.FromSeconds(timeRemaining).ToString(@"mm\:ss");
        timerText.color = timeRemaining < 6f ? UnityEngine.Color.red : UnityEngine.Color.white;
    }

    public void SetScore(int playerIndex, int score) => playerScoreText[playerIndex].text = score.ToString();
    
    public void SetPlayerData(int playerIndex, string name, UnityEngine.Color color) => playerScoreText[playerIndex].color = color;

    public void ShowGameOver(string winnerTeam, UnityEngine.Color winnerColor)
    {
        gameOverOverlay.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = $"{winnerTeam} Wins!";
        gameOverText.color = winnerColor;
    }

    public void ResetGUI(float duration)
    {
        gameOverOverlay.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        for(var i = 0; i < playerScoreText.Length; ++i)
            SetScore(i, 0);
        SetTimer(duration);
    }
}