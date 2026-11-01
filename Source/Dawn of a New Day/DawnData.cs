using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnData
    {
        public static string ModName => "Dawn of a New Day";

        #region Logging

        private static string FormatLog(string message) => $"[{ModName}] {message}.";
        public static void Warn(string message) => Log.Warning(FormatLog(message));
        public static void Info(string message) => Log.Message(FormatLog(message));
        public static void Error(string message) => Log.Error(FormatLog(message));
        public static void Exception(Exception exception, [CallerMemberName] string functionName = "") =>
            Error($"Exception catched into {functionName}() - {exception}");

        #endregion

        #region Texture

        #region Cached Texture

        private static Texture2D GetCachedTexture(ref Texture2D cachedTexture, string fileName)
        {
            if (cachedTexture == null)
                cachedTexture = ContentFinder<Texture2D>.Get(fileName);
            return cachedTexture;
        }

        private static Texture2D m_CachedCurlyBrackets;
        private static Texture2D m_CachedAngleBrackets;
        private static Texture2D m_CachedSquareBrackets;

        private static Texture2D m_CachedCurlyBracketsMN;
        private static Texture2D m_CachedSquareBracketsMN;

        #endregion

        public static Texture2D CurlyBrackets => GetCachedTexture(ref m_CachedCurlyBrackets, "Curly Brackets");
        public static Texture2D AngleBrackets => GetCachedTexture(ref m_CachedAngleBrackets, "Angle Brackets");
        public static Texture2D SquareBrackets => GetCachedTexture(ref m_CachedSquareBrackets, "Square Brackets");

        public static Texture2D CurlyBracketsMN => GetCachedTexture(ref m_CachedCurlyBracketsMN, "Curly Brackets MN");
        public static Texture2D SquareBracketsMN => GetCachedTexture(ref m_CachedSquareBracketsMN, "Square Brackets MN");

        #endregion

        #region Settings

        public const float TinyRowHeight = 30f;
        public const float MediumRowHeight = 40f;
        public const float BigRowHeight = 50f;

        #region Appearance Section

        public static readonly float[] ScalePresets = [0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f];

        #endregion

        #region Sound Section

        public static string DefaultSoundDefName => "DawnSound_MajorasMask";

        #endregion

        #endregion
    }
}
