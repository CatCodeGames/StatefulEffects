#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;

namespace CatCode.StatefulEffects
{
    public sealed class IShowHideEqualityCondition : ICondition<ShowHideState>
    {
        private ShowHideState _targetState;

        public void Init(ShowHideState targetState)
            => _targetState = targetState;

        public bool Check(ShowHideState value)
            => value == _targetState;
    }
}

#endif