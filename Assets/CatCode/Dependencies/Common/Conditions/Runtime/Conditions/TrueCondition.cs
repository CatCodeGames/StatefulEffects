namespace CatCode.Common
{
    public sealed class TrueCondition : ICondition
    {
        public bool Check()
            => true;
    }
}