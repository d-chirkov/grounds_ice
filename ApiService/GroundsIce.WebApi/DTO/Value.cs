using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroundsIce.WebApi.DTO.Common
{
    public class Value
    {
        public Value(int type) => Type = type;

        public int Type { get; set; }
        public object Payload => null;
    }

    public class Value<T> : Value
    {
        public Value(int type) : base(type)
        {
        }

        public new T Payload { get; set; }
    }
}