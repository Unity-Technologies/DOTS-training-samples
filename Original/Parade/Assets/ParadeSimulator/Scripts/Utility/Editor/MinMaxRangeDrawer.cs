using UnityEngine;
using UnityEditor;

/// <summary>
/// Handles drawing the min-max property in editor for the various Parade parameters that are variable and random (such as distraction timing for Person objects)
/// </summary>
[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeEditor : PropertyDrawer
{

    private static float widthFactor = 0.3f;
    private static float heightFactor = 0.5f;
    private static float minSpacing = 30.0f;
    private static float maxSpacing = 40.0f;
    private static string rangeLabelFormat = "0.##";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 16;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.type != "MinMaxRange")
        {
            Debug.LogWarning("Use only with MinMaxRange type");
        }
        else
        {

            MinMaxRangeAttribute range = attribute as MinMaxRangeAttribute;
            var minValue = property.FindPropertyRelative("minimumValue");
            var maxValue = property.FindPropertyRelative("maximumValue");
            float newMin = minValue.floatValue;
            float newMax = maxValue.floatValue;

            float xPosDifference = position.width * widthFactor;
            float yPosDifference = position.height * heightFactor;

            EditorGUI.LabelField(new Rect(position.x, position.y, xPosDifference, yPosDifference), label);
            EditorGUI.LabelField(new Rect(position.x, position.y + yPosDifference, position.width, yPosDifference), range.minLimit.ToString(rangeLabelFormat));
            EditorGUI.LabelField(new Rect(position.x + position.width - minSpacing, position.y + yPosDifference, position.width, yPosDifference), range.maxLimit.ToString(rangeLabelFormat));
            EditorGUI.MinMaxSlider(new Rect(position.x + minSpacing, position.y + yPosDifference, position.width - (minSpacing*2.0f), yPosDifference), ref newMin, ref newMax, range.minLimit, range.maxLimit);

            EditorGUI.LabelField(new Rect(position.x + xPosDifference, position.y, xPosDifference, yPosDifference), "MIN:");
            newMin = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xPosDifference + minSpacing, position.y, xPosDifference - minSpacing, yPosDifference), newMin), range.minLimit, newMax);
            EditorGUI.LabelField(new Rect(position.x + xPosDifference * 2.0f, position.y, xPosDifference, yPosDifference), "MAX:");
            newMax = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xPosDifference * 2.0f + maxSpacing, position.y, xPosDifference - maxSpacing, yPosDifference), newMax), newMin, range.maxLimit);

            minValue.floatValue = newMin;
            maxValue.floatValue = newMax;

        }

    }

}
