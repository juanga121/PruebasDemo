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
    public class CreditosService : ICreditoService
    {
        private readonly IGenericRepository<CreditoEntity, Guid> _repository;
        private readonly ILogger<CreditosService> _logger;
        private readonly IValidator<CreditoDto> _creditoDtoValidator;
        private readonly IValidator<PagoCreditoDto> _pagoCreditoValidator;

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

        public async Task PagarCuota(PagoCreditoDto pagoCreditoDto)
        {
            var validationResult = await _pagoCreditoValidator.ValidateAsync(pagoCreditoDto);
            
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var credito = (await _repository.FindByIdAsync(pagoCreditoDto.CreditoId))!;
            
            credito.Saldo -= pagoCreditoDto.MontoPago;

            if (credito.Saldo == 0)
                credito.Estado = CreditoEstado.Pagado;

            _logger.LogInformation(LogTemplates.PaymentMade, credito.Id, pagoCreditoDto.MontoPago);
            await _repository.UpdateAsync(credito);
        }
    }
}
