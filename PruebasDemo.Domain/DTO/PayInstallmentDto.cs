using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasDemo.Domain.DTO
{
    public class PayInstallmentDto
    {
        public Guid Id { get; set; }
        public decimal PaymentAmount{ get; set; }
    }
}
