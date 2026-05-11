#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Pool;

namespace CatCode.StatefulEffects
{
    public static class ShowHideAsyncExtensions
    {
        private readonly static ObjectPool<IShowHideEqualityCondition> s_conditionPool;

        static ShowHideAsyncExtensions()
        {
            s_conditionPool = new ObjectPool<IShowHideEqualityCondition>(
                createFunc: () => new(),
                collectionCheck: false);
        }

        public static async UniTask WaitStateAsync(this IShowHide showHide, ShowHideState targetState, CancellationToken token)
        {
            using var handle = s_conditionPool.Get(out var condition);
            condition.Init(targetState);
            await showHide.State.WaitAsync(condition, true, token);
        }

        public static UniTask ShowAsync(this IShowHide showHide, CancellationToken token)
        {
            showHide.Show();
            return showHide.WaitStateAsync(ShowHideState.Shown, token);
        }

        public static UniTask HideAsync(this IShowHide showHide, CancellationToken token)
        {
            showHide.Hide();
            return showHide.WaitStateAsync(ShowHideState.Hidden, token);
        }

        public static UniTask WaitShowAsync(this IShowHide showHide, CancellationToken token)
            => showHide.WaitStateAsync(ShowHideState.Shown, token);

        public static UniTask WaitHideAsync(this IShowHide showHide, CancellationToken token)
            => showHide.WaitStateAsync(ShowHideState.Hidden, token);
    }
}

#endif