using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class Dialog_ChooseFontFamily : Window
    {
        private readonly string m_CurrentFontName;
        private readonly Action<string> m_OnChoose;

        private static string[] m_CachedOSFontNames;
        private static string[] OSFontNames
        {
            get
            {
                if (m_CachedOSFontNames == null)
                {
                    try
                    {
                        m_CachedOSFontNames = Font.GetOSInstalledFontNames();
                    }
                    finally
                    {
                        if (m_CachedOSFontNames == null)
                            m_CachedOSFontNames = new string[0];
                    }
                }

                return m_CachedOSFontNames;
            }
        }

        private readonly List<string> m_FontNames;

        private string m_SearchBuffer = "";

        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public Dialog_ChooseFontFamily(string currentFontName, Action<string> onChoose)
        {
            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            draggable = true;

            m_CurrentFontName = currentFontName;
            m_OnChoose = onChoose;

            if (m_FontNames == null)
                m_FontNames = new List<string>(OSFontNames);
        }

        public override Vector2 InitialSize => new Vector2(600f, 800f);

        public override void DoWindowContents(Rect canva)
        {
            try
            {
                Listing_Standard listing = new Listing_Standard { maxOneColumn = true };
                listing.Begin(canva);

                ShowHeader(listing);
                float headerHeight = listing.CurHeight;

                listing.End();

                Rect outRect = new Rect(canva.x, canva.y + headerHeight, canva.width, canva.height - headerHeight);
                Rect viewRect = new Rect(0f, 0f, outRect.width - 32f, Mathf.Max(outRect.height, m_CachedScrollViewHeight));
                Widgets.BeginScrollView(outRect, ref m_ScrollPosition, viewRect);

                listing.Begin(viewRect);

                ShowFontList(listing);
                m_CachedScrollViewHeight = listing.CurHeight;

                listing.End();

                Widgets.EndScrollView();
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into Dialog_ChooseFontFamily.DoWindowContents(canva: {canva})\nException: {exception}");
            }
        }

        private void ShowHeader(Listing_Standard listing)
        {
            GameFont defaultFont = Text.Font;

            Text.Font = GameFont.Medium;

            listing.Label($"{DawnData.SettingsLabel_FontFamily} ({m_CurrentFontName.Fallback(DawnData.SettingsLabel_DefaultFont)})");

            Text.Font = defaultFont;

            listing.LabeledTextEntry(DawnData.SettingsLabel_Search, ref m_SearchBuffer, 0.1f, 0.9f);

            listing.Gap();

            if (listing.ButtonText(DawnData.SettingsLabel_DefaultFont))
            {
                m_OnChoose("");
                Close();
            }
        }

        private void ShowFontList(Listing_Standard listing)
        {
            IEnumerable<string> showableFontNames = GetShowableFontNames();
            foreach (string fontName in showableFontNames)
            {
                listing.GapLine();
                Rect fontRect = listing.GetRect(30f);

                TextAnchor defaultAnchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;

                Widgets.Label(new Rect(0f, fontRect.y, fontRect.width - 0f, fontRect.height), fontName);

                Text.Anchor = defaultAnchor;

                if (!m_CurrentFontName.NullOrEmpty() && m_CurrentFontName == fontName)
                    Widgets.DrawBoxSolid(fontRect, Color.green.WithAlpha(0.25f));

                if (Mouse.IsOver(fontRect))
                    Widgets.DrawHighlight(fontRect);

                if (Widgets.ButtonInvisible(fontRect, true))
                {
                    m_OnChoose?.Invoke(fontName);

                    Close();
                    break;
                }
            }
        }

        private IEnumerable<string> GetShowableFontNames()
        {
            return m_FontNames
                .Where(fontName => m_SearchBuffer.NullOrEmpty() || fontName.ToUpper().Contains(m_SearchBuffer.ToUpper()))
                .Where(fontName => !fontName.ToUpper().Contains("BOLD"))
                .Where(fontName => !fontName.ToUpper().Contains("ITALIC"));
        }
    }
}
