using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasDemo.Domain.Entities
{
    public class CreditoEntity
    {
        public Guid Id { get;  set; }
        public decimal Monto { get; set; }
        public decimal Saldo { get; set; }
        public decimal TasaInteres { get; set; }
        public int Meses { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
