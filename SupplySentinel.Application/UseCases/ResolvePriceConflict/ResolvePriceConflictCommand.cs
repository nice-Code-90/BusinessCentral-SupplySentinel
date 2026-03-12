using MediatR;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Application.UseCases.ResolvePriceConflict;

public record ResolvePriceConflictCommand(Guid ItemId, Money NewPrice, Guid VendorId) : IRequest<Result>;
