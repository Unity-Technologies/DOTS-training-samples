using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ECSExamples {

public class PlayerData {
    public override string ToString() {
        return string.Format("<PlayerData {0} (mice: {1}, color: {2}, colorname: {3})>",
                playerIndex, mouseCount, Color, ColorName);
    }
    public PlayerData(int playerIndex) { this.playerIndex = playerIndex; }
    public int playerIndex;
    public int mouseCount = 0;
    public Color Color {
        get {
            return PlayerCursor.PlayerColors[playerIndex];
        }
    }
    public string ColorName {
        get {
            return PlayerCursor.PlayerColorNames[playerIndex];
        }
    }

}

public class PlayerCursor : MonoBehaviour {
    public GameObject CursorPrefab;
    public Canvas canvas;

	GameObject Cursor;
    Image cursorImage;

    public static Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();

    public static PlayerData GetPlayerData(int playerIndex) {
        PlayerData data;
        if (!Players.TryGetValue(playerIndex, out data))
            data = Players[playerIndex] = new PlayerData(playerIndex);
        return data;
    }

    static List<PlayerCursor> activeCursors = new List<PlayerCursor>();

    void OnEnable() { activeCursors.Add(this); }

    void OnDisable() { activeCursors.Remove(this); }

    static public void ArrowWasRemoved(Cell cell) {
        foreach (var player in activeCursors)
            player.ArrowPlacements.Remove(cell);
    }

    [HideInInspector]
    public List<Cell> ArrowPlacements = new List<Cell>();

    public int MaxArrows = 3;

    public void AddAndExpireArrows(Cell cell) {
        ArrowPlacements.Add(cell);
        while (ArrowPlacements.Count > MaxArrows) {
            ArrowPlacements[0].RemoveArrow();
            ArrowPlacements.RemoveAt(0);
        }
    }

    public void RemoveArrow(Cell cell) {
        ArrowPlacements.Remove(cell);
    }

    public static string[] PlayerColorNames = new string[] {
        "Black",
        "Red",
        "Green",
        "Blue",
    };

	public static Color[] PlayerColors = new Color[] {
		new Color(0, 0, 0, 1f),
		new Color(1f, 0.2f, 0.2f, 1f),
		new Color(0.2f, 1f, 0.2f, 1f),
		new Color(0.2f, 0.2f, 1f, 1f),
	};

    public void Init(Canvas canvas) {
        this.canvas = canvas;
		Cursor = Instantiate<GameObject>(CursorPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
        cursorImage = Cursor.GetComponentInChildren<Image>();
    }

    public void SetColor(Color color) {
        cursorImage.color = color;
    }

    public void VisualClick() {
        StartCoroutine(_VisualClick());
    }

    IEnumerator _VisualClick() {
        var fromScale = new Vector3(0.5f, 0.5f, 0.5f);
        var toScale = Vector3.one;
        var tr = cursorImage.transform;

        float duration = 0.2f;
        float t = 0f;
        while (t < duration) {
            tr.localScale = Vector3.Lerp(fromScale, toScale, Mathf.Clamp01(t / duration));
            yield return null;
            t += Time.deltaTime;
        }

        tr.localScale = toScale;
    }

	public void SetScreenPosition(Vector2 screenPoint) {
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out pos);
		Cursor.transform.position = canvas.transform.TransformPoint(pos);
	}

}

}
