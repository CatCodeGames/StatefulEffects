using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CatCode.ShowHideEffects
{
    public static class ShowHideAsyncExtensions
    {
        public static Awaitable StateToAwaitable(this IShowHide showHide, ShowHideState targetState, CancellationToken token)
        {
            var tcs = new AwaitableCompletionSource();
            if (showHide.State == targetState)
                tcs.SetResult();
            else
            {
                var cts = token.Register(OnCancel);
                showHide.StateChanged += OnStateChanged;
            }
            return tcs.Awaitable;

            void OnStateChanged(ShowHideState state)
            {
                if (state != targetState)
                    return;
                showHide.StateChanged -= OnStateChanged;
                tcs.SetResult();
            }

            void OnCancel()
            {
                showHide.StateChanged -= OnStateChanged;
                tcs.SetCanceled();
            }
        }

        public static Awaitable WaitShowToAwaitable(this IShowHide showHide, CancellationToken token)
            => showHide.StateToAwaitable(ShowHideState.Show, token);

        public static Awaitable WaitHideToAwaitable(this IShowHide showHide, CancellationToken token)
            => showHide.StateToAwaitable(ShowHideState.Hide, token);

        public static Awaitable WaitShownToAwaitable(this IShowHide showHide, CancellationToken token)
            => showHide.StateToAwaitable(ShowHideState.Shown, token);

        public static Awaitable WaitHiddenToAwaitable(this IShowHide showHide, CancellationToken token)
            => showHide.StateToAwaitable(ShowHideState.Hidden, token);



        public static Task WaitStateToTask(this IShowHide showHide, ShowHideState targetState, CancellationToken token)
        {
            if (showHide.State == targetState)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<ShowHideState>();
            var cts = token.Register(OnCancel);
            showHide.StateChanged += OnStateChanged;

            return tcs.Task;

            void OnStateChanged(ShowHideState state)
            {
                if (state != targetState)
                    return;
                showHide.StateChanged -= OnStateChanged;
                tcs.SetResult(state);
            }

            void OnCancel()
            {
                showHide.StateChanged -= OnStateChanged;
                tcs.SetCanceled();
            }
        }

        public static Task WaitShowToTask(this IShowHide showHide, CancellationToken token)
            => showHide.WaitStateToTask(ShowHideState.Show, token);

        public static Task WaitHideToTask(this IShowHide showHide, CancellationToken token)
            => showHide.WaitStateToTask(ShowHideState.Hide, token);

        public static Task WaitShownToTask(this IShowHide showHide, CancellationToken token)
            => showHide.WaitStateToTask(ShowHideState.Shown, token);

        public static Task WaitHiddenToTask(this IShowHide showHide, CancellationToken token)
            => showHide.WaitStateToTask(ShowHideState.Hidden, token);
    }
}