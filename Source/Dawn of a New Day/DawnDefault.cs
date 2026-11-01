using DawnNewDay.Compatibility;
using System.Collections.Generic;
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

        #endregion

        #region Duration Section

        public readonly static float DisplayDurationSeconds = 8f;
        public readonly static float FadeInDurationSeconds = 1f;
        public readonly static float FadeOutDurationSeconds = 2f;

        #endregion

        #region Overlay Widget Section

        public readonly static string UpperTextFormat = "DAY {DAY_SETTLE} <size={FONTSIZE / 2}>{HOUR_D2}:00 | <color={TEMPERATURE_COLOR}>{TEMPERATURE}</color></size>";
        public readonly static string BottomTextFormat = "YEAR {YEAR} | <upper>{QUADRUM}</upper> | <upper>{SEASON}</upper>";
        public readonly static string SubtitleTextFormat = "{FACTION} ~ {SETTLEMENT} <ifnotempty={CONDITION}>[{CONDITION}]</ifnotempty>";

        public static List<IDawnWidget> OverlayWidgets => [
            new DawnText(UpperTextFormat, 40, true, false),
            new DawnSeparator(),
            new DawnText(BottomTextFormat, 24, false, false),
            new DawnSeparator(10f, 1f, 7f),
            new DawnText(SubtitleTextFormat, 12, false, true, 0.25f),
        ];

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

        public readonly static Dictionary<string, bool> MN_ExcludeOccasionCategory = new()
        {
            { "Reminder", true },
        };

        public static string MN_ReminderTextFormat =>
            "<if={MN_REMINDER_REMAINING_DAY [ 5}><ifnotempty={MN_REMINDER_TITLE}>" +
            "<color={MN_REMINDER_SEVERITY_COLOR}><size={FONTSIZE / 1.25}>{MN_REMINDER_TITLE}</size></color>{ENDLINE}" +
            "<color={MN_REMINDER_REMAINING_COLOR}>-{MN_REMINDER_REMAINING} Remain-</color>" +
            "</ifnotempty></if>";

        public static string MN_OccasionTextFormat =>
            "<if={MN_OCCASION_REMAINING_DAY [ 5}><ifnotempty={MN_OCCASION_CATEGORY}>" +
            "<size={FONTSIZE / 1.25}>[{MN_OCCASION_CATEGORY}] {MN_OCCASION_LABEL}</size>{ENDLINE}" +
            "<color={MN_OCCASION_REMAINING_COLOR}>-{MN_OCCASION_REMAINING} Remain-</color>" +
            "</ifnotempty></if>";

        #endregion

        #endregion
    }
}
