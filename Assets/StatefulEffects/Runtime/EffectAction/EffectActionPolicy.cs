using System;

namespace CatCode.StatefulEffects
{
    [Serializable]
    public struct EffectActionPolicy
    {
        public InterruptResolution OnRepeat;

        public static EffectActionPolicy Default => default;
    }
}
