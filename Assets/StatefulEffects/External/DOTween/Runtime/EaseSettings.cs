using DG.Tweening;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    [Serializable]
    public sealed class EaseSettings
    {
        [field: SerializeField] public Ease Ease { get; private set; }
        [field: SerializeField] public bool UseCurve { get; private set; }
        [field: SerializeField] public AnimationCurve Curve { get; private set; }
    }
}