namespace CatCode.Common
{
    public interface IEventRequest : IReadOnlyEventRequest
    {
        void Request();
        void Reset();
    }
}