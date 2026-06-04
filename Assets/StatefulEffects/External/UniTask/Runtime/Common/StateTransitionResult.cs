#if STATEFULEFFECTS_UNITASK_SUPPORT

namespace CatCode.StatefulEffects
{
    public enum StateTransitionResult
    {
        AlreadyInState,
        ForceCompleted,
        Completed,
        Interrupted,
    }
}

#endif