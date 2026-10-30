using System;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnMod : Mod
    {
        public static DawnSettings Settings;
        public static ModContentPack ModContent;

        private bool m_OnceSettings = true;
        private bool m_EverySettings = true;

        private int m_SkipFrameTimer = 0;

        public DawnMod(ModContentPack content)
            : base(content)
        {
            Settings = GetSettings<DawnSettings>();
            ModContent = content;
        }

        public override void WriteSettings()
        {
            base.WriteSettings();

            m_EverySettings = true;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (Settings == null)
            {
                DawnData.Error("Not initialized correctly: DawnSettings is null");
                return;
            }

            if (m_OnceSettings)
            {
                Settings.OnceOpen();
                m_OnceSettings = false;
            }

            if (m_EverySettings)
            {
                if (m_SkipFrameTimer > 10)
                {
                    Settings.EveryOpen();

                    m_EverySettings = false;
                    m_SkipFrameTimer = 0;
                }
                else
                    m_SkipFrameTimer++;
            }

            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory() => DawnData.ModName;
    }
}
