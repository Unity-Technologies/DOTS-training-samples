using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupAutoFade : MonoBehaviour
{
    [SerializeField] private float _fadeDelayTime = 2f;
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private CanvasGroup _canvasGroup = null;

    void Start()
    {
        StartCoroutine(GroupFade(_fadeTime));
    }

    public IEnumerator GroupFade(float fadeTime = 1f)
    {
        float timePassed = 0f;

        // Wait for our delay to finish
        while (timePassed < _fadeDelayTime)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        timePassed = 0f;

        // Begin fading out
        while(timePassed < fadeTime)
        {
            timePassed += Time.deltaTime;
            _canvasGroup.alpha = 1f - (timePassed / fadeTime);

            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }
}
