using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Entities
{
	public class ProfileInfoEntry : IEquatable<ProfileInfoEntry>
	{
		public string Value { get; set; }
		public bool IsPublic { get; set; }

		public bool Equals(ProfileInfoEntry other)
		{
			return other != null && Value == other.Value && IsPublic == other.IsPublic;
		}
	}
}
