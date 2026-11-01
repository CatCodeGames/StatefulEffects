using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    [Serializable]
    public sealed class DOTweenFloatSettings
    {
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public DOTweenEaseSettings Ease { get; private set; }

        public DOTweenFloatSettings()
        {
            Ease = new DOTweenEaseSettings();
        }

        public DOTweenFloatSettings(float value, float duration, DOTweenEaseSettings ease)
        {
            Value = value;
            Duration = duration;
            Ease = ease;
        }
    }
}
