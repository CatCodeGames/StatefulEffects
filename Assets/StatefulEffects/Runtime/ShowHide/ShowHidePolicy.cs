using System;

namespace CatCode.StatefulEffects
{

    [Serializable]
    public struct ShowHidePolicy
    {
        public TransitionPolicy Show;
        public TransitionPolicy Hide;

        public static ShowHidePolicy Default => default;
    }
}
