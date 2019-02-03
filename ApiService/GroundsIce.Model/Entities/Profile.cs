using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Entities
{
	public class Profile
	{
		public string Login { get; set; }
		public string Avatar { get; set; }
		public ProfileInfo ProfileInfo { get; set; }
	}
}
