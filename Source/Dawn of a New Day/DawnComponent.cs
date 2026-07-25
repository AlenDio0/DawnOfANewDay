using RimWorld;
using UnityEngine;
using Verse;
using static DawnNewDay.DawnSettings;

namespace DawnNewDay
{
    public class DawnComponent : GameComponent
    {
        private readonly Game m_Game;

        private int m_LastDay = -1;
        private bool m_DisplayActive = false;
        private float m_DisplayTimer = 0f;

        private string m_CachedDayText;
        private string m_CachedDateText;

        private Vector2 m_CachedDaySize;
        private Vector2 m_CachedDateSize;

        public DawnComponent(Game game)
            : base() => m_Game = game;

        private void TriggerDawnOfANewDay()
        {
            var settings = DawnMod.s_Settings;

            long absTicks = m_Game.tickManager.TicksAbs;
            Vector2 location = m_Game.World.grid.LongLatOf(m_Game.CurrentMap.Tile);

            int day = DaysRelative(settings.DayRelativeTo, absTicks, location.x) + (settings.StartsAtZero ? 0 : 1);
            m_CachedDayText = $"{settings.DayText} {day}";

            int year = GenDate.Year(absTicks, location.x);
            Quadrum quadrum = GenDate.Quadrum(absTicks, location.x);
            Season season = GenDate.Season(absTicks, location);
            m_CachedDateText = $"{settings.YearText} {year} | {quadrum.Label().ToUpper()} | {season.Label().ToUpper()}";

            m_CachedDaySize = settings.DayTextStyle.CalcSize(new GUIContent(m_CachedDayText));
            m_CachedDateSize = settings.DateTextStyle.CalcSize(new GUIContent(m_CachedDateText));

            m_DisplayTimer = settings.DisplayDurationSeconds;
            m_DisplayActive = true;
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (m_Game.CurrentMap == null)
                return;

            var settings = DawnMod.s_Settings;
            if (settings.ConsumeShowExample())
            {
                TriggerDawnOfANewDay();
                return;
            }

            if (!settings.Enabled)
                return;

            int currentDay = GenDate.DaysPassed;
            if (currentDay > m_LastDay)
            {
                m_LastDay = currentDay;
                if (settings.ShowEveryXDays <= 1 || m_LastDay % settings.ShowEveryXDays == 0)
                    TriggerDawnOfANewDay();
            }
        }

        public override void GameComponentOnGUI()
        {
            var settings = DawnMod.s_Settings;

            if (!m_DisplayActive)
                return;

            m_DisplayTimer -= Time.unscaledDeltaTime;
            if (m_DisplayTimer <= 0f)
            {
                m_DisplayActive = false;
                return;
            }

            Color defaultColor = GUI.color;

            float elapsedTime = settings.DisplayDurationSeconds - m_DisplayTimer;
            float alpha = 1f;
            if (elapsedTime < settings.FadeInDurationSeconds)
                alpha = elapsedTime / settings.FadeInDurationSeconds;
            else if (m_DisplayTimer < settings.FadeOutDurationSeconds)
                alpha = m_DisplayTimer / settings.FadeOutDurationSeconds;

            alpha = Mathf.Clamp01(alpha);
            GUI.color = new Color(1f, 1f, 1f, alpha);

            float rectWidth = Mathf.Max(m_CachedDaySize.x, m_CachedDateSize.x) * 1.2f;
            float rectHeight = m_CachedDaySize.y + m_CachedDateSize.y + settings.LineThickness + (settings.LinePadding * settings.Scale * 3f);
            Vector2 rectSize = new Vector2(rectWidth, rectHeight);

            Rect dawnLayoutRect = new Rect(settings.Offset - (rectSize / 2f), rectSize);

            if (settings.ShowHighlight)
                Widgets.DrawHighlight(dawnLayoutRect, alpha);

            GUILayout.BeginArea(dawnLayoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Rect dayRect = GUILayoutUtility.GetRect(new GUIContent(m_CachedDayText), settings.DayTextStyle);
            DrawText(dayRect, m_CachedDayText, settings.DayTextStyle, settings.DayOutlineColor, settings.DayOutlineThickness, alpha);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            float scaledLinePadding = settings.LinePadding * settings.Scale;

            GUILayout.Space(scaledLinePadding);

            float scaledLineThickness = settings.LineThickness;
            float lineWidth = rectSize.x * settings.LineWidthPercentage;
            float lineX = (rectSize.x - lineWidth) / 2f;

            Rect lineSpace = GUILayoutUtility.GetRect(rectSize.x, scaledLineThickness);
            Rect lineRect = new Rect(lineSpace.x + lineX, lineSpace.y, lineWidth, scaledLineThickness);
            Widgets.DrawBoxSolid(lineRect, new Color(0.6f, 0.6f, 0.6f, alpha));

            GUILayout.Space(scaledLinePadding);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Rect dateRect = GUILayoutUtility.GetRect(new GUIContent(m_CachedDateText), settings.DateTextStyle);
            DrawText(dateRect, m_CachedDateText, settings.DateTextStyle, settings.DateOutlineColor, settings.DateOutlineThickness, alpha);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUI.color = defaultColor;
        }

        private void DrawText(Rect rect, string text, GUIStyle style, Color outlineColor, float outlineThickness, float alpha)
        {
            Color originalColor = style.normal.textColor;

            originalColor.a = alpha;
            outlineColor.a = alpha;

            if (outlineThickness > 0)
            {
                style.normal.textColor = outlineColor;

                float step = outlineThickness / 10f;
                for (float x = -outlineThickness; x <= outlineThickness; x += step)
                {
                    for (float y = -outlineThickness; y <= outlineThickness; y += step)
                    {
                        Vector2 offset = new Vector2(x, y);
                        GUI.Label(new Rect(rect.position + offset, rect.size), text, style);
                    }
                }
            }

            style.normal.textColor = originalColor;
            GUI.Label(rect, text, style);
        }

        private int DaysRelative(DayRelative dayRelative, long absTicks, float longitude)
        {
            switch (dayRelative)
            {
                case DayRelative.None:
                    return m_LastDay;
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
