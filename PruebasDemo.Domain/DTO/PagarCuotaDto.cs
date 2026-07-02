using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasDemo.Domain.DTO
{
    public class PagarCuotaDto
    {
        public Guid Id { get; set; }
        public decimal MontoPago{ get; set; }
    }
}
