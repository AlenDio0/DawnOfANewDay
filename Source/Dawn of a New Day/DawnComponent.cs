using RimWorld;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DawnNewDay
{
    public class DawnComponent : GameComponent
    {
        private readonly Game m_Game;
        private readonly DawnFormatter m_FormatManager;

        private int m_LastTriggeredYear = -1;
        private int m_LastTriggeredDay = -1;

        private bool m_DisplayActive = false;
        private float m_DisplayTimer = 0f;
        private float m_CurrentAlpha = 1f;

        private string m_CachedUpperText;
        private string m_CachedBottomText;

        private Rect m_CachedOverlayRect = Rect.zero;
        private Rect m_CachedUpperRect = Rect.zero;
        private Rect m_CachedLineRect = Rect.zero;
        private Rect m_CachedBottomRect = Rect.zero;

        private static readonly Regex FirstColorTagRegex = new Regex(@"<color=(?<color>[^<>]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DawnComponent(Game game)
            : base()
        {
            m_Game = game;
            m_FormatManager = new DawnFormatter(m_Game);
        }

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

            if (settings.ShowHighlight)
                Widgets.DrawHighlight(m_CachedOverlayRect);

            GUI.BeginGroup(m_CachedOverlayRect);

            string upperText = WithAlphaToRichText(m_CachedUpperText);
            settings.UpperTextStyle.Draw(m_CachedUpperRect, upperText, settings.Scale);

            Widgets.DrawBoxSolid(m_CachedLineRect, settings.LineColor.WithAlpha(m_CurrentAlpha));

            string bottomText = WithAlphaToRichText(m_CachedBottomText);
            settings.BottomTextStyle.Draw(m_CachedBottomRect, bottomText, settings.Scale);

            GUI.EndGroup();

            GUI.color = defaultColor;
        }

        private void TriggerDawnOfANewDay()
        {
            if (m_Game.CurrentMap == null)
                return;

            try
            {
                var settings = DawnMod.Settings;

                FormatContext context = m_FormatManager.CreateFormatContext(settings.StartsAtZero);
                m_CachedUpperText = context.FormatText(settings.UpperTextFormat);
                m_CachedBottomText = context.FormatText(settings.BottomTextFormat);

                settings.UpdateText();

                #region Cached Rects

                m_CachedUpperRect.size = settings.UpperTextStyle.TextGUIStyle.CalcSize(new GUIContent(m_CachedUpperText)) * 1.2f;
                m_CachedBottomRect.size = settings.BottomTextStyle.TextGUIStyle.CalcSize(new GUIContent(m_CachedBottomText)) * 1.2f;

                m_CachedOverlayRect.width = Mathf.Max(m_CachedUpperRect.width, m_CachedBottomRect.width);
                m_CachedOverlayRect.height = m_CachedUpperRect.height + m_CachedBottomRect.height + settings.LineThickness + (settings.LinePadding * settings.Scale * 3f);
                m_CachedOverlayRect.position = settings.Offset - (m_CachedOverlayRect.size / 2f);

                m_CachedUpperRect.x = (m_CachedOverlayRect.width - m_CachedUpperRect.size.x) / 2f;
                m_CachedUpperRect.y = 0f;

                float scaledLinePadding = settings.LinePadding * settings.Scale;
                m_CachedLineRect.width = m_CachedOverlayRect.width * (settings.LineWidthPercentage / 100f);
                m_CachedLineRect.height = settings.LineThickness;
                m_CachedLineRect.x = (m_CachedOverlayRect.width - m_CachedLineRect.width) / 2f;
                m_CachedLineRect.y = m_CachedUpperRect.size.y + scaledLinePadding;

                m_CachedBottomRect.x = (m_CachedOverlayRect.width - m_CachedBottomRect.size.x) / 2f;
                m_CachedBottomRect.y = m_CachedLineRect.yMax + scaledLinePadding;

                #endregion

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

        private string WithAlphaToRichText(string text)
        {
            if (text.NullOrEmpty())
                return text;

            return FirstColorTagRegex.Replace(text, match =>
            {
                string colorString = match.Groups["color"].Value.Trim();

                if (ColorUtility.TryParseHtmlString(colorString, out Color color))
                {
                    color.a *= m_CurrentAlpha;
                    return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
                }

                return match.Value;
            });
        }
    }
}
