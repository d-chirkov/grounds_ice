using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Entities
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ProfileInfoType
	{
		FirstName,
		LastName,
		MiddleName,
		Description,
		City,
		Region
	}
}
