﻿using System;

namespace CatCode.ShowHideEffects
{
    public interface IShowHide
    {
        ShowHideState State { get; }

        event Action<ShowHideState> StateChanged;

        void Show();
        void Hide();

        void SetShown();
        void SetHidden();

        void Stop();
    }
}