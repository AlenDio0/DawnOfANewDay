using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace DawnNewDay.Utils
{
    public class Dialog_ChooseInterface : Window
    {
        public string Current { get; }
        public Action<string> OnChoose { get; }

        private string m_SearchBuffer = "";

        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public virtual List<string> InitialValues => [];
        public virtual string HeaderLabel => null;
        public virtual string DefaultValue => null;
        public virtual IEnumerable<string> WhereShowable(IEnumerable<string> list) => list;

        public Dialog_ChooseInterface(Action<string> onChoose, string current = null)
        {
            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            draggable = true;

            Current = current;
            OnChoose = onChoose;
        }

        public override Vector2 InitialSize => new(600f, 800f);

        public override void DoWindowContents(Rect canva)
        {
            Listing_Standard listing = new() { maxOneColumn = true };
            listing.Begin(canva);

            try
            {
                ShowHeader(listing);
                float headerHeight = listing.CurHeight;

                listing.End();

                Rect outRect = new(canva.x, canva.y + headerHeight, canva.width, canva.height - headerHeight);
                Rect viewRect = new(0f, 0f, outRect.width - 32f, Mathf.Max(outRect.height, m_CachedScrollViewHeight));
                Widgets.BeginScrollView(outRect, ref m_ScrollPosition, viewRect);

                listing.Begin(viewRect);

                ShowList(listing);
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
            finally
            {
                m_CachedScrollViewHeight = listing.CurHeight;

                listing.End();

                Widgets.EndScrollView();
            }
        }

        private void ShowHeader(Listing_Standard listing)
        {
            if (HeaderLabel != null)
            {
                GameFont defaultFont = Text.Font;
                Text.Font = GameFont.Medium;

                listing.Label($"{HeaderLabel} ({Current.Fallback(DefaultValue)})");

                Text.Font = defaultFont;
            }

            Rect searchRect = listing.GetRect(DawnData.TinyRowHeight);
            float iconSize = searchRect.height;

            Rect iconRect = new(searchRect.position, new(iconSize, iconSize));
            GUI.DrawTexture(iconRect.ContractedBy(4f), TexButton.Search);

            Rect fieldRect = new(iconRect.xMax, searchRect.y, searchRect.width - iconSize, searchRect.height);
            m_SearchBuffer = Widgets.TextField(fieldRect.MiddlePart(0.9f, 1f), m_SearchBuffer);

            if (DefaultValue != null)
            {
                listing.Gap();

                if (listing.ButtonText(DefaultValue))
                {
                    OnChoose(DefaultValue);
                    Close();
                }
            }
        }

        private void ShowList(Listing_Standard listing)
        {
            IEnumerable<string> showableValues = ShowableValues;
            foreach (string value in showableValues)
            {
                listing.GapLine();
                Rect rowRect = listing.GetRect(DawnData.TinyRowHeight);

                TextAnchor defaultAnchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;

                Widgets.Label(rowRect, value);

                Text.Anchor = defaultAnchor;

                if (!Current.NullOrEmpty() && Current == value)
                    Widgets.DrawBoxSolid(rowRect, Color.green.WithAlpha(0.25f));

                if (Mouse.IsOver(rowRect))
                    Widgets.DrawHighlight(rowRect);

                if (Widgets.ButtonInvisible(rowRect, true))
                {
                    OnChoose?.Invoke(value);

                    Close();
                    break;
                }
            }
        }

        private IEnumerable<string> ShowableValues => WhereShowable(InitialValues.Where(value => m_SearchBuffer.NullOrEmpty() || value.ToUpper().Contains(m_SearchBuffer.ToUpper())));
    }
}
