using PruebasDemo.Application.Repositories;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasDemo.Application.Services
{
    public class CreditosService(IGenericRepository<CreditoEntity, Guid> repository)
    {
        private readonly IGenericRepository<CreditoEntity, Guid> _repository = repository;

        public async Task CrearCredito(CreditoDTO creditoDTO)
        {
            CreditoEntity credito = new()
            {
                Id = Guid.NewGuid(),
                Monto = creditoDTO.Monto,
                Saldo = creditoDTO.Monto,
                TasaInteres = creditoDTO.TasaInteres,
                Meses = creditoDTO.Meses,
                Estado = 1,
                FechaCreacion = DateTime.UtcNow
            };

            await _repository.CreateAsync(credito);
        }

        public async Task<List<CreditoEntity>> ObtenerCreditos()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CreditoEntity?> ObtenerCreditoPorId(Guid id)
        {
            return await _repository.FindByIdAsync(id);
        }

        public async Task ActualizarCredito(Guid id, CreditoDTO creditoDTO)
        {
            var creditoExistente = await _repository.FindByIdAsync(id) ?? throw new Exception("Crédito no encontrado");

            creditoExistente.Monto = creditoDTO.Monto;
            creditoExistente.TasaInteres = creditoDTO.TasaInteres;
            creditoExistente.Meses = creditoDTO.Meses;

            await _repository.UpdateAsync(creditoExistente);
        }

        public async Task EliminarCredito(Guid id)
        {
            var creditoExistente = await _repository.FindByIdAsync(id) ?? throw new Exception("Crédito no encontrado");
            await _repository.DeleteAsync(creditoExistente.Id);
        }

        public async Task PagarCuota(Guid id, decimal montoPago)
        {
            var creditoExistente = await _repository.FindByIdAsync(id) ?? throw new Exception("Crédito no encontrado");

            if (creditoExistente.Estado != 1)
                throw new Exception("El crédito no está activo");

            if (montoPago <= 0)
                throw new Exception("El monto de pago debe ser mayor a cero");

            if (montoPago > creditoExistente.Saldo)
                throw new Exception("El monto de pago excede el saldo del crédito");

            creditoExistente.Saldo -= montoPago;

            if (creditoExistente.Saldo == 0)
                creditoExistente.Estado = 2;

            await _repository.UpdateAsync(creditoExistente);
        }
    }
}
