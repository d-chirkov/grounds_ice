namespace GroundsIce.Model.Entities
{
    using System;

    public class BorrowOrder
    {
        public long Id { get; set; }

        public DateTime? CreationTime { get; set; }

        public decimal Amount { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public int TermInDays { get; set; }

        public float Percent { get; set; }

        public PaymentFrequency? PaymentFrequency { get; set; }

        public Surety Surety { get; set; }

        public CreditType CreditType { get; set; }

        public string Comment { get; set; }
    }
}
