using Microsoft.Extensions.Logging;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Interfaces.Services;
using PruebasDemo.Application.Resources;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Services
{
    public class CreditosService(IGenericRepository<CreditoEntity, Guid> repository, ILogger<CreditosService> logger) : ICreditoService
    {
        private readonly IGenericRepository<CreditoEntity, Guid> _repository = repository;
        private readonly ILogger<CreditosService> _logger = logger;

        public async Task CrearCredito(CreditoDto creditoDTO)
        {
            var credito = new CreditoEntity
            {
                Id = Guid.NewGuid(),
                Monto = creditoDTO.Monto,
                Saldo = creditoDTO.Monto,
                TasaInteres = creditoDTO.TasaInteres,
                Meses = creditoDTO.Meses,
                Estado = CreditoEstado.Activo
            };

            _logger.LogInformation(LogTemplates.CreditCreate, credito.Id);
            await _repository.CreateAsync(credito);
        }

        public async Task<List<CreditoEntity>> ObtenerCreditos()
            => await _repository.GetAllAsync();

        public async Task<CreditoEntity?> ObtenerCreditoPorId(Guid id)
            => await _repository.FindByIdAsync(id);

        public async Task ActualizarCredito(Guid id, CreditoDto creditoDTO)
        {
            var creditoExistente = await _repository.FindByIdAsync(id)
                ?? throw new KeyNotFoundException(Mensajes.CreditoNotFound);

            creditoExistente.Monto = creditoDTO.Monto;
            creditoExistente.TasaInteres = creditoDTO.TasaInteres;
            creditoExistente.Meses = creditoDTO.Meses;

            await _repository.UpdateAsync(creditoExistente);
        }

        public async Task EliminarCredito(Guid id)
        {
            var creditoExistente = await _repository.FindByIdAsync(id)
                ?? throw new KeyNotFoundException(Mensajes.CreditoNotFound);

            await _repository.DeleteAsync(creditoExistente.Id);
        }

        public async Task PagarCuota(Guid id, decimal montoPago)
        {
            var creditoExistente = await _repository.FindByIdAsync(id)
                ?? throw new KeyNotFoundException(Mensajes.CreditoNotFound);

            if (creditoExistente.Estado != CreditoEstado.Activo)
                throw new InvalidOperationException(Mensajes.CreditoNotActive);

            if (montoPago <= 0)
                throw new InvalidOperationException(Mensajes.PaymentMustBePositive);

            if (montoPago > creditoExistente.Saldo)
                throw new InvalidOperationException(Mensajes.PaymentExceedsBalance);

            creditoExistente.Saldo -= montoPago;

            if (creditoExistente.Saldo == 0)
                creditoExistente.Estado = CreditoEstado.Pagado;

            _logger.LogInformation(LogTemplates.PaymentMade, creditoExistente.Id, montoPago);
            await _repository.UpdateAsync(creditoExistente);
        }
    }
}
