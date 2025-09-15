using EventHorizon.Application.DTOs;
using EventHorizon.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Api.Controllers;

/// <summary>
/// Products management (alias for Policies)
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("products")] // Lowercase alias
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly PoliciesService _policiesService;

    public ProductsController(PoliciesService policiesService)
    {
        _policiesService = policiesService;
    }

    /// <summary>
    /// Get all products (admin)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PolicyDto>>> GetProducts()
    {
        var products = await _policiesService.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get public products (read-only subset)
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PolicyDto>>> GetPublicProducts()
    {
        var products = await _policiesService.GetAllAsync();
        // Return subset of fields for public consumption
        var publicProducts = products.Select(p => new
        {
            p.Id,
            p.ProductCode,
            p.ProductName
        });
        return Ok(publicProducts);
    }

    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<PolicyDto>> CreateProduct([FromBody] PolicyCreateDto dto)
    {
        var product = await _policiesService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PolicyDto>> GetProduct(Guid id)
    {
        var product = await _policiesService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    /// <summary>
    /// Update product
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "Administrator")]
    public async Task<ActionResult<PolicyDto>> UpdateProduct(Guid id, [FromBody] PolicyUpdateDto dto)
    {
        var product = await _policiesService.UpdateAsync(id, dto);
        if (product == null) return NotFound();
        return Ok(product);
    }

    /// <summary>
    /// Update product status (placeholder - policies don't have status field)
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> UpdateProductStatus(Guid id, [FromBody] object statusUpdate)
    {
        // For now, just return success since policies don't have a status field
        // This could be extended to add an IsActive field to Policy entity
        return NoContent();
    }

    /// <summary>
    /// Delete product
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var success = await _policiesService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}