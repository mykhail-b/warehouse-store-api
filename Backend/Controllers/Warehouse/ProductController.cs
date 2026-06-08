using Backend.Services.Warehouse;
using ClassLibrary.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Warehouse;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    { 
         return  Ok(await _service.GetAllAsync());
    }
   

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product item)
    { 
         return  Ok(await _service.CreateAsync(item));
    }


    [HttpPut]
    public async Task<IActionResult> Update(Product item)
    { 
        return  Ok(await _service.UpdateAsync(item));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    { 
        return  Ok(await _service.DeleteItemAsync(id));
    }
    
}