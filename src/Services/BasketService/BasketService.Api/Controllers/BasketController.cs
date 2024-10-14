using System;
using System.Net;
using System.Threading.Tasks;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        IBasketRepository basketRepository,
        IIdentityService identityService,
        IEventBus eventBus,
        ILogger<BasketController> logger)
    {
        _basketRepository = basketRepository;
        _identityService = identityService;
        _eventBus = eventBus;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Basket Service is up and running");
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket ), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetBasketById(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id);
        return Ok(basket ?? new CustomerBasket(id));
    }
    
    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(CustomerBasket ), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateBasket([FromBody] CustomerBasket basket)
    {
        var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);
        return Ok(updatedBasket);
    }
    
    [HttpPost]
    [Route("additem")]
    [ProducesResponseType(typeof(CustomerBasket ), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddItemToBasket([FromBody] BasketItem item)
    {
        var userId = _identityService.GetUserName();
        
        var basket = await _basketRepository.GetBasketAsync(userId);
        
        if (basket == null)
        {
            basket = new CustomerBasket(userId);
        }
        
        basket.Items.Add(item);
        
        var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);
        
        return Ok(updatedBasket);
    }
    
    
    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType(typeof(CustomerBasket ), (int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var userId = basketCheckout.Buyer;
        
        var basket = await _basketRepository.GetBasketAsync(userId);
        
        if (basket == null)
        {
            return BadRequest();
        }
        
        var userName = _identityService.GetUserName();

        var eventMessage = new OrderCreatedIntegrationEvent(
            userId: userId,
            userName: userName,
            city: basketCheckout.City,
            street: basketCheckout.Street,
            state: basketCheckout.State,
            country: basketCheckout.Country,
            zipCode: basketCheckout.ZipCode,
            cardNumber: basketCheckout.CardNumber,
            cardHolderName: basketCheckout.CardHolderName,
            cardExpiration: basketCheckout.CardExpiration,
            cardSecurityNumber: basketCheckout.CardSecurityNumber,
            cardTypeId: basketCheckout.CardTypeId,
            buyer: basketCheckout.Buyer,
            basket: basket);
        
        try
        {
            _eventBus.Publish(eventMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ERROR Publishing integration event: {EventId} from {AppName}", eventMessage.RequestId, "BasketService");
            throw;
        }
        
        return Accepted();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteBasket(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
        return Ok();
    }
    
}