using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class MonoShowHide : MonoBehaviour, IShowHide
    {
        public abstract ShowHideState State { get; }
        public abstract event Action<ShowHideState> OnStateChanged;

        public abstract void Show();
        public abstract void Hide();
        public abstract void SetShown();
        public abstract void SetHidden();
        public abstract void Stop();
    }
}
