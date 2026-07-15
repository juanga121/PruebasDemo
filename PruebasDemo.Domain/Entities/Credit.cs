using System;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Domain.Entities
{
    public class Credit
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal InterestRate { get; set; }
        public int Months { get; set; }
        public CreditStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
