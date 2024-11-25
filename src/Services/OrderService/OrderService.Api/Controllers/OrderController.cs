using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Order.Queries;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMediator  _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetailsById(Guid id)
    {
        var response = await _mediator.Send(new GetOrderDetailsQuery(id));
        return Ok(response);
    }
}