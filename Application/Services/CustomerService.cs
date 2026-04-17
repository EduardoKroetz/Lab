using AutoMapper;
using Lab.Api.Application.DTOs.Customers;
using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Exceptions;
using Lab.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Application.Services;

public class CustomerService
{
    private readonly LabDbContext _dbContext;
    private readonly IMapper _mapper;

    public CustomerService(LabDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<GetCustomerDto>> GetListAsync()
    {
        var customers = await _dbContext.Customers.AsNoTracking().ToListAsync();

        return customers.Select(c => _mapper.Map<GetCustomerDto>(c)).ToList();
    }

    public async Task<GetCustomerDto> GetByIdAsync(Guid id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        return _mapper.Map<GetCustomerDto>(customer);
    }

    public async Task<GetCustomerDto> CreateAsync(UpsertCustomerDto dto)
    {
        await ValidateAsync(dto);

        var customer = new Customer(dto.Name, dto.CpfCnpj, dto.Email, dto.PhoneNumber);

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<GetCustomerDto>(customer);
    }

    public async Task<GetCustomerDto> UpdateAsync(Guid id, UpsertCustomerDto dto)
    {
        await ValidateAsync(dto, id);

        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        customer.Update(dto.Name, dto.CpfCnpj, dto.Email, dto.PhoneNumber);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<GetCustomerDto>(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            throw new NotFoundException("Cliente não encontrado.");

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync();
    }

    private async Task ValidateAsync(UpsertCustomerDto dto, Guid? id = null)
    {
        if (!string.IsNullOrWhiteSpace(dto.Email) && await _dbContext.Customers.AnyAsync(c => c.Email == dto.Email && c.Id != id))
            throw new BadRequestException("Já existe um cliente com este e-mail.");

        if (!string.IsNullOrWhiteSpace(dto.CpfCnpj) && await _dbContext.Customers.AnyAsync(c => c.CpfCnpj == dto.CpfCnpj && c.Id != id))
            throw new BadRequestException("Já existe um cliente com este CPF/CNPJ.");
    }
}
