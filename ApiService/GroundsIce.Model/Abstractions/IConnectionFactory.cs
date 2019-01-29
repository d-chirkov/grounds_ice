using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Abstractions
{
	public interface IConnectionFactory
	{
		Task<IDbConnection> GetConnectionAsync();
	}
}
