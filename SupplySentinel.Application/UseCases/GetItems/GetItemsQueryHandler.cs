using MediatR;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.UseCases.GetItems;

public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, Result<List<Item>>>
{
    private readonly IERPComparisonTool _erpTool;

    public GetItemsQueryHandler(IERPComparisonTool erpTool)
    {
        _erpTool = erpTool;
    }

    public async Task<Result<List<Item>>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        return await _erpTool.GetItemsAsync(cancellationToken);
    }
}