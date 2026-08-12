using UnityEngine;
using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnDefault
    {
        #region Header

        public readonly static bool Enabled = true;
        public readonly static bool ScreenshotMode = false;

        #endregion

        #region Appearance Section

        public readonly static bool ShowHighlight = true;

        public readonly static float Scale = 2f;
        public readonly static Vector2 Offset = new Vector2(UI.screenWidth, UI.screenHeight) / 2f;

        public readonly static float LineWidthPercentage = 80f;
        public readonly static float LineThickness = 4f;
        public readonly static float LinePadding = 6f;
        public readonly static Color LineColor = Color.gray;

        #endregion

        #region Duration Section

        public readonly static float DisplayDurationSeconds = 8f;
        public readonly static float FadeInDurationSeconds = 1f;
        public readonly static float FadeOutDurationSeconds = 2f;

        #endregion

        #region Label Format Section

        public readonly static string UpperTextFormat = "DAY {DAY_SETTLE} <size={UPPER_FONTSIZE / 2}>{HOUR_D2}:00 | <color={TEMPERATURE_COLOR}>{TEMPERATURE}</color></size>";
        public readonly static string BottomTextFormat = "YEAR {YEAR} | <upper>{QUADRUM}</upper> | <upper>{SEASON}</upper>";

        #endregion

        #region Sound Section

        public readonly static bool SoundEnabled = true;

        public readonly static float SoundVolume = 0.25f;
        public readonly static float SoundPitch = 1f;

        #endregion

        #region Extra Section

        public readonly static bool StartsAtZero = false;
        public readonly static int ShowEveryXDays = 1;
        public readonly static int TriggerHour = 6;
        public readonly static DayRelative DayRelativeTo = DayRelative.Settle;

        #endregion

    }
}
