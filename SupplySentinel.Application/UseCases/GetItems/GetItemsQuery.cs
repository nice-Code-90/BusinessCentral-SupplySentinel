using MediatR;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.UseCases.GetItems;

public record GetItemsQuery() : IRequest<Result<List<Item>>>;