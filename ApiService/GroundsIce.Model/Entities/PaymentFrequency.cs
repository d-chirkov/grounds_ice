namespace GroundsIce.Model.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentFrequency
    {
        Year,
        Month,
        Day,
        AllPeriod
    }
}
