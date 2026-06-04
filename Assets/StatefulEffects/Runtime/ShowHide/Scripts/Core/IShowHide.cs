using CatCode.Events;

namespace CatCode.StatefulEffects
{
    public interface IShowHide
    {
        bool IsRunning { get; }
        IReadOnlyEventValue<ShowHideState> State { get; }

        void Show();
        void Hide();
        void SetShown();
        void SetHidden();
        void Stop();
    }        
}