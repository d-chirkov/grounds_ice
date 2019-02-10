using GroundsIce.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroundsIce.WebApi.Controllers.Profile.DTO
{
	public class ProfileInfoModel
	{
		public List<ProfileInfoEntry> ProfileInfo { get; set; }
	}
}