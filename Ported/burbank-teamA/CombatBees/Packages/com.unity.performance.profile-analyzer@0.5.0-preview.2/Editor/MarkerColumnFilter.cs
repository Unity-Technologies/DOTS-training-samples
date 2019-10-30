using System;
using UnityEngine.Assertions;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class MarkerColumnFilter
    {
        public enum Mode
        {
            TimeAndCount,
            Time,
            Totals,
            TimeWithTotals,
            CountTotals,
            CountPerFrame,
            Custom,
        };

        public static readonly string[] ModeNames =
        {
            "Time and Count",
            "Time",
            "Totals",
            "Time With Totals",
            "Count Totals",
            "Count Per Frame",
            "Custom",
        };
        public static readonly int[] ModeValues = (int[]) Enum.GetValues(typeof(Mode));

        public Mode mode;
        public int[] visibleColumns;

        public MarkerColumnFilter(Mode newMode)
        {
            Assert.AreEqual(ModeNames.Length, ModeValues.Length, "Number of ModeNames should match number of enum values ModeValues: You probably forgot to update one of them.");

            mode = newMode;
            if (mode == Mode.Custom)
                mode = Mode.TimeAndCount;

            visibleColumns = null;
        }
    }
}