namespace GroundsIce.Model.Abstractions.Validators
{
    using System.Threading.Tasks;

    public interface IStringValidator
    {
        Task<bool> ValidateAsync(string value);
    }
}
