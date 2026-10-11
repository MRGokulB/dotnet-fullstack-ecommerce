using DotnetEcommerce.Domain.Entities;

namespace DotnetEcommerce.Application.Features.Catalog.Dtos;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string Icon
)
{
    public static CategoryResponse FromEntity(Category c) =>
        new(c.Id, c.Name, c.Slug, c.Description, c.Icon);
}

public record ProductResponse(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId,
    string? CategoryName,
    int StockQuantity,
    double Rating,
    int ReviewCount,
    bool IsFeatured
)
{
    public static ProductResponse FromEntity(Product p) =>
        new(
            p.Id,
            p.Name,
            p.Slug,
            p.Description,
            p.Price,
            p.ImageUrl,
            p.CategoryId,
            p.Category?.Name,
            p.StockQuantity,
            p.Rating,
            p.ReviewCount,
            p.IsFeatured
        );
}
