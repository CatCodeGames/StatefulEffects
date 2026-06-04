#if STATEFULEFFECTS_UNITASK_SUPPORT

using Cysharp.Threading.Tasks;

namespace CatCode.StatefulEffects
{
    public interface IShowHideAsync : IShowHide
    {
        UniTask<StateTransitionResult> ShowAsync();
        UniTask<StateTransitionResult> HideAsync();
    }
}

#endif