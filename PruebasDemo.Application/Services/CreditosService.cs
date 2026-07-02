using FluentValidation;
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
    public class CreditosService(IGenericRepository<CreditoEntity, Guid> repository, ILogger<CreditosService> logger, IValidator<PagarCuotaDto> pagarCuotaValidator) : ICreditoService
    {
        private readonly IGenericRepository<CreditoEntity, Guid> _repository = repository;
        private readonly ILogger<CreditosService> _logger = logger;
        private readonly IValidator<PagarCuotaDto> _pagarCuotaValidator = pagarCuotaValidator;

        public CreditosService(
            IGenericRepository<CreditoEntity, Guid> repository, 
            ILogger<CreditosService> logger,
            IValidator<CreditoDto> creditoDtoValidator,
            IValidator<PagoCreditoDto> pagoCreditoValidator)
        {
            _repository = repository;
            _logger = logger;
            _creditoDtoValidator = creditoDtoValidator;
            _pagoCreditoValidator = pagoCreditoValidator;
        }

        public async Task CrearCredito(CreditoDto creditoDTO)
        {
            var validationResult = await _creditoDtoValidator.ValidateAsync(creditoDTO);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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
            var validationResult = await _creditoDtoValidator.ValidateAsync(creditoDTO);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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

        public async Task PagarCuota(PagarCuotaDto dto)
        {
            var result = await _pagarCuotaValidator.ValidateAsync(dto);

            if (!result.IsValid)
                throw new InvalidOperationException(result.Errors.First().ErrorMessage);

            var credito = await ObtenerCredito(dto.Id);

            ValidarCreditoActivo(credito);
            ValidarMonto(dto.MontoPago, credito.Saldo);

            ApplyPago(credito, dto.MontoPago);

            _logger.LogInformation(LogTemplates.PaymentMade, credito.Id, dto.MontoPago);

            await _repository.UpdateAsync(credito);
        }

        private async Task<CreditoEntity> ObtenerCredito(Guid id)
        {
            return await _repository.FindByIdAsync(id)
                ?? throw new KeyNotFoundException(Mensajes.CreditoNotFound);
        }

        private static void ValidarCreditoActivo(CreditoEntity credito)
        {
            if (credito.Estado != CreditoEstado.Activo)
                throw new InvalidOperationException(Mensajes.CreditoNotActive);
        }

        private static void ValidarMonto(decimal monto, decimal saldo)
        {
            if (monto > saldo)
                throw new InvalidOperationException(Mensajes.PaymentExceedsBalance);
        }

        private static void ApplyPago(CreditoEntity credito, decimal montoPago)
        {
            credito.Saldo -= montoPago;

            if (credito.Saldo == 0)
            {
                credito.Estado = CreditoEstado.Pagado;
            }
        }
    }
}
