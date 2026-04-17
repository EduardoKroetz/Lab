using Lab.Api.Application.DTOs;
using Lab.Api.Application.DTOs.Customers;
using Lab.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var dto = await _customerService.GetListAsync();

        return Ok(new ResponseDto(dto));
    }

    [HttpGet("{id:guid}", Name = nameof(GetCustomerByIdAsync))]
    public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
    {
        var dto = await _customerService.GetByIdAsync(id);

        return Ok(new ResponseDto(dto));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertCustomerDto dto)
    {
        var responseDto = await _customerService.CreateAsync(dto);

        return CreatedAtRoute(nameof(GetCustomerByIdAsync), new { id = responseDto.Id }, new ResponseDto(responseDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertCustomerDto dto)
    {
        var responseDto = await _customerService.UpdateAsync(id, dto);

        return Ok(new ResponseDto(responseDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _customerService.DeleteAsync(id);

        return NoContent();
    }
}
