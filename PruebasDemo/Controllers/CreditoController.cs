using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebasDemo.Application.Creditos.Commands.ActualizarCredito;
using PruebasDemo.Application.Creditos.Commands.CrearCredito;
using PruebasDemo.Application.Creditos.Commands.EliminarCredito;
using PruebasDemo.Application.Creditos.Commands.PagarCuota;
using PruebasDemo.Application.Creditos.Queries.ObtenerCreditoPorId;
using PruebasDemo.Application.Creditos.Queries.ObtenerCreditos;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditoController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CrearCredito([FromBody] CreditoDto creditoDTO)
        {
            await sender.Send(new CrearCreditoCommand(creditoDTO));

            return Ok(new
            {
                exito = true,
                mensaje = Mensajes.SuccessCreated
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCreditos()
        {
            var creditos = await sender.Send(new ObtenerCreditosQuery());

            return Ok(new
            {
                exito = true,
                mensaje = Mensajes.SuccessGet,
                data = creditos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerCreditoPorId(Guid id)
        {
            var credito = await sender.Send(new ObtenerCreditoPorIdQuery(id));

            return Ok(new
            {
                exito = true,
                mensaje = Mensajes.SuccessFound,
                data = credito
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCredito(Guid id, [FromBody] CreditoDto creditoDTO)
        {
            await sender.Send(new ActualizarCreditoCommand(id, creditoDTO));

            return Ok(new
            {
                exito = true,
                mensaje = Mensajes.SuccessUpdated
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCredito(Guid id)
        {
            await sender.Send(new EliminarCreditoCommand(id));

            return Ok(new
            {
                exito = true,
                mensaje = Mensajes.SuccessDeleted
            });
        }

        [HttpPut("pagar")]
        public async Task<IActionResult> PagarCuota(PagarCuotaDto pagarCuotaDto)
        {
            await sender.Send(new PagarCuotaCommand(pagarCuotaDto));

            return Ok(new
            {
                exito = true,
                mensaje = Mensajes.SuccessPayment
            });
        }
    }
}
