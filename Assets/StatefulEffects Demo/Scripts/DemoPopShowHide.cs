using CatCode.StatefulEffects;
using DG.Tweening;
using System;
using UnityEngine;

public sealed class DemoPopShowHide : CallbackShowHide
{
    private Tween _tween;

    [SerializeField] private Transform _target;

    protected override void OnShow(ShowHideState from, Action onCompleted)
    {
        _tween.Kill();
        if (from == ShowHideState.None)
            _target.localScale = Vector3.zero;
        _tween = DOTween.Sequence()
            .Append(_target.DOScale(1.5f * Vector3.one, 0.75f).SetEase(Ease.Linear))
            .Append(_target.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear))
            .SetEase(Ease.OutQuad)
            .OnComplete(() => onCompleted());
    }

    protected override void OnHide(ShowHideState from, Action onCompleted)
    {
        _tween.Kill();
        if (from == ShowHideState.None)
            _target.localScale = Vector3.one;
        _tween = DOTween.Sequence()
            .Append(_target.DOScale(1.5f * Vector3.one, 0.25f).SetEase(Ease.Linear))
            .Append(_target.DOScale(Vector3.zero, 0.75f).SetEase(Ease.Linear))
            .SetEase(Ease.InQuad)
            .OnComplete(() => onCompleted());
    }

    protected override void OnSetHidden(ShowHideState from)
    {
        _tween.Kill();
        _target.localScale = Vector3.zero;
    }

    protected override void OnSetShown(ShowHideState from)
    {
        _tween.Kill();
        _target.localScale = Vector3.one;
    }

    protected override void OnStop(ShowHideState from)
    {
        _tween.Kill();
    }
}
