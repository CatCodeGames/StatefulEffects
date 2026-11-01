using System;

namespace CatCode.StatefulEffects
{
    [Serializable]
    public struct TransitionPolicy
    {
        public InterruptResolution OnInterrupt;
        public RepeatResolution OnRepeat;
    }
}
