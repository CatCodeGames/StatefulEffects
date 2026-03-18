using CatCode.Common;

namespace CatCode.StatefulEffects
{
    public interface IShowHide
    {
        IReadOnlyEventValue<ShowHideState> State { get; }

        void Show();
        void Hide();
        void SetShown();
        void SetHidden();
        void Stop();
    }
}