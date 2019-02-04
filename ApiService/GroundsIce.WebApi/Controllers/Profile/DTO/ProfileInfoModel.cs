using GroundsIce.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroundsIce.WebApi.Controllers.Profile.DTO
{
	//public class ProfileInfoEntryModel
	//{
	//	public string Type { get; set; }
	//	public string Value { get; set; }
	//	public bool IsPublic { get; set; }
	//}

	public class ProfileInfoModel
	{
		public List<ProfileInfoEntry> ProfileInfo { get; set; }
	}
}