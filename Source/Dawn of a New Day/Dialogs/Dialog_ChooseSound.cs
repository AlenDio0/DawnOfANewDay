using DawnNewDay.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DawnNewDay.Dialogs
{
    public class Dialog_ChooseSound : Dialog_ChooseInterface
    {
        private static List<string> m_CachedSoundNames;
        private static List<string> SoundNames
        {
            get
            {
                if (m_CachedSoundNames == null)
                {
                    try
                    {
                        m_CachedSoundNames = DefDatabase<SoundDef>.AllDefsListForReading
                            .Where(soundDef => soundDef.subSounds.All(subSoundDef => !subSoundDef.sustainLoop))
                            .ToList().ConvertAll(soundDef => soundDef.defName);
                    }
                    finally
                    {
                        if (m_CachedSoundNames == null)
                            m_CachedSoundNames = new List<string>();
                    }
                }

                return m_CachedSoundNames;
            }
        }

        public override List<string> InitalValues => new List<string>(SoundNames);

        public override string HeaderLabel => DawnData.SettingsLabel_Sound;

        public override string DefaultValue => DawnData.DefaultSoundDefName;

        public Dialog_ChooseSound(Action<string> onChoose, string current = null)
            : base(onChoose, current)
        {
        }
    }
}
