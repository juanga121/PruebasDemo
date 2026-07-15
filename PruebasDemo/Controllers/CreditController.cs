using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebasDemo.Application.Credits.Commands.UpdateCredit;
using PruebasDemo.Application.Credits.Commands.CreateCredit;
using PruebasDemo.Application.Credits.Commands.DeleteCredit;
using PruebasDemo.Application.Credits.Commands.PayInstallment;
using PruebasDemo.Application.Credits.Queries.GetCreditById;
using PruebasDemo.Application.Credits.Queries.GetCredits;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCredit([FromBody] CreditDto creditDto)
        {
            await sender.Send(new CreateCreditCommand(creditDto));

            return Ok(new
            {
                exito = true,
                mensaje = Messages.SuccessCreated
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCredits()
        {
            var credits = await sender.Send(new GetCreditsQuery());

            return Ok(new
            {
                exito = true,
                mensaje = Messages.SuccessGet,
                data = credits
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCreditById(Guid id)
        {
            var credit = await sender.Send(new GetCreditByIdQuery(id));

            return Ok(new
            {
                exito = true,
                mensaje = Messages.SuccessFound,
                data = credit
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCredit(Guid id, [FromBody] CreditDto creditDto)
        {
            await sender.Send(new UpdateCreditCommand(id, creditDto));

            return Ok(new
            {
                exito = true,
                mensaje = Messages.SuccessUpdated
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCredit(Guid id)
        {
            await sender.Send(new DeleteCreditCommand(id));

            return Ok(new
            {
                exito = true,
                mensaje = Messages.SuccessDeleted
            });
        }

        [HttpPut("pagar")]
        public async Task<IActionResult> PayInstallment(PayInstallmentDto payInstallmentDto)
        {
            await sender.Send(new PayInstallmentCommand(payInstallmentDto));

            return Ok(new
            {
                exito = true,
                mensaje = Messages.SuccessPayment
            });
        }
    }
}
