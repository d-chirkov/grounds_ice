namespace GroundsIce.Model.Abstractions
{
    using System.Data;
    using System.Threading.Tasks;

    public interface IConnectionFactory
    {
        Task<IDbConnection> GetConnectionAsync();
    }
}
