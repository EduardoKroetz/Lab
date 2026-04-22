using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Customers;
using Lab.Domain.Common.Models;
using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Services;

public class CustomerService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CustomerService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetCustomerResponse>>> GetListAsync()
    {
        var customers = await _dbContext.Customers
            .AsNoTracking()
            .ToListAsync();

        var responses = customers.Select(c => _mapper.Map<GetCustomerResponse>(c)).ToList();

        return Result<List<GetCustomerResponse>>.Success(responses);
    }

    public async Task<Result<GetCustomerResponse>> GetByIdAsync(Guid id)
    {
        var customer = await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
            return Result<GetCustomerResponse>.Failure("Cliente não encontrado.");

        return Result<GetCustomerResponse>.Success(_mapper.Map<GetCustomerResponse>(customer));
    }

    public async Task<Result<GetCustomerResponse>> CreateAsync(UpsertCustomerRequest request)
    {
        var validationResult = await ValidateAsync(request);
        if (!validationResult.Succeeded)
            return Result<GetCustomerResponse>.Failure(validationResult.Errors);

        var customer = new Customer(request.Name, request.CpfCnpj, request.Email, request.PhoneNumber);

        await _dbContext.Customers.AddAsync(customer);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(customer.Id);
    }

    public async Task<Result<GetCustomerResponse>> UpdateAsync(Guid id, UpsertCustomerRequest request)
    {
        var validationResult = await ValidateAsync(request, id);
        if (!validationResult.Succeeded)
            return Result<GetCustomerResponse>.Failure(validationResult.Errors);

        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            return Result<GetCustomerResponse>.Failure("Cliente não encontrado.");

        customer.Update(request.Name, request.CpfCnpj, request.Email, request.PhoneNumber);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);
        if (customer == null)
            return Result.Failure("Cliente não encontrado.");

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<Result> ValidateAsync(UpsertCustomerRequest request, Guid? id = null)
    {
        if (!string.IsNullOrWhiteSpace(request.Email) &&
            await _dbContext.Customers.AnyAsync(c => c.Email == request.Email && c.Id != id))
        {
            return Result.Failure("Já existe um cliente com este e-mail.");
        }

        if (!string.IsNullOrWhiteSpace(request.CpfCnpj) &&
            await _dbContext.Customers.AnyAsync(c => c.CpfCnpj == request.CpfCnpj && c.Id != id))
        {
            return Result.Failure("Já existe um cliente com este CPF/CNPJ.");
        }

        return Result.Success();
    }
}
