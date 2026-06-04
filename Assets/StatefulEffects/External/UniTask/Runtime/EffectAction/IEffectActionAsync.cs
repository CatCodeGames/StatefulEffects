#if STATEFULEFFECTS_UNITASK_SUPPORT

using Cysharp.Threading.Tasks;

namespace CatCode.StatefulEffects
{

    public interface IEffectActionAsync : IEffectAction
    {
        UniTask<StateTransitionResult> PlayAsync();
    }
}

#endif