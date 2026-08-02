using DawnNewDay.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DawnNewDay.Dialogs
{
    public class Dialog_ChooseFontFamily : Dialog_ChooseInterface
    {
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

        public override List<string> InitalValues => new List<string>(OSFontNames);

        public override string HeaderLabel => DawnTranslation.Label_FontFamily;

        public override string DefaultValue => DawnTranslation.Label_DefaultFont;

        public override IEnumerable<string> WhereShowable(IEnumerable<string> list) =>
            list.Where(fontName => !fontName.ToUpper().Contains("BOLD"))
                .Where(fontName => !fontName.ToUpper().Contains("ITALIC"));

        public Dialog_ChooseFontFamily(Action<string> onChoose, string current = null)
            : base(onChoose, current)
        {
        }
    }
}
