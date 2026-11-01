using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CatCode.StatefulEffects
{
    public static class ShowHideAsyncExtensions
    {
        public static UniTask<ShowHideResult> ShowAsync(this IShowHide showHide, CancellationToken token = default)
            => showHide.ShowAsync(ShowHideWaitMode.FirstChange, token);

        public static UniTask<ShowHideResult> ShowAsync(this IShowHide showHide, ShowHideWaitMode mode, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return UniTask.FromCanceled<ShowHideResult>(token);

            var before = showHide.State;
            showHide.Show();

            switch (showHide.State)
            {
                case ShowHideState.None:
                    return UniTask.FromResult(before == ShowHideState.None ? ShowHideResult.Rejected : ShowHideResult.Stopped);
                case ShowHideState.Hidden:
                    return UniTask.FromResult(ShowHideResult.Rejected);
                case ShowHideState.Showing:
                    return ShowHideStatePromise.WaitAsync(showHide, ShowHideState.Shown, ShowHideState.Showing, mode, token);
                case ShowHideState.Shown:
                    return UniTask.FromResult(before == ShowHideState.Shown ? ShowHideResult.AlreadyInState : ShowHideResult.Completed);
                case ShowHideState.Hiding:
                    return UniTask.FromResult(ShowHideResult.Rejected);
            }

            throw new InvalidOperationException($"Unexpected state {showHide.State}.");
        }

        public static UniTask<ShowHideResult> HideAsync(this IShowHide showHide, CancellationToken token = default)
            => showHide.HideAsync(ShowHideWaitMode.FirstChange, token);

        public static UniTask<ShowHideResult> HideAsync(this IShowHide showHide, ShowHideWaitMode mode, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return UniTask.FromCanceled<ShowHideResult>(token);

            var before = showHide.State;
            showHide.Hide();

            switch (showHide.State)
            {
                case ShowHideState.None:
                    return UniTask.FromResult(before == ShowHideState.None ? ShowHideResult.Rejected : ShowHideResult.Stopped);
                case ShowHideState.Hidden:
                    return UniTask.FromResult(before == ShowHideState.Hidden ? ShowHideResult.AlreadyInState : ShowHideResult.Completed);
                case ShowHideState.Showing:
                    return UniTask.FromResult(ShowHideResult.Rejected);
                case ShowHideState.Shown:
                    return UniTask.FromResult(ShowHideResult.Rejected);
                case ShowHideState.Hiding:
                    return ShowHideStatePromise.WaitAsync(showHide, ShowHideState.Hidden, ShowHideState.Hiding, mode, token);
            }

            throw new InvalidOperationException($"Unexpected state {showHide.State}.");
        }

        public static UniTask<ShowHideResult> WaitForStateAsync(this IShowHide showHide, ShowHideState state, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return UniTask.FromCanceled<ShowHideResult>(token);

            if (showHide.State == state)
                return UniTask.FromResult(ShowHideResult.AlreadyInState);

            return ShowHideStatePromise.WaitAsync(showHide, state, state, ShowHideWaitMode.UntilReached, token);
        }
    }
}
