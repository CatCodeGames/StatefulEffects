using Cysharp.Threading.Tasks;
using System.Threading;

namespace CatCode.StatefulEffects
{
    public static class ShowHideAsyncExtensions
    {
        public static UniTask WaitStateAsync(this IShowHide showHide, ShowHideState targetState, CancellationToken token)
            => showHide.State.WaitAsync(targetState, token);

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