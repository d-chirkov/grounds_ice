namespace GroundsIce.Model.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreditType
    {
        Auto,
        Business,
        Consumer,
        Hypothec,
        Micro,
        Refinancing,
        Other
    }
}
