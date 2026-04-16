using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasDemo.Application.Interfaces.Services
{
    public interface ICreditoService
    {
        Task CrearCredito(CreditoDto creditoDTO);
        Task<List<CreditoEntity>> ObtenerCreditos();
        Task<CreditoEntity?> ObtenerCreditoPorId(Guid id);
        Task ActualizarCredito(Guid id, CreditoDto creditoDTO);
        Task EliminarCredito(Guid id);
        Task PagarCuota(Guid id, decimal montoPago);
    }
}
