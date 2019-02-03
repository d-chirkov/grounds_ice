using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Entities
{
	public class ProfileInfo
	{
		public ProfileInfoEntry FirstName { get; set; }
		public ProfileInfoEntry MiddleName { get; set; }
		public ProfileInfoEntry Surname { get; set; }
		public ProfileInfoEntry Location { get; set; }
		public ProfileInfoEntry Description { get; set; }
	}
}
