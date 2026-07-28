using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnComponent : GameComponent
    {
        private readonly Game m_Game;

        private int m_LastTriggeredYear = -1;
        private int m_LastTriggeredDay = -1;

        private bool m_DisplayActive = false;
        private float m_DisplayTimer = 0f;
        private float m_CurrentAlpha = 1f;

        private string m_CachedUpperText;
        private string m_CachedBottomText;

        private Vector2 m_CachedUpperSize;
        private Vector2 m_CachedBottomSize;

        public DawnComponent(Game game)
            : base() => m_Game = game;

        private void TriggerDawnOfANewDay()
        {
            try
            {
                var settings = DawnMod.s_Settings;

                long absTicks = m_Game.tickManager.TicksAbs;
                Vector2 location = m_Game.World.grid.LongLatOf(m_Game.CurrentMap.Tile);

                string day = (DaysRelative(settings.DayRelativeTo, absTicks, location.x) + (settings.StartsAtZero ? 0 : 1)).ToStringSafe();
                string year = GenDate.Year(absTicks, location.x).ToStringSafe();
                string quadrum = GenDate.Quadrum(absTicks, location.x).Label();
                string season = GenDate.Season(absTicks, location).Label();
                string hour = GenDate.HourOfDay(absTicks, location.x).ToStringSafe();

                m_CachedUpperText = FormatLabel(settings.UpperTextFormat, day, year, quadrum, season, hour);
                m_CachedBottomText = FormatLabel(settings.BottomTextFormat, day, year, quadrum, season, hour);

                settings.UpdateText();

                m_CachedUpperSize = settings.UpperTextStyle.TextGUIStyle.CalcSize(new GUIContent(m_CachedUpperText));
                m_CachedBottomSize = settings.BottomTextStyle.TextGUIStyle.CalcSize(new GUIContent(m_CachedBottomText));

                m_DisplayTimer = settings.DisplayDurationSeconds;
                m_DisplayActive = true;
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into DawnComponent.TriggerDawnOfANewDay()\nException: {exception}");
            }
        }

        private string FormatLabel(string format, string day, string year, string quadrum, string season, string hour)
        {
            return format
                .Replace("{}", day)
                .Replace("{D}", day)
                .Replace("{d}", day)
                .Replace("{Y}", year)
                .Replace("{y}", year.Substring(year.Length - 2))
                .Replace("{Q}", quadrum.ToUpper())
                .Replace("{q}", quadrum)
                .Replace("{S}", season.ToUpper())
                .Replace("{s}", season)
                .Replace("{H}", $"{hour:00}")
                .Replace("{h}", hour);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (m_Game.tickManager.TicksGame % GenTicks.SecondsToTicks(1f) != 0)
                return;

            if (m_Game.CurrentMap == null)
                return;

            var settings = DawnMod.s_Settings;
            if (settings.ConsumeShowExample)
            {
                TriggerDawnOfANewDay();
                return;
            }

            if (!settings.Enabled)
                return;

            long absTicks = m_Game.tickManager.TicksAbs;
            float longitude = m_Game.World.grid.LongLatOf(m_Game.CurrentMap.Tile).x;

            int currentYear = GenDate.Year(absTicks, longitude);
            int currentDay = GenDate.DayOfYear(absTicks, longitude);
            int currentHour = GenDate.HourOfDay(absTicks, longitude);

            if (currentDay <= m_LastTriggeredDay && currentYear == m_LastTriggeredYear)
                return;
            if (currentHour < settings.TriggerHour)
                return;

            m_LastTriggeredYear = currentYear;
            m_LastTriggeredDay = currentDay;

            if (m_LastTriggeredDay % settings.ShowEveryXDays == 0)
                TriggerDawnOfANewDay();
        }

        public override void GameComponentUpdate()
        {
            base.GameComponentUpdate();

            if (!m_DisplayActive)
                return;

            m_DisplayTimer -= Time.unscaledDeltaTime;
            if (m_DisplayTimer <= 0f)
            {
                m_DisplayActive = false;
                return;
            }

            var settings = DawnMod.s_Settings;

            float elapsedTime = settings.DisplayDurationSeconds - m_DisplayTimer;
            m_CurrentAlpha = 1f;
            if (elapsedTime < settings.FadeInDurationSeconds)
                m_CurrentAlpha = elapsedTime / settings.FadeInDurationSeconds;
            else if (m_DisplayTimer < settings.FadeOutDurationSeconds)
                m_CurrentAlpha = m_DisplayTimer / settings.FadeOutDurationSeconds;

            m_CurrentAlpha = Mathf.Clamp01(m_CurrentAlpha);
        }

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();

            if (!m_DisplayActive)
                return;

            var settings = DawnMod.s_Settings;

            if (settings.ScreenshotMode && Find.ScreenshotModeHandler.Active)
                return;

            Color defaultColor = GUI.color;
            GUI.color = Color.white.WithAlpha(m_CurrentAlpha);

            float rectWidth = Mathf.Max(m_CachedUpperSize.x, m_CachedBottomSize.x) * 1.2f;
            float rectHeight = m_CachedUpperSize.y + m_CachedBottomSize.y + settings.LineThickness + (settings.LinePadding * settings.Scale * 3f);

            Vector2 rectSize = new Vector2(rectWidth, rectHeight);
            Rect dawnLayoutRect = new Rect(settings.Offset - (rectSize / 2f), rectSize);

            if (settings.ShowHighlight)
                Widgets.DrawHighlight(dawnLayoutRect, m_CurrentAlpha);

            GUILayout.BeginArea(dawnLayoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayoutOption dayTextWidth = GUILayout.Width(rectWidth);
            Rect dayRect = GUILayoutUtility.GetRect(new GUIContent(m_CachedUpperText), settings.UpperTextStyle.TextGUIStyle, dayTextWidth);
            DrawText(dayRect, m_CachedUpperText, settings.UpperTextStyle, m_CurrentAlpha);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            float scaledLinePadding = settings.LinePadding * settings.Scale;

            GUILayout.Space(scaledLinePadding);

            float scaledLineThickness = settings.LineThickness;
            float lineWidth = rectSize.x * (settings.LineWidthPercentage / 100f);
            float lineX = (rectSize.x - lineWidth) / 2f;

            Rect lineSpace = GUILayoutUtility.GetRect(rectSize.x, scaledLineThickness);
            Rect lineRect = new Rect(lineSpace.x + lineX, lineSpace.y, lineWidth, scaledLineThickness);
            Widgets.DrawBoxSolid(lineRect, settings.LineColor.WithAlpha(m_CurrentAlpha));

            GUILayout.Space(scaledLinePadding);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayoutOption dateTextWidth = GUILayout.Width(rectWidth);
            Rect dateRect = GUILayoutUtility.GetRect(new GUIContent(m_CachedBottomText), settings.BottomTextStyle.TextGUIStyle, dateTextWidth);
            DrawText(dateRect, m_CachedBottomText, settings.BottomTextStyle, m_CurrentAlpha);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUI.color = defaultColor;
        }

        private void DrawText(Rect rect, string text, DawnTextStyle style, float alpha)
        {
            Color originalColor = style.TextColor.WithAlpha(alpha);
            Color outlineColor = style.OutlineColor.WithAlpha(alpha);

            if (style.OutlineThickness > 0)
            {
                style.TextGUIStyle.normal.textColor = outlineColor;

                float step = style.OutlineThickness / 10f;
                for (float x = -style.OutlineThickness; x <= style.OutlineThickness; x += step)
                {
                    for (float y = -style.OutlineThickness; y <= style.OutlineThickness; y += step)
                    {
                        Vector2 offset = new Vector2(x, y);
                        GUI.Label(new Rect(rect.position + offset, rect.size), text, style.TextGUIStyle);
                    }
                }
            }

            style.TextGUIStyle.normal.textColor = originalColor;
            GUI.Label(rect, text, style.TextGUIStyle);
        }

        private int DaysRelative(DayRelative dayRelative, long absTicks, float longitude)
        {
            switch (dayRelative)
            {
                case DayRelative.Settle:
                    return GenDate.DaysPassed + (GenDate.HourOfDay(absTicks, longitude) >= 6 ? 0 : 1);
                case DayRelative.Quadrum:
                    return GenDate.DayOfQuadrum(absTicks, longitude);
                case DayRelative.Season:
                    return GenDate.DayOfSeason(absTicks, longitude);
                case DayRelative.Year:
                    return GenDate.DayOfYear(absTicks, longitude);

                default:
                    break;
            }

            return 0;
        }
    }
}
