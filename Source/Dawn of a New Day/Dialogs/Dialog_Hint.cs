using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DawnNewDay.Dialogs
{
    public class Dialog_Hint : Window
    {
        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public string HeaderLabel { get; }
        public Dictionary<string, string> Hints { get; }

        public Dialog_Hint(string header, Dictionary<string, string> hints)
        {
            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            draggable = true;

            HeaderLabel = header;
            Hints = hints;
        }

        public override Vector2 InitialSize => new(700f, 600f);

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

                ShowHints(listing);
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into Dialog_Hint.DoWindowContents(canva: {canva})\nException: {exception}");
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
            if (HeaderLabel == null)
                return;

            GameFont defaultFont = Text.Font;
            Text.Font = GameFont.Medium;

            listing.Label(HeaderLabel);

            Text.Font = defaultFont;
        }

        private void ShowHints(Listing_Standard listing)
        {
            foreach ((string format, string description) in Hints)
            {

                listing.GapLine();
                Rect rowRect = listing.GetRect(40f);

                float iconSize = rowRect.height;

                Rect copyRect = new(rowRect.xMax - iconSize, rowRect.y, iconSize, iconSize);
                Rect hintRect = new(rowRect.x, rowRect.y, rowRect.xMax - iconSize, rowRect.height);

                TextAnchor defaultAnchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;

                string displayFormat = format.Replace("<", "<\u200B");
                Widgets.Label(hintRect, $"<b>{displayFormat}</b>\n~ {description}");

                Text.Anchor = defaultAnchor;

                GUI.DrawTexture(copyRect, TexButton.Copy);

                if (Mouse.IsOver(rowRect))
                    Widgets.DrawHighlight(rowRect);

                if (Widgets.ButtonInvisible(rowRect))
                {
                    GUIUtility.systemCopyBuffer = format;
                    Messages.Message("FormatText copied into the clipboard!", MessageTypeDefOf.NeutralEvent, false);

                    Close();
                    break;
                }
            }
        }
    }
}
