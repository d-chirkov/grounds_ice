using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model
{
    public class User
    {
        public User(UInt64 id) => Id = id;
        public UInt64 Id { get; private set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
