using CatCode.StatefulEffects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public sealed class DemoShowHideAsync : MonoShowHideAsync
{

    [SerializeField] private Transform _target;

    protected override UniTask OnShow(CancellationToken token)
    {
        return DOTween.Sequence()
            .Append(_target.DOScale(1.5f * Vector3.one, 0.75f).SetEase(Ease.Linear))
            .Append(_target.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear))
            .SetEase(Ease.OutQuad).AwaitForComplete(cancellationToken: token);
    }

    protected override UniTask OnHide(CancellationToken token)
    {
        return DOTween.Sequence()
            .Append(_target.DOScale(1.5f * Vector3.one, 0.25f).SetEase(Ease.Linear))
            .Append(_target.DOScale(Vector3.zero, 0.75f).SetEase(Ease.Linear))
            .SetEase(Ease.InQuad).AwaitForComplete(cancellationToken: token);
    }

    protected override void OnSetHidden()
    {
        _target.localScale = Vector3.zero;
    }

    protected override void OnSetShown()
    {
        _target.localScale = Vector3.one;
    }

    protected override void OnStop()
    {
    }
}
