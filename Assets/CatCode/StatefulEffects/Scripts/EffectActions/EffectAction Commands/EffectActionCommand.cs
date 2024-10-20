using CatCode.Commands;
using CatCode.StatefulEffects;
using UnityEngine;

public class EffectActionCommand : MonoBehaviour
{
    public static EffectActionCommand<T> WaitIdle<T>(T effectAction) where T : IEffectAction
        => new(effectAction, EffectActionState.Idle);

    public static EffectActionCommand<T> WaitPlay<T>(T effectAction) where T : IEffectAction
        => new(effectAction, EffectActionState.Playing);

    public static EffectActionCommand<T> PlayAndWaitIdle<T>(T effectAction) where T : IEffectAction
    {
        return new EffectActionCommand<T>(effectAction, EffectActionState.Idle)
            .AddOnStarted(() => effectAction.Play());
    }
}

public sealed class EffectActionCommand<T> : Command where T : IEffectAction
{
    private readonly T _effectAction;
    private readonly EffectActionState _targetState;

    public EffectActionCommand(T effectAction, EffectActionState targetState)
    {
        _effectAction = effectAction;
        _targetState = targetState;
    }

    protected override void OnExecute()
    {
        var monoEffectAction = _effectAction as MonoEffectAction;
        if (monoEffectAction == null || !monoEffectAction)
        {
            Continue();
            return;
        }
        if (_effectAction.State == _targetState)
        {
            Continue();
            return;
        }
        _effectAction.StateChanged += OnEffectActionStateChanged;
    }

    private void OnEffectActionStateChanged(EffectActionState state)
    {
        if (state != _targetState)
            return;
        _effectAction.StateChanged -= OnEffectActionStateChanged;
        Continue();
    }

    protected override void OnStop()
    {
        _effectAction.StateChanged -= OnEffectActionStateChanged;
    }
}
