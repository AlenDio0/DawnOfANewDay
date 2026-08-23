using DawnNewDay.Compatibility;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
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

        public readonly static float SubtitleGap = 8f;

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

        #region Text Section

        public static DawnTextStyle UpperTextStyle => new(40, true, false);
        public static DawnTextStyle BottomTextStyle => new(24, false, false);
        public static DawnTextStyle SubtitleTextStyle => new(12, false, true, 0.25f);

        #endregion

        #region Label Format Section

        public readonly static string UpperTextFormat = "DAY {DAY_SETTLE} <size={UPPER_FONTSIZE / 2}>{HOUR_D2}:00 | <color={TEMPERATURE_COLOR}>{TEMPERATURE}</color></size>";
        public readonly static string BottomTextFormat = "YEAR {YEAR} | <upper>{QUADRUM}</upper> | <upper>{SEASON}</upper>";
        public readonly static string SubtitleTextFormat = "{FACTION} ~ {SETTLEMENT}";

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

        #region Compatibility Section

        #region Modern Notifications Section

        public readonly static string MN_ReminderTextFormat =
            "<color={MN_REMINDER_COLOR}><size={MN_REMINDER_FONTSIZE / 1.5}>{MN_REMINDER_TITLE}</size>{ENDLINE}-{MN_REMINDER_REMAINING} Remain-</color>";
        public static DawnTextStyle MN_ReminderTextStyle => new(18, true, false, 0.25f);

        public static ModernNotificationsSettings MN_Reminder => new(true, 15, MN_ReminderTextFormat, MN_ReminderTextStyle);

        public readonly static string MN_OccasionTextFormat =
            "<size={MN_OCCASION_FONTSIZE / 1.5}>[{MN_OCCASION_CATEGORY}] {MN_OCCASION_LABEL}</size>{ENDLINE}-{MN_OCCASION_REMAINING} Remain-";
        public static DawnTextStyle MN_OccasionTextStyle => new(16, true, false, 0.25f);

        public static ModernNotificationsSettings MN_Occasion => new(false, 15, MN_OccasionTextFormat, MN_OccasionTextStyle);

        #endregion

        #endregion
    }
}
