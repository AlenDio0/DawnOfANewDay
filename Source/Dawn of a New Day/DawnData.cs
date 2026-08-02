using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public struct FormatContext
    {
        public string Day;
        public string Year;
        public string Quadrum;
        public string Season;
        public string Hour;
    }

    [StaticConstructorOnStartup]
    public static class DawnData
    {
        public static string ModName => "Dawn of a New Day";

        #region Logging

        private static string FormatLog(string message) => $"[{ModName}] {message}.";
        public static void Warn(string message) => Log.Warning(FormatLog(message));
        public static void Info(string message) => Log.Message(FormatLog(message));
        public static void Error(string message) => Log.Error(FormatLog(message));

        #endregion

        #region Settings

        #region Appearance Section

        public static readonly float[] ScalePresets = { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f };

        public static (TaggedString Name, Vector2 Preset)[] OffsetPresets => new[]
        {
            ("DawnNewDay.Offset_MiddleTop".Translate(), new Vector2(0.5f, 0.2f)),
            ("DawnNewDay.Offset_MiddleBottom".Translate(), new Vector2(0.5f, 0.8f)),
            ("DawnNewDay.Offset_MiddleMiddle".Translate(), new Vector2(0.5f, 0.5f)),
            ("DawnNewDay.Offset_LeftMiddle".Translate(), new Vector2(0.2f, 0.5f)),
            ("DawnNewDay.Offset_RightMiddle".Translate(), new Vector2(0.8f, 0.5f)),
            ("DawnNewDay.Offset_LeftTop".Translate(), new Vector2(0.2f, 0.2f)),
            ("DawnNewDay.Offset_RightTop".Translate(), new Vector2(0.8f, 0.2f)),
            ("DawnNewDay.Offset_LeftBottom".Translate(), new Vector2(0.2f, 0.8f)),
            ("DawnNewDay.Offset_RightBottom".Translate(), new Vector2(0.8f, 0.8f)),
        };

        #endregion

        #region Label Format Section

        public static Dictionary<string, Func<FormatContext, string>> FormatTokens => new Dictionary<string, Func<FormatContext, string>>
        {
            { "{}",  context => context.Day },
            { "{D}", context => context.Day },
            { "{d}", context => context.Day },

            { "{Y}", context => context.Year },
            { "{y}", context => (context.Year.Length > 2 ? context.Year.Substring(context.Year.Length - 2) : context.Year) },

            { "{Q}", context => context.Quadrum.ToUpper() },
            { "{q}", context => context.Quadrum },

            { "{S}", context => context.Season.ToUpper() },
            { "{s}", context => context.Season },

            { "{H}", context => $"{context.Hour:00}" },
            { "{h}", context => context.Hour }
        };

        #endregion

        #region Sound Section

        public static string DefaultSoundDefName => "DawnSound_MajorasMask";

        #endregion

        #endregion
    }
}
