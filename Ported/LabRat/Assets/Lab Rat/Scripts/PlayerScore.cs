using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples {
public class PlayerScore : MonoBehaviour {
    public int PlayerIndex;
    PlayerData playerData;
    Text text;

    void OnEnable() {
        text = GetComponent<Text>();
        text.color = PlayerCursor.PlayerColors[PlayerIndex];
        playerData = PlayerCursor.GetPlayerData(PlayerIndex);
    }

    void Update() {
        text.text = playerData.mouseCount.ToString();
    }
}
}
