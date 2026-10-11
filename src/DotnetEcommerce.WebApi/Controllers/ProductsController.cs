using DotnetEcommerce.Application.Common.Interfaces;
using DotnetEcommerce.Application.Features.Catalog.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetEcommerce.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;

    public ProductsController(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts(
        [FromQuery] string? category,
        [FromQuery] string? search,
        [FromQuery] string? sort,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category) && !category.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(p => p.Category != null && p.Category.Slug == category.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchLower) || p.Description.ToLower().Contains(searchLower));
        }

        query = sort switch
        {
            "price-asc" => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.Rating),
            _ => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAtUtc)
        };

        var products = await query.ToListAsync(cancellationToken);
        return Ok(products.Select(ProductResponse.FromEntity));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(ProductResponse.FromEntity(product));
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return Ok(categories.Select(CategoryResponse.FromEntity));
    }
}
