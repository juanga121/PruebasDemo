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

            return Ok(new
            {
                exito = true,
                mensaje = "Crédito creado exitosamente"
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCreditos()
        {
            var creditos = await _creditosService.ObtenerCreditos();

            return Ok(new
            {
                exito = true,
                mensaje = "Créditos obtenidos correctamente",
                data = creditos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerCreditoPorId(Guid id)
        {
            var credito = await _creditosService.ObtenerCreditoPorId(id);

            if (credito == null)
            {
                return NotFound(new
                {
                    exito = false,
                    mensaje = "Crédito no encontrado"
                });
            }

            return Ok(new
            {
                exito = true,
                mensaje = "Crédito encontrado",
                data = credito
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCredito(Guid id, [FromBody] CreditoDTO creditoDTO)
        {
            try
            {
                await _creditosService.ActualizarCredito(id, creditoDTO);

                return Ok(new
                {
                    exito = true,
                    mensaje = "Crédito actualizado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCredito(Guid id)
        {
            try
            {
                await _creditosService.EliminarCredito(id);

                return Ok(new
                {
                    exito = true,
                    mensaje = "Crédito eliminado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        [HttpPut("pagar/{id}")]
        public async Task<IActionResult> PagarCuota(Guid id, [FromBody] decimal montoPago)
        {
            try
            {
                await _creditosService.PagarCuota(id, montoPago);

                return Ok(new
                {
                    exito = true,
                    mensaje = "Pago realizado correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }
    }
}
