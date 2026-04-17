using Lab.Api.Application.DTOs.Customers;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Application.Services;

public class CustomerService
{
    private readonly LabDbContext _dbContext;

    public CustomerService(LabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetCustomerDto>> GetListAsync()
    {
        var customers = await _dbContext.Customers.AsNoTracking().ToListAsync();

        return customers
            .Select(c => new GetCustomerDto(c.Id, c.Name, c.CpfCnpj, c.Email, c.PhoneNumber))
            .ToList();
    }

    public async Task<GetCustomerDto> GetByIdAsync(Guid id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        return new GetCustomerDto(customer.Id, customer.Name, customer.CpfCnpj, customer.Email, customer.PhoneNumber);
    }

    public async Task<GetCustomerDto> CreateAsync(UpsertCustomerDto dto)
    {
        if (await _dbContext.Customers.AnyAsync(c => c.Email == dto.Email))
            throw new BadRequestException("Já possui um cliente com este email.");

        if (await _dbContext.Customers.AnyAsync(c => c.CpfCnpj == dto.CpfCnpj))
            throw new BadRequestException("Já possui um cliente com este CPF/CNPJ.");

        var customer = new Customer
        {
            Name = dto.Name,
            CpfCnpj = dto.CpfCnpj,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        return new GetCustomerDto(customer.Id, customer.Name, customer.CpfCnpj, customer.Email, customer.PhoneNumber);
    }

    public async Task<GetCustomerDto> UpdateAsync(Guid id, UpsertCustomerDto dto)
    {
        if (await _dbContext.Customers.AnyAsync(c => c.Email == dto.Email && c.Id != id))
            throw new BadRequestException("Já possui um cliente com este e-mail.");

        if (await _dbContext.Customers.AnyAsync(c => c.CpfCnpj == dto.CpfCnpj && c.Id != id))
            throw new BadRequestException("Já possui um cliente com este CPF/CNPJ.");

        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        customer.Name = dto.Name;
        customer.CpfCnpj = dto.CpfCnpj;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;

        await _dbContext.SaveChangesAsync();

        return new GetCustomerDto(customer.Id, customer.Name, customer.CpfCnpj, customer.Email, customer.PhoneNumber);
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync();
    }
}
