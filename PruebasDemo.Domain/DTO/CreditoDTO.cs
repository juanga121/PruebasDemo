using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasDemo.Domain.DTO
{
    public class CreditoDTO
    {
        public decimal Monto { get;set; }
        public decimal TasaInteres { get; set; }
        public int Meses { get; set; }
    }
}
