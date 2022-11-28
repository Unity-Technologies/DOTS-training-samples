using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples {

[RequireComponent(typeof(Text))]
public class UITimer : MonoBehaviour {
	public Game game;
	Text text;

	void OnEnable() {
		text = GetComponent<Text>();
	}

	void Update () {
        var timeLeft = game.TimeLeft();
        var t = TimeSpan.FromSeconds(timeLeft);
        var answer = string.Format("{1:D2}:{2:D2}", 
            t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
        text.text = answer.ToString();
        text.color = timeLeft < 5f ? Color.red : Color.white;
	}
}

}
