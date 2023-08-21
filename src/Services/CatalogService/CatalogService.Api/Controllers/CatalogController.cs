using Microsoft.AspNetCore.Mvc;
using CatalogService.Api.Core.Application.ViewModels;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(CatalogDbContext context, ILogger<CatalogController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Get Catalog Items

    /// <summary>
    /// Retrieves paginated catalog items.
    /// </summary>
    /// <param name="pageIndex">Page index.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paginated list of catalog items.</returns>
    /// <remarks>
    /// Use this endpoint to retrieve a paginated list of catalog items.
    /// Example request: GET /api/catalog/items?pageIndex=1&amp;pageSize=10
    /// </remarks>
    [HttpGet("items")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), 200)]
    public async Task<IActionResult> GetCatalogItems([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var totalItems = await _context.CatalogItems.LongCountAsync();
        var catalogItems = await _context.CatalogItems
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedItemsViewModel =
            new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, catalogItems);
        return Ok(paginatedItemsViewModel);
    }

    #endregion

    #region Get Catalog Brands

    /// <summary>
    /// Retrieves a list of catalog brands.
    /// </summary>
    /// <returns>List of catalog brands.</returns>
    /// <remarks>
    /// Use this endpoint to retrieve a list of available catalog brands.
    /// Example request: GET /api/catalog/brands
    /// </remarks>
    [HttpGet("brands")]
    [ProducesResponseType(typeof(IEnumerable<CatalogBrand>), 200)]
    public async Task<IActionResult> GetCatalogBrands()
    {
        var catalogBrands = await _context.CatalogBrands.ToListAsync();
        return Ok(catalogBrands);
    }

    #endregion

    #region Get Catalog Types

    /// <summary>
    /// Retrieves a list of catalog types.
    /// </summary>
    /// <returns>List of catalog types.</returns>
    /// <remarks>
    /// Use this endpoint to retrieve a list of available catalog types.
    /// Example request: GET /api/catalog/types
    /// </remarks>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<CatalogType>), 200)]
    public async Task<IActionResult> GetCatalogTypes()
    {
        var catalogTypes = await _context.CatalogTypes.ToListAsync();
        return Ok(catalogTypes);
    }

    #endregion

    #region Create Catalog Item

    /// <summary>
    /// Creates a new catalog item.
    /// </summary>
    /// <param name="catalogItem">Catalog item data.</param>
    /// <returns>Created catalog item.</returns>
    /// <remarks>
    /// Use this endpoint to create a new catalog item.
    /// Example request: POST /api/catalog
    /// Example request body:
    /// {
    ///     "name": "New Item",
    ///     "description": "This is a new item.",
    ///     "price": 19.99,
    ///     "pictureFileName": "new_item.jpg",
    ///     "pictureUri": "https://example.com/new_item.jpg",
    ///     "catalogTypeId": 1,
    ///     "catalogBrandId": 2
    /// }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateCatalogItem([FromBody] CatalogItem catalogItem)
    {
        if (catalogItem == null)
        {
            return BadRequest("Catalog item data is invalid.");
        }

        _context.CatalogItems.Add(catalogItem);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Created catalog item with ID: {catalogItem.Id}");
        return CreatedAtAction(nameof(GetCatalogItems), new { id = catalogItem.Id }, catalogItem);
    }

    #endregion
}