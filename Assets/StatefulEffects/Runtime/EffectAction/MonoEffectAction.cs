using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class MonoEffectAction : MonoBehaviour, IEffectAction
    {
        public abstract EffectActionState State { get; }
        public abstract event Action<EffectActionState> OnStateChanged;

        public abstract void Play();
        public abstract void Prepare();
        public abstract void Stop();
    }
}
