namespace GroundsIce.WebApi.DTO.Common
{
    public class Value
    {
        public Value(int type) => this.Type = type;

        public int Type { get; set; }

        public object Payload => null;
    }
}