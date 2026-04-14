using Microsoft.AspNetCore.Mvc;
using PruebasDemo.Application.Services;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Resources;

namespace PruebasDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditoController(CreditosService creditosService) : Controller
    {
        private readonly CreditosService _creditosService = creditosService;

        [HttpPost]
        public async Task<IActionResult> CrearCredito([FromBody] CreditoDTO creditoDTO)
        {
            await _creditosService.CrearCredito(creditoDTO);

            return Ok(new
            {
                exito = true,
                mensaje = CreditoMensajes.SuccessCreated
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCreditos()
        {
            var creditos = await _creditosService.ObtenerCreditos();

            return Ok(new
            {
                exito = true,
                mensaje = CreditoMensajes.SuccessGet,
                data = creditos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerCreditoPorId(Guid id)
        {
            var credito = await _creditosService.ObtenerCreditoPorId(id);

            return Ok(new
            {
                exito = true,
                mensaje = CreditoMensajes.SuccessFound,
                data = credito
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCredito(Guid id, [FromBody] CreditoDTO creditoDTO)
        {
            await _creditosService.ActualizarCredito(id, creditoDTO);

            return Ok(new
            {
                exito = true,
                mensaje = CreditoMensajes.SuccessUpdated
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCredito(Guid id)
        {
            await _creditosService.EliminarCredito(id);

            return Ok(new
            {
                exito = true,
                mensaje = CreditoMensajes.SuccessDeleted
            });
        }

        [HttpPut("pagar/{id}")]
        public async Task<IActionResult> PagarCuota(Guid id, [FromBody] decimal montoPago)
        {
            await _creditosService.PagarCuota(id, montoPago);

            return Ok(new
            {
                exito = true,
                mensaje = CreditoMensajes.SuccessPayment
            });
        }
    }
}
