using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnTranslation
    {
        #region Header

        public static string Label_Enabled => "DawnNewDay.Label_Enabled".Translate();
        public static string Label_ScreenshotMode => "DawnNewDay.Label_ScreenshotMode".Translate();

        public static string Label_ShowExample => "DawnNewDay.Label_ShowExample".Translate();
        public static string Message_ShowExample => "DawnNewDay.Message_ShowExample".Translate(DawnData.ModName);

        #endregion

        #region Appearance Section

        public static string Section_Appearance => "DawnNewDay.Section_Appearance".Translate();

        public static string Label_ShowHighlight => "DawnNewDay.Label_ShowHighlight".Translate();

        public static string Label_Scale => "DawnNewDay.Label_Scale".Translate();

        public static string Label_Offset => "DawnNewDay.Label_Offset".Translate();
        public static string Label_OffsetPresets => "DawnNewDay.Label_OffsetPresets".Translate();

        public static string Label_LineWidthPercentage => "DawnNewDay.Label_LineWidthPercentage".Translate();
        public static string Label_LineThickness => "DawnNewDay.Label_LineThickness".Translate();
        public static string Label_LinePadding => "DawnNewDay.Label_LinePadding".Translate();
        public static string Label_LineColor => "DawnNewDay.Label_LineColor".Translate();

        #endregion

        #region Duration Section

        public static string Section_Duration => "DawnNewDay.Section_Duration".Translate();

        public static string Label_DisplayDuration => "DawnNewDay.Label_DisplayDuration".Translate();
        public static string Label_FadeInDuration => "DawnNewDay.Label_FadeInDuration".Translate();
        public static string Label_FadeOutDuration => "DawnNewDay.Label_FadeOutDuration".Translate();

        #endregion

        #region Text Section

        public static string Section_Text => "DawnNewDay.Section_Text".Translate();

        public static string Section_UpperText => "DawnNewDay.Section_UpperText".Translate();
        public static string Section_BottomText => "DawnNewDay.Section_BottomText".Translate();

        public static string Label_DefaultFont => "DawnNewDay.Label_DefaultFont".Translate();

        public static string Label_FontFamily => "DawnNewDay.Label_FontFamily".Translate();
        public static string Label_FontSize => "DawnNewDay.Label_FontSize".Translate();
        public static string Label_Bold => "DawnNewDay.Label_Bold".Translate();
        public static string Label_Italic => "DawnNewDay.Label_Italic".Translate();
        public static string Label_TextColor => "DawnNewDay.Label_TextColor".Translate();
        public static string Label_OutlineThickness => "DawnNewDay.Label_OutlineThickness".Translate();
        public static string Label_OutlineColor => "DawnNewDay.Label_OutlineColor".Translate();

        public static string Label_Search => "DawnNewDay.Label_Search".Translate();

        #endregion

        #region Label Format Section

        public static string Section_LabelFormat => "DawnNewDay.Section_LabelFormat".Translate();

        public static string Label_UpperTextFormat => "DawnNewDay.Label_UpperTextFormat".Translate();
        public static string Label_BottomTextFormat => "DawnNewDay.Label_BottomTextFormat".Translate();

        public static string Hint_LabelFormat =>
            "DawnNewDay.Hint_DayFormat".Translate() + "\n" +
            "DawnNewDay.Hint_YearFormat".Translate() + "\n" +
            "DawnNewDay.Hint_QuadrumFormat".Translate() + "\n" +
            "DawnNewDay.Hint_SeasonFormat".Translate() + "\n" +
            "DawnNewDay.Hint_HourFormat".Translate();

        #endregion

        #region Sound Section

        public static string Section_Sound => "DawnNewDay.Section_Sound".Translate();

        public static string Label_Sound => "DawnNewDay.Label_Sound".Translate();

        public static string Label_SoundVolume => "DawnNewDay.Label_SoundVolume".Translate();
        public static string Label_SoundPitch => "DawnNewDay.Label_SoundPitch".Translate();

        #endregion

        #region Extra Section

        public static string Section_Extra => "DawnNewDay.Section_Extra".Translate();

        public static string Label_StartsAtZero => "DawnNewDay.Label_StartsAtZero".Translate();
        public static string Label_ShowEveryXDays => "DawnNewDay.Label_ShowEveryXDays".Translate();
        public static string Label_TriggerHour => "DawnNewDay.Label_TriggerHour".Translate();

        public static string Label_DayRelativeTo => "DawnNewDay.Label_DayRelativeTo".Translate();

        public static Dictionary<DayRelative, string> Label_DayRelative => new Dictionary<DayRelative, string>
        {
            { DayRelative.Settle, "DawnNewDay.Label_DayRelativeSettle".Translate() },
            { DayRelative.Quadrum, "DawnNewDay.Label_DayRelativeQuadrum".Translate() },
            { DayRelative.Season, "DawnNewDay.Label_DayRelativeSeason".Translate() },
            { DayRelative.Year, "DawnNewDay.Label_DayRelativeYear".Translate() }
        };

        public static Dictionary<DayRelative, string> SettingsTooltip_DayRelativeTo => new Dictionary<DayRelative, string>
        {
            { DayRelative.Settle, "DawnNewDay.Tooltip_Settle".Translate() },
            { DayRelative.Quadrum, "DawnNewDay.Tooltip_Quadrum".Translate() },
            { DayRelative.Season, "DawnNewDay.Tooltip_Season".Translate() },
            { DayRelative.Year, "DawnNewDay.Tooltip_Year".Translate() }
        };

        #endregion
    }
}
