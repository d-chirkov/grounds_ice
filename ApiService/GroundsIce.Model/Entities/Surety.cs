namespace GroundsIce.Model.Entities
{
    using System.Collections.Generic;

    public class Surety
    {
        public List<SuretyType> Types { get; set; }

        public string Others { get; set; }
    }
}
