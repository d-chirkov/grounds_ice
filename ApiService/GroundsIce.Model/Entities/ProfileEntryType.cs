namespace GroundsIce.Model.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProfileEntryType
    {
        FirstName,
        LastName,
        MiddleName,
        Description,
        City,
        Region
    }
}
