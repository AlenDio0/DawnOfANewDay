using UnityEngine;
using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnDefault
    {
        #region Header

        public static bool Enabled => true;
        public static bool ScreenshotMode => false;

        #endregion

        #region Appearance Section

        public static bool ShowHighlight => true;

        public static float Scale => 2f;
        public static Vector2 Offset => new Vector2(UI.screenWidth, UI.screenHeight) / 2f;

        public static float LineWidthPercentage => 80f;
        public static float LineThickness => 4f;
        public static float LinePadding => 8f;
        public static Color LineColor => Color.gray;

        #endregion

        #region Duration Section

        public static float DisplayDurationSeconds => 8f;
        public static float FadeInDurationSeconds => 1f;
        public static float FadeOutDurationSeconds => 2f;

        #endregion

        #region Label Format Section

        public static string UpperTextFormat => "DAY {DAY_SETTLE} <size={UPPER_FONTSIZE / 2}>{HOUR_D2}:00 | <color={TEMPERATURE_COLOR}>{TEMPERATURE}</color></size>";
        public static string BottomTextFormat => "YEAR {YEAR} | <upper>{QUADRUM}</upper> | <upper>{SEASON}</upper>";

        #endregion

        #region Sound Section

        public static bool SoundEnabled => true;

        public static float SoundVolume => 0.25f;
        public static float SoundPitch => 1f;

        #endregion

        #region Extra Section

        public static bool StartsAtZero => false;
        public static int ShowEveryXDays => 1;
        public static int TriggerHour => 6;
        public static DayRelative DayRelativeTo => DayRelative.Settle;

        #endregion

    }
}
