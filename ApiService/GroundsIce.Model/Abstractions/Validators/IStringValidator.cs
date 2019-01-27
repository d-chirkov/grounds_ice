using System.Threading.Tasks;

namespace GroundsIce.Model.Abstractions.Validators
{
    public interface IStringValidator
    {
        Task<bool> ValidateAsync(string value);
    }
}
