namespace CatCode.Common
{
    public interface IEventValue<T> : IReadOnlyEventValue<T>
    {
        new T Value { get; set; }
    }
}
