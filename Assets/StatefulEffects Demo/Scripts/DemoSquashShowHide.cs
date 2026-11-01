using CatCode.StatefulEffects;
using DG.Tweening;
using System;
using UnityEngine;

public sealed class DemoSquashShowHide : CallbackShowHide
{
    private Tween _tween;

    [SerializeField] private Transform _target;
    [SerializeField] private ParticleSystem _hideParticles;

    protected override void OnShow(ShowHideState from, Action onCompleted)
    {
        _tween.Kill();
        _target.localScale = new Vector3(1f, 0f, 1f);
        _tween = DOTween.Sequence()
            .Append(_target.DOScale(new Vector3(0.5f, 1.5f, 0.5f), 0.2f).SetEase(Ease.OutQuad))
            .Append(_target.DOScale(new Vector3(1.5f, 0.5f, 1.5f), 0.2f).SetEase(Ease.InOutQuad))
            .Append(_target.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad))
            .OnComplete(() => onCompleted());
    }

    protected override void OnSetShown(ShowHideState from)
    {
        _tween.Kill();
        _target.localScale = Vector3.one;
    }

    protected override void OnHide(ShowHideState from, Action onCompleted)
    {
        _tween.Kill();
        if (from == ShowHideState.None)
            _target.localScale = Vector3.one;
        _tween = DOTween.Sequence()
            .Append(_target.DOScale(new Vector3(0.5f, 1.5f, 0.5f), 0.2f).SetEase(Ease.OutQuad))
            .Append(_target.DOScale(new Vector3(1.5f, 0.0f, 1.5f), 0.2f).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                _target.localScale = Vector3.zero;
                _hideParticles.Play();
                onCompleted();
            });
    }

    protected override void OnSetHidden(ShowHideState from)
    {
        _tween.Kill();
        _target.localScale = Vector3.zero;
    }

    protected override void OnStop(ShowHideState from)
    {
        _tween.Kill();
    }
}
