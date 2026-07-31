using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DawnNewDay.Utils
{
    public class Dialog_ChooseInterface : Window
    {
        private readonly string m_Current = null;
        private readonly Action<string> m_OnChoose;

        private readonly List<string> m_Values;

        private string m_SearchBuffer = "";

        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public virtual List<string> InitalValues => new List<string>();
        public virtual string HeaderLabel => null;
        public virtual string DefaultValue => null;
        public virtual IEnumerable<string> WhereShowable(IEnumerable<string> list) => list;

        public Dialog_ChooseInterface(Action<string> onChoose, string current = null)
        {
            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            draggable = true;

            m_Current = current;
            m_OnChoose = onChoose;

            if (m_Values == null)
                m_Values = InitalValues;
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

                ShowList(listing);
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
            if (HeaderLabel != null)
            {
                GameFont defaultFont = Text.Font;
                Text.Font = GameFont.Medium;

                listing.Label($"{HeaderLabel} ({m_Current.Fallback(DefaultValue)})");

                Text.Font = defaultFont;
            }

            listing.LabeledTextEntry(DawnData.Label_Search, ref m_SearchBuffer, 0.1f, 0.9f);

            if (DefaultValue != null)
            {
                listing.Gap();

                if (listing.ButtonText(DefaultValue))
                {
                    m_OnChoose(DefaultValue);
                    Close();
                }
            }
        }

        private void ShowList(Listing_Standard listing)
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

                if (!m_Current.NullOrEmpty() && m_Current == fontName)
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
            return WhereShowable(m_Values.Where(value => m_SearchBuffer.NullOrEmpty() || value.ToUpper().Contains(m_SearchBuffer.ToUpper())));
        }
    }
}
