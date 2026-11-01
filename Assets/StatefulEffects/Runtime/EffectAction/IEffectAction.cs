using System;

namespace CatCode.StatefulEffects
{
    public interface IEffectAction
    {
        EffectActionState State { get; }
        event Action<EffectActionState> OnStateChanged;

        void Play();
        void Prepare();
        void Stop();
    }
}
