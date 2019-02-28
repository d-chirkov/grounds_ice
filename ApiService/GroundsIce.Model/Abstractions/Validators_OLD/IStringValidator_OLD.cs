namespace GroundsIce.Model.Abstractions.Validators
{
    using System.Threading.Tasks;

    public interface IStringValidator_OLD
    {
        Task<bool> ValidateAsync(string value);
    }
}
