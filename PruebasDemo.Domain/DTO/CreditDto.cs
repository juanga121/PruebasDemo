using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasDemo.Domain.DTO
{
    public class CreditDto
    {
        public decimal Amount { get;set; }
        public decimal InterestRate { get; set; }
        public int Months { get; set; }
    }
}
