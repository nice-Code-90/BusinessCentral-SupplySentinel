using MediatR;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Application.UseCases.ResolvePriceConflict;

public class ResolvePriceConflictCommandHandler : IRequestHandler<ResolvePriceConflictCommand, Result>
{
    private readonly IBCDataSyncTool _dataSyncTool;

    public ResolvePriceConflictCommandHandler(IBCDataSyncTool dataSyncTool)
    {
        _dataSyncTool = dataSyncTool;
    }

    public async Task<Result> Handle(ResolvePriceConflictCommand request, CancellationToken cancellationToken)
    {
        return await _dataSyncTool.UpdateItemPriceAsync(request.VendorId, request.ItemId, request.NewPrice, cancellationToken);
    }
}
