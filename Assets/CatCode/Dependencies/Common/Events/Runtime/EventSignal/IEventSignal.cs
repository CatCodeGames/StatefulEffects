namespace CatCode.Common
{
    public interface IEventSignal : IReadOnlyEventSignal
    {
        void Invoke();
    }
}