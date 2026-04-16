using Lab.Api.Application.DTOs.Customers;
using Lab.Api.DTOs;
using Lab.Api.Entities;
using Lab.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly LabDbContext _dbContext;

    public CustomersController(LabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        var customers = await _dbContext.Customers.AsNoTracking().ToListAsync();

        var dto = customers.Select(c => new GetCustomerDto(c.Id, c.Name, c.CpfCnpj, c.Email, c.PhoneNumber)).ToList();

        return Ok(new ResponseDto(dto));
    }

    [HttpGet("{id:guid}", Name = nameof(GetCustomerByIdAsync))]
    public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            return NotFound(new ResponseDto("Cliente não encontrado."));

        var dto = new GetCustomerDto(customer.Id, customer.Name, customer.CpfCnpj, customer.Email, customer.PhoneNumber);

        return Ok(new ResponseDto(dto));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UpsertCustomerDto dto)
    {
        if (await _dbContext.Customers.AnyAsync(c => c.Email == dto.Email))
            return BadRequest(new ResponseDto("Já possui um cliente com este email."));

        if (await _dbContext.Customers.AnyAsync(c => c.CpfCnpj == dto.CpfCnpj))
            return BadRequest(new ResponseDto("Já possui um cliente com este CPF/CNPJ."));

        var customer = new Customer
        {  
            Name = dto.Name,
            CpfCnpj = dto.CpfCnpj,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        return CreatedAtRoute(nameof(GetCustomerByIdAsync), new { id = customer.Id }, new ResponseDto(customer));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertCustomerDto dto)
    {
        if (await _dbContext.Customers.AnyAsync(c => c.Email == dto.Email && id != c.Id))
            return BadRequest(new ResponseDto("Já possui um cliente com este e-mail."));

        if (await _dbContext.Customers.AnyAsync(c => c.CpfCnpj == dto.CpfCnpj && id != c.Id))
            return BadRequest(new ResponseDto("Já possui um cliente com este CPF/CNPJ."));

        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            return NotFound(new ResponseDto("Cliente não encontrado."));

        customer.Name = dto.Name;
        customer.CpfCnpj = dto.CpfCnpj;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;

        await _dbContext.SaveChangesAsync();

        return Ok(new ResponseDto(customer));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            return NotFound(new ResponseDto("Cliente não encontrado."));

        _dbContext.Customers.Remove(customer);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
