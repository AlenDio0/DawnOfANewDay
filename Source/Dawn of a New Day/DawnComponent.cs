using DawnNewDay.Compatibility;
using RimWorld;
using System;
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
        private readonly DawnFormatter m_FormatManager;

        private int m_LastTriggeredYear = -1;
        private int m_LastTriggeredDay = -1;

        private bool m_DisplayActive = false;
        private float m_DisplayTimer = 0f;
        private float m_CurrentAlpha = 1f;

        private string m_CachedUpperText;
        private string m_CachedBottomText;
        private string m_CachedSubtitleText;

        private enum Text
        {
            Upper = 0, 
            Bottom, 
            Subtitle, 
            MN_Reminder, 
            MN_Occasion,

            Length,
        }

        private Rect m_CachedOverlayRect = Rect.zero;
        private Rect m_CachedLineRect = Rect.zero;
        private readonly Rect[] m_CachedTextRects = new Rect[(int)Text.Length];

        #region Compatibility Section

        #region Modern Notifications Section

        private string m_MN_CachedReminderText = "";
        private string m_MN_CachedOccasionText = "";

        #endregion

        #endregion

        private static readonly Regex FirstColorTagRegex = new(@"<color=(?<color>[^<>]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

            string upperText = WithAlphaToRichText(m_CachedUpperText);
            settings.UpperTextStyle.Draw(RetrieveTextRect(Text.Upper), upperText, settings.Scale);

            Widgets.DrawBoxSolid(m_CachedLineRect, settings.LineColor.WithAlpha(m_CurrentAlpha));

            string bottomText = WithAlphaToRichText(m_CachedBottomText);
            settings.BottomTextStyle.Draw(RetrieveTextRect(Text.Bottom), bottomText, settings.Scale);

            string subtitleText = WithAlphaToRichText(m_CachedSubtitleText);
            settings.SubtitleTextStyle.Draw(RetrieveTextRect(Text.Subtitle), subtitleText, settings.Scale);

            if (ModernNotifications.Present)
            {
                string reminderText = WithAlphaToRichText(m_MN_CachedReminderText);
                settings.MN_Reminder.TextStyle.Draw(RetrieveTextRect(Text.MN_Reminder), reminderText, settings.Scale);

                string occasionText = WithAlphaToRichText(m_MN_CachedOccasionText);
                settings.MN_Occasion.TextStyle.Draw(RetrieveTextRect(Text.MN_Occasion), occasionText, settings.Scale);
            }

            GUI.EndGroup();

            GUI.color = defaultColor;
        }

        private void TriggerDawnOverlay()
        {
            if (m_Game.CurrentMap == null)
                return;

            try
            {
                var settings = DawnMod.Settings;

                UpdateCachedTextFormat();
                UpdateCachedTextRect();

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
                DawnData.Exception(exception);
            }
        }

        private void UpdateCachedTextFormat()
        {
            var settings = DawnMod.Settings;

            FormatContext context = m_FormatManager.CreateFormatContext(settings.StartsAtZero);
            m_CachedUpperText = context.FormatText(settings.UpperTextFormat);
            m_CachedBottomText = context.FormatText(settings.BottomTextFormat);
            m_CachedSubtitleText = context.FormatText(settings.SubtitleTextFormat);

            if (ModernNotifications.Present)
            {
                m_MN_CachedReminderText = "";
                m_MN_CachedOccasionText = "";
                
                int GetTimeRemainingDay(ModernNotificationUtility.YearTime time) => Mathf.CeilToInt(context.TimeRemainingDay(context.TimeRemainingHour(time)));

                if (settings.MN_Reminder.AddText)
                {
                    var reminder = context.NextReminder;
                    if (reminder.IsValid && GetTimeRemainingDay(reminder.Time) <= settings.MN_Reminder.MaximumDays)
                        m_MN_CachedReminderText = context.FormatText(settings.MN_Reminder.TextFormat);
                }
                if (settings.MN_Occasion.AddText)
                {
                    var occasion = context.NextOccasion;
                    if (occasion.IsValid && GetTimeRemainingDay(occasion.Time) <= settings.MN_Occasion.MaximumDays)
                        m_MN_CachedOccasionText = context.FormatText(settings.MN_Occasion.TextFormat);

                }
            }

            settings.UpdateText();
        }

        private void UpdateCachedTextRect()
        {
            var settings = DawnMod.Settings;

            const float cTextPaddingFactor = 1.2f;
            float scaledSubtitleGap = !m_CachedSubtitleText.NullOrEmpty() ? settings.SubtitleGap * settings.Scale : 0f;
            float scaledLinePadding = settings.LinePadding * settings.Scale;

            RetrieveTextRect(Text.Upper).size = CalcTextSize(settings.UpperTextStyle, m_CachedUpperText);
            RetrieveTextRect(Text.Bottom).size = CalcTextSize(settings.BottomTextStyle, m_CachedBottomText);
            RetrieveTextRect(Text.Subtitle).size = CalcTextSize(settings.SubtitleTextStyle, m_CachedSubtitleText);

            if (ModernNotifications.Present)
            {
                RetrieveTextRect(Text.MN_Reminder).size = CalcTextSize(settings.MN_Reminder.TextStyle, m_MN_CachedReminderText);
                RetrieveTextRect(Text.MN_Occasion).size = CalcTextSize(settings.MN_Occasion.TextStyle, m_MN_CachedOccasionText);
            }

            float textWidth = Mathf.Max([.. m_CachedTextRects.Select(rect => rect.width)]) * cTextPaddingFactor;

            float totalTextHeight = 0f;
            for (int i = 0; i < m_CachedTextRects.Length; i++)
            {
                ref Rect rect = ref m_CachedTextRects[i];

                rect.height *= cTextPaddingFactor;
                rect.width = textWidth;

                totalTextHeight += rect.height;
            }

            m_CachedOverlayRect.width = textWidth;
            m_CachedOverlayRect.height = totalTextHeight + settings.LineThickness + (scaledLinePadding * 3f) + scaledSubtitleGap;
            m_CachedOverlayRect.position = settings.Offset - (m_CachedOverlayRect.size / 2f);

            RetrieveTextRect(Text.Upper).y = 0f;

            m_CachedLineRect.width = m_CachedOverlayRect.width * (settings.LineWidthPercentage / 100f);
            m_CachedLineRect.height = settings.LineThickness;

            m_CachedLineRect.x = (m_CachedOverlayRect.width - m_CachedLineRect.width) / 2f;
            m_CachedLineRect.y = RetrieveTextRect(Text.Upper).yMax + scaledLinePadding;

            RetrieveTextRect(Text.Bottom).y = m_CachedLineRect.yMax + scaledLinePadding;
            RetrieveTextRect(Text.Subtitle).y = RetrieveTextRect(Text.Bottom).yMax + scaledSubtitleGap;

            if (ModernNotifications.Present)
            {
                RetrieveTextRect(Text.MN_Reminder).y = RetrieveTextRect(Text.Subtitle).yMax;
                RetrieveTextRect(Text.MN_Occasion).y = RetrieveTextRect(Text.MN_Reminder).yMax;
            }
        }

        private ref Rect RetrieveTextRect(Text text) => ref m_CachedTextRects[(int)text];

        private Vector2 CalcTextSize(DawnTextStyle style, string text)
        {
            if (text.NullOrEmpty())
                return Vector2.zero;

            return style.TextGUIStyle.CalcSize(new GUIContent(text));
        }

        private string WithAlphaToRichText(string text)
        {
            if (text.NullOrEmpty())
                return text;

            return FirstColorTagRegex.Replace(text, match =>
            {
                string colorString = match.Groups["color"].Value.Trim();

                if (ColorUtility.TryParseHtmlString(colorString, out Color color))
                    return $"<color=#{ColorUtility.ToHtmlStringRGBA(color.WithAlpha(color.a * m_CurrentAlpha))}>";

                return match.Value;
            });
        }
    }
}
