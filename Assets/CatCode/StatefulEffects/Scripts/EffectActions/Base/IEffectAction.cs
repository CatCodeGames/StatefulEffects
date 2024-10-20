using System;

namespace CatCode.StatefulEffects
{
    public interface IEffectAction
    { 
        EffectActionState State { get; }
        event Action<EffectActionState> StateChanged;
        void Play();
        void Stop();
    }
}