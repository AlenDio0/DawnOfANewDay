using DawnNewDay.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DawnNewDay.Dialogs
{
    public class Dialog_ChooseSound(Action<string> onChoose, string current = null) : Dialog_ChooseInterface(onChoose, current)
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
                        m_CachedSoundNames = [.. DefDatabase<SoundDef>.AllDefsListForReading
                            .Where(soundDef => !soundDef.sustain)
                            .Where(soundDef => !soundDef.subSounds.NullOrEmpty() && soundDef.subSounds.All(subSoundDef => !subSoundDef.sustainLoop))
                            .Select(soundDef => soundDef.defName)];
                    }
                    finally
                    {
                        m_CachedSoundNames ??= [];
                    }
                }

                return m_CachedSoundNames;
            }
        }

        public override List<string> InitialValues => SoundNames;

        public override string HeaderLabel => DawnTranslation.Label_Sound;

        public override string DefaultValue => DawnData.DefaultSoundDefName;
    }
}
