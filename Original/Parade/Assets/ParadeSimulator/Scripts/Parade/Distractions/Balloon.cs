using UnityEngine;

/// <summary>
/// Manages basic Balloon behaviour and allows Balloon to be let go by Person and become a distraction for the crowd.
/// </summary>
public class Balloon : Distraction {

    private Vector3 startPosition = Vector3.zero;
    private float bounceDistanceFactor = 0.1f;
    private float bounceFrequencyFactor = 2.0f;
    private bool anchored = true;
    private Vector3 flyAwaySpeed = new Vector3(0.0f, 5.0f, 0.0f);

	void Start ()
    {
        startPosition = gameObject.transform.position;
	}

    protected override void Update()
    {

        base.Update();

        if (anchored)
        {

            gameObject.transform.position = startPosition + new Vector3(0.0f, Mathf.Sin(Time.time * bounceFrequencyFactor) * bounceDistanceFactor, 0.0f);

            // Every frame, there's a chance to accidentally fly away and become a distraction
            if (Random.Range(1, CityStreamManager.Instance.BalloonLetGoChance) == 1)
            {
                anchored = false;
                IsCurrentADistraction = true;
            }

        }
        else
        {
            gameObject.transform.Translate(flyAwaySpeed * Time.deltaTime);
        }

	}

}
