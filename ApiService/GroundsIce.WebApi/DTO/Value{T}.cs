namespace GroundsIce.WebApi.DTO.Common
{
    public class Value<T> : Value
    {
        public Value(int type)
            : base(type)
        {
        }

        public new T Payload { get; set; }
    }
}