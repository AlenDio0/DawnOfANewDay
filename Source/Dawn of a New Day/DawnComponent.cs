using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

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

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (m_Game.tickManager.TicksGame % GenTicks.SecondsToTicks(1f) != 0)
                return;

            if (m_Game.CurrentMap == null)
                return;

            var settings = DawnMod.Settings;
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

            if (settings.ShowEveryXDays > 0 && m_LastTriggeredDay % settings.ShowEveryXDays == 0)
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

            var settings = DawnMod.Settings;

            m_CurrentAlpha = 1f;

            float elapsedTime = settings.DisplayDurationSeconds - m_DisplayTimer;
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

            var settings = DawnMod.Settings;

            if (settings.ScreenshotMode && Find.ScreenshotModeHandler.Active)
                return;

            Color defaultColor = GUI.color;
            GUI.color = Color.white.WithAlpha(m_CurrentAlpha);

            float rectWidth = Mathf.Max(m_CachedUpperSize.x, m_CachedBottomSize.x) * 1.2f;
            float rectHeight = m_CachedUpperSize.y + m_CachedBottomSize.y + settings.LineThickness + (settings.LinePadding * settings.Scale * 3f);

            GUILayoutOption textWidth = GUILayout.Width(rectWidth);

            Vector2 rectSize = new Vector2(rectWidth, rectHeight);
            Rect overlaytRect = new Rect(settings.Offset - (rectSize / 2f), rectSize);

            if (settings.ShowHighlight)
                Widgets.DrawHighlight(overlaytRect);

            GUILayout.BeginArea(overlaytRect);

            #region UpperText

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Rect upperRect = GUILayoutUtility.GetRect(new GUIContent(m_CachedUpperText), settings.UpperTextStyle.TextGUIStyle, textWidth);
            settings.UpperTextStyle.Draw(upperRect, m_CachedUpperText, settings.Scale);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            #endregion

            #region Line

            float scaledLinePadding = settings.LinePadding * settings.Scale;

            GUILayout.Space(scaledLinePadding);

            float scaledLineThickness = settings.LineThickness;
            float lineWidth = rectWidth * (settings.LineWidthPercentage / 100f);
            float lineX = (rectWidth - lineWidth) / 2f;

            Rect lineSpace = GUILayoutUtility.GetRect(rectWidth, scaledLineThickness);
            Rect lineRect = new Rect(lineSpace.x + lineX, lineSpace.y, lineWidth, scaledLineThickness);
            Widgets.DrawBoxSolid(lineRect, settings.LineColor.WithAlpha(m_CurrentAlpha));

            GUILayout.Space(scaledLinePadding);

            #endregion

            #region BottomText

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Rect bottomRect = GUILayoutUtility.GetRect(new GUIContent(m_CachedBottomText), settings.BottomTextStyle.TextGUIStyle, textWidth);
            settings.BottomTextStyle.Draw(bottomRect, m_CachedBottomText, settings.Scale);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            #endregion

            GUILayout.EndArea();

            GUI.color = defaultColor;
        }

        private void TriggerDawnOfANewDay()
        {
            try
            {
                var settings = DawnMod.Settings;

                FormatContext context = CreateFormatContext();
                m_CachedUpperText = FormatLabel(settings.UpperTextFormat, context);
                m_CachedBottomText = FormatLabel(settings.BottomTextFormat, context);

                settings.UpdateText();

                m_CachedUpperSize = settings.UpperTextStyle.TextGUIStyle.CalcSize(new GUIContent(m_CachedUpperText));
                m_CachedBottomSize = settings.BottomTextStyle.TextGUIStyle.CalcSize(new GUIContent(m_CachedBottomText));

                if (settings.SoundEnabled && settings.Sound != null)
                {
                    SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo());
                    soundInfo.volumeFactor = settings.SoundVolume;
                    soundInfo.pitchFactor = settings.SoundPitch;
                    soundInfo.forcedPlayOnCamera = true;
                    settings.Sound.PlayOneShot(soundInfo);
                }

                m_DisplayTimer = settings.DisplayDurationSeconds;
                m_DisplayActive = true;
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into DawnComponent.TriggerDawnOfANewDay()\nException: {exception}");
            }
        }

        private FormatContext CreateFormatContext()
        {
            var settings = DawnMod.Settings;

            long absTicks = m_Game.tickManager.TicksAbs;
            Vector2 location = m_Game.World.grid.LongLatOf(m_Game.CurrentMap.Tile);

            return new FormatContext
            {
                Day = (MapDayRelative(settings.DayRelativeTo, absTicks, location.x) + (settings.StartsAtZero ? 0 : 1)).ToStringSafe(),
                Year = GenDate.Year(absTicks, location.x).ToStringSafe(),
                Quadrum = GenDate.Quadrum(absTicks, location.x).Label(),
                Season = GenDate.Season(absTicks, location).LabelCap(),
                Hour = GenDate.HourOfDay(absTicks, location.x).ToStringSafe()
            };
        }

        private string FormatLabel(string format, FormatContext context)
        {
            if (format.NullOrEmpty())
                return "";

            foreach ((string token, var replacer) in DawnData.FormatTokens)
            {
                if (format.Contains(token))
                    format = format.Replace(token, replacer?.Invoke(context));
            }

            return format;
        }

        private int MapDayRelative(DayRelative dayRelative, long absTicks, float longitude)
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
