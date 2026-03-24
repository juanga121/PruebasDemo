using Microsoft.AspNetCore.Mvc;
using PruebasDemo.Application.Services;
using PruebasDemo.Domain.DTO;

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
            return Ok("Crédito creado exitosamente");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCreditos()
        {
            var creditos = await _creditosService.ObtenerCreditos();
            return Ok(creditos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerCreditoPorId(Guid id)
        {
            var credito = await _creditosService.ObtenerCreditoPorId(id);
            if (credito == null)
                return NotFound("Crédito no encontrado");
            return Ok(credito);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCredito(Guid id, [FromBody] CreditoDTO creditoDTO)
        {
            try
            {
                await _creditosService.ActualizarCredito(id, creditoDTO);
                return Ok("Crédito actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCredito(Guid id)
        {
            try
            {
                await _creditosService.EliminarCredito(id);
                return Ok("Crédito eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }
    }
}
