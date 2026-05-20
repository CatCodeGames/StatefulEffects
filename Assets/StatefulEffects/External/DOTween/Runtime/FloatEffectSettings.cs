using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    [Serializable]
    public sealed class FloatEffectSettings
    {
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public EaseSettings Ease { get; private set; }
    }
}