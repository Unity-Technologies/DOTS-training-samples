using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

public class Homebase : MonoBehaviour {
    float lastAbsorbTime = Mathf.NegativeInfinity;
    public float AbsorbScale = 1.5f;
    public float AbsorbScaleTime = 0.2f;

    public PlayerData playerData;

    InstanceProps[] instanceProps;

    void OnEnable() {
        instanceProps = GetComponentsInChildren<InstanceProps>();
    }

    public static Homebase ForPlayerIndex(int playerIndex) {
        foreach (var homebase in FindObjectsOfType<Homebase>())
            if (homebase.playerData.playerIndex == playerIndex)
                return homebase;
        return null;
    }

    public void SetPlayerIndex(int playerIndex) {
        SetColor(PlayerCursor.PlayerColors[playerIndex]);
        playerData = PlayerCursor.GetPlayerData(playerIndex);
    }

    public void SetColor(Color color) {
        foreach (var instanceProp in instanceProps) {
            instanceProp.SetColor = true;
            instanceProp.color = color;
        }
    }

    public void DidAbsorb(Walks walker) {
        lastAbsorbTime = Time.time;

        if (walker.CompareTag("Mouse")) {
            playerData.mouseCount += 1;
        } else if (walker.CompareTag("Cat")) {
            playerData.mouseCount = (int)(playerData.mouseCount * 0.6666f);
        }
    }

    void Update() {
        transform.localScale = Vector3.one * Mathf.Lerp(AbsorbScale, 1f, (Time.time - lastAbsorbTime) / AbsorbScaleTime);
    }

}

}
