using System;
using UnityEngine.Assertions;
using UnityEngine;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public enum Units
    {
        Milliseconds,
        Microseconds,
        Count,
    };

    public class DisplayUnits
    {
        public static readonly string[] UnitNames =
        {
            "Milliseconds",
            "Microseconds",
            "Count",
        };

        public static readonly int[] UnitValues = (int[]) Enum.GetValues(typeof(Units));
        
        public readonly Units Units;

        public DisplayUnits(Units units)
        {
            Assert.AreEqual(UnitNames.Length, UnitValues.Length, "Number of UnitNames should match number of enum values UnitValues: You probably forgot to update one of them.");

            Units = units;
        }

        public string Postfix()
        {
            switch (Units)
            {
                default:
                case Units.Milliseconds:
                    return "ms";
                case Units.Microseconds:
                    return "us";
                case Units.Count:
                    return "";
            }
        }

        private int ClampToRange(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;

            return value;
        }

        public string ToString(float ms, bool showUnits, int limitToNDigits)
        {
            float value = ms;
            int unitPower = -3;

            int maxDecimalPlaces = 0;
            switch (Units)
            {
                default:
                case Units.Milliseconds:
                    maxDecimalPlaces = 2;
                    break;
                case Units.Microseconds:
                    maxDecimalPlaces = 0;
                    value *= 1000f;
                    unitPower -= 3;
                    break;
                case Units.Count:
                    maxDecimalPlaces = 0;
                    showUnits = false;
                    break;
            }


            int numberOfDecimalPlaces = maxDecimalPlaces;
            int unitsTextLength = showUnits ? 2 : 0;

            if (limitToNDigits>0)
            {
                int originalUnitPower = unitPower;

                float limitRange = (float)Math.Pow(10, limitToNDigits);

                if (limitRange > 0 && value >= limitRange)
                {
                    while (value >= 1000f && unitPower < 9)
                    {
                        value /= 1000f;
                        unitPower += 3;
                    }
                }

                if (unitPower != originalUnitPower)
                    showUnits = true;
            
                int numberOfSignificantFigures = limitToNDigits - unitsTextLength;
                int numberOfDigitsBeforeDecimalPoint = 1 + Math.Max(0, (int)Math.Log10((int)value));
                numberOfDecimalPlaces = ClampToRange(numberOfSignificantFigures - numberOfDigitsBeforeDecimalPoint, 0, maxDecimalPlaces);
            }

            string siUnitString = showUnits ? GetSIUnitString(unitPower) + "s" : "";

            string formatString = string.Concat("{0:f", numberOfDecimalPlaces, "}{1}");

            return string.Format(formatString, value, siUnitString);
        }

        public string GetSIUnitString(int unitPower)
        {
            switch (unitPower)
            {
                case -6:
                    return "u";
                case -3:
                    return "m";
                case 0:
                    return "";
                case 3:
                    return "k";
                case 6:
                    return "m";
            }

            return "?";
        }

        public string ToString(double ms, bool showUnits, int limitToNDigits)
        {
            return ToString((float)ms, showUnits, limitToNDigits);
        }
        
        public GUIContent ToGUIContentWithTooltips(float ms, bool showUnits = false, int limitToNDigits = 5, int frameIndex = -1)
        {
            if (frameIndex>=0)
                return new GUIContent(ToString(ms, showUnits, limitToNDigits), string.Format("{0} on frame {1}", ToString(ms, true, 0), frameIndex));

            return new GUIContent(ToString(ms, showUnits, limitToNDigits), ToString(ms, true, 0));
        }
    }
}