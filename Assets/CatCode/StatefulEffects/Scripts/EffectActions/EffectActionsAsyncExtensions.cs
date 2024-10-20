using CatCode.StatefulEffects;
using CatCode.StatefulEffects;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static  class EffectActionsAsyncExtensions 
{
    public static Awaitable WaitStateToAwaitable(this IEffectAction effectAction, EffectActionState targetState, CancellationToken token)
    {
        var tcs = new AwaitableCompletionSource();
        if (effectAction.State == targetState)
            tcs.SetResult();
        else
        {
            var cts = token.Register(OnCancel);
            effectAction.StateChanged += OnStateChanged;
        }
        return tcs.Awaitable;

        void OnStateChanged(EffectActionState state)
        {
            if (state != targetState)
                return;
            effectAction.StateChanged -= OnStateChanged;
            tcs.SetResult();
        }

        void OnCancel()
        {
            effectAction.StateChanged -= OnStateChanged;
            tcs.SetCanceled();
        }
    }
  
    public static Awaitable WaitPlayToAwaitable(this IEffectAction effectAction, CancellationToken token)
       => effectAction.WaitStateToAwaitable(EffectActionState.Playing, token);

    public static Awaitable WaitIdleToAwaitable(this IEffectAction effectAction, CancellationToken token)
           => effectAction.WaitStateToAwaitable(EffectActionState.Idle, token);




    public static Task WaitStateToTask(this IEffectAction effectAction, EffectActionState targetState, CancellationToken token)
    {
        if (effectAction.State == targetState)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource<EffectActionState>();
        var cts = token.Register(OnCancel);
        effectAction.StateChanged += OnStateChanged;

        return tcs.Task;

        void OnStateChanged(EffectActionState state)
        {
            if (state != targetState)
                return;
            effectAction.StateChanged -= OnStateChanged;
            tcs.SetResult(state);
        }

        void OnCancel()
        {
            effectAction.StateChanged -= OnStateChanged;
            tcs.SetCanceled();
        }
    }

    public static Task WaitPlayToTask(this IEffectAction effectAction, CancellationToken token)
           => effectAction.WaitStateToTask(EffectActionState.Playing, token);

    public static Task WaitIdleToTask(this IEffectAction effectAction, CancellationToken token)
           => effectAction.WaitStateToTask(EffectActionState.Idle, token);
}
