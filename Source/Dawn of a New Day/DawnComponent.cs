using DawnNewDay.Compatibility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DawnNewDay
{
    public class DawnComponent : GameComponent
    {
        private readonly Game m_Game;
        private readonly DawnFormatter m_Formatter;

        private int m_LastTriggeredYear = -1;
        private int m_LastTriggeredDay = -1;

        private bool m_DisplayActive = false;
        private float m_DisplayTimer = 0f;
        private float m_CurrentAlpha = 1f;

        private readonly Dictionary<DawnText, string> m_CachedLabels = [];

        private readonly List<Rect> m_CachedRects = [];
        private Rect m_CachedOverlayRect = Rect.zero;

        private static readonly Regex m_ColorTagRegex = new(@"<color=(?<color>[^<>]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DawnComponent(Game game)
            : base()
        {
            m_Game = game;
            m_Formatter = new DawnFormatter(m_Game);
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
                TriggerDawnOverlay();
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
                TriggerDawnOverlay();
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

            for (int i = 0; i < settings.OverlayWidgets.Count; i++)
            {
                IDawnWidget widget = settings.OverlayWidgets.ElementAt(i);
                Rect cachedRect = m_CachedRects.ElementAtOrDefault(i);

                switch (widget)
                {
                    case DawnText text:
                        string cachedLabel = m_CachedLabels.TryGetValue(text, "");
                        text.Draw(cachedRect, FinalizeLabel(cachedLabel), settings.Scale);
                        break;
                    case DawnSeparator separator:
                        separator.Draw(separator.CalcDrawRect(cachedRect, settings.Scale), m_CurrentAlpha);
                        break;

                    default:
                        break;
                }
            }

            // TODO: ModernNotification Compatibility
            //if (ModernNotifications.Present)
            //{
            //    string reminderText = WithAlphaToRichText(m_MN_CachedReminderText);
            //    settings.MN_Reminder.TextStyle.Draw(RetrieveTextRect(Text.MN_Reminder), reminderText, settings.Scale);

            //    string occasionText = WithAlphaToRichText(m_MN_CachedOccasionText);
            //    settings.MN_Occasion.TextStyle.Draw(RetrieveTextRect(Text.MN_Occasion), occasionText, settings.Scale);
            //}

            GUI.EndGroup();

            GUI.color = defaultColor;
        }

        private void TriggerDawnOverlay()
        {
            if (m_Game.CurrentMap == null)
                return;

            try
            {
                UpdateCachedLabels();
                UpdateCachedRects();

                StartSound();

                m_DisplayTimer = DawnMod.Settings.DisplayDurationSeconds;
                m_DisplayActive = true;
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
        }

        private static void StartSound()
        {
            var settings = DawnMod.Settings;

            if (settings.Sound == null)
                return;

            SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo());
            soundInfo.volumeFactor = settings.SoundVolume;
            soundInfo.pitchFactor = settings.SoundPitch;
            soundInfo.forcedPlayOnCamera = true;
            settings.Sound.PlayOneShot(soundInfo);
        }

        private void UpdateCachedLabels()
        {
            var settings = DawnMod.Settings;

            FormatContext context = m_Formatter.CreateFormatContext(settings.StartsAtZero);

            m_CachedLabels.Clear();
            for (int i = 0; i < settings.OverlayWidgets.Count; i++)
            {
                var widget = settings.OverlayWidgets[i];
                if (widget is DawnText text)
                {
                    string format = text.Format;
                    m_CachedLabels.Add(text, context.FormatText(format, i, settings.Scale));
                }
            }

            settings.UpdateText();

            //m_MN_CachedReminderText = "";
            //m_MN_CachedOccasionText = "";
            //if (ModernNotifications.Present)
            //{
            //    int GetTimeRemainingDay(ModernNotificationUtility.YearTime time) => Mathf.CeilToInt(context.TimeRemainingDay(context.TimeRemainingHour(time)));

            //    if (settings.MN_Reminder.AddText)
            //    {
            //        var reminder = context.NextReminder;
            //        if (reminder.IsValid && GetTimeRemainingDay(reminder.Time) <= settings.MN_Reminder.MaximumDays)
            //            m_MN_CachedReminderText = context.FormatText(settings.MN_Reminder.TextFormat);
            //    }
            //    if (settings.MN_Occasion.AddText)
            //    {
            //        var occasion = context.NextOccasion;
            //        if (occasion.IsValid && GetTimeRemainingDay(occasion.Time) <= settings.MN_Occasion.MaximumDays)
            //            m_MN_CachedOccasionText = context.FormatText(settings.MN_Occasion.TextFormat);

            //    }
            //}
        }

        private void UpdateCachedRects()
        {
            var settings = DawnMod.Settings;

            List<(Rect rect, bool isText)> tempCachedRects = [];
            for (int i = 0; i < settings.OverlayWidgets.Count; i++)
            {
                Rect lastRect = i > 0 ? tempCachedRects[i - 1].rect : Rect.zero;
                Vector2 topLeft = new(0f, lastRect.yMax);

                IDawnWidget widget = settings.OverlayWidgets[i];
                switch (widget)
                {
                    case DawnText text:
                        string cachedLabel = m_CachedLabels.TryGetValue(text, "");

                        Rect textRect = new(topLeft, text.CalcSize(cachedLabel));
                        tempCachedRects.Add(new(textRect, true));
                        break;
                    case DawnSeparator separator:
                        tempCachedRects.Add(new(separator.CalcFullRect(topLeft.y, 0f, settings.Scale), false));
                        break;

                    default:
                        break;
                }
            }

            float maxWidth = Mathf.Max([.. tempCachedRects.Select(cached => cached.rect.width)]) * 1.2f;
            float totalHeight = tempCachedRects.Sum(cached => cached.rect.height);

            m_CachedOverlayRect.size = new(maxWidth, totalHeight);
            m_CachedOverlayRect.position = settings.Offset - (m_CachedOverlayRect.size / 2f);

            for (int i = 0; i < tempCachedRects.Count; i++)
            {
                Rect lastRect = i > 0 ? tempCachedRects[i - 1].rect : Rect.zero;

                if (settings.OverlayWidgets[i] is DawnSeparator separator)
                    tempCachedRects[i] = new(separator.CalcFullRect(lastRect.yMax, maxWidth, settings.Scale), false);
            }

            m_CachedRects.Clear();
            foreach (var (rect, isText) in tempCachedRects)
            {
                Rect finalRect = rect;
                if (isText)
                    finalRect = new(rect.position, new(maxWidth, rect.height));

                m_CachedRects.Add(finalRect);
            }

            //if (ModernNotifications.Present)
            //{
            //    RetrieveTextRect(Text.MN_Reminder).size = CalcTextSize(settings.MN_Reminder.TextStyle, m_MN_CachedReminderText);
            //    RetrieveTextRect(Text.MN_Occasion).size = CalcTextSize(settings.MN_Occasion.TextStyle, m_MN_CachedOccasionText);
            //}
            //else
            //{
            //    RetrieveTextRect(Text.MN_Reminder).size = Vector2.zero;
            //    RetrieveTextRect(Text.MN_Occasion).size = Vector2.zero;
            //}

            //RetrieveTextRect(Text.Upper).y = 0f;

            //RetrieveTextRect(Text.Bottom).y = m_CachedLineRect.yMax + scaledLinePadding;
            //RetrieveTextRect(Text.Subtitle).y = RetrieveTextRect(Text.Bottom).yMax + scaledSubtitleGap;

            //if (ModernNotifications.Present)
            //{
            //    RetrieveTextRect(Text.MN_Reminder).y = RetrieveTextRect(Text.Subtitle).yMax;
            //    RetrieveTextRect(Text.MN_Occasion).y = RetrieveTextRect(Text.MN_Reminder).yMax;
            //}
        }

        private string FinalizeLabel(string label) => WithAlphaToRichText(label);

        private string WithAlphaToRichText(string text)
        {
            if (text.NullOrEmpty())
                return text;

            return m_ColorTagRegex.Replace(text, match =>
            {
                string colorString = match.Groups["color"].Value.Trim();

                if (ColorUtility.TryParseHtmlString(colorString, out Color color))
                    return $"<color=#{ColorUtility.ToHtmlStringRGBA(color.WithAlpha(color.a * m_CurrentAlpha))}>";

                return match.Value;
            });
        }
    }
}
