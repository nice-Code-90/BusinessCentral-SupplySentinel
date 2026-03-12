using MediatR;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.UseCases.GetVendors;

public record GetVendorsQuery() : IRequest<Result<List<Vendor>>>;
