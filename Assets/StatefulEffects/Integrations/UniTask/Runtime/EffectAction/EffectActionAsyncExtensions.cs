using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CatCode.StatefulEffects
{
    public static class EffectActionAsyncExtensions
    {
        public static UniTask<EffectActionResult> PlayAsync(this IEffectAction effectAction, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return UniTask.FromCanceled<EffectActionResult>(token);

            effectAction.Play();

            switch (effectAction.State)
            {
                case EffectActionState.None:
                    return UniTask.FromResult(EffectActionResult.Stopped);
                case EffectActionState.Ready:
                    return UniTask.FromResult(EffectActionResult.Completed);
                case EffectActionState.Playing:
                    return EffectActionStatePromise.WaitAsync(effectAction, EffectActionState.Ready, token);
            }

            throw new InvalidOperationException($"Unexpected state {effectAction.State}.");
        }

        public static UniTask<EffectActionResult> WaitForStateAsync(this IEffectAction effectAction, EffectActionState state, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return UniTask.FromCanceled<EffectActionResult>(token);

            if (effectAction.State == state)
                return UniTask.FromResult(EffectActionResult.AlreadyInState);

            return EffectActionStatePromise.WaitAsync(effectAction, state, token);
        }
    }
}
