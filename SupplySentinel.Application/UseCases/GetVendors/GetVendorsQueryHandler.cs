using MediatR;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.UseCases.GetVendors;

public class GetVendorsQueryHandler : IRequestHandler<GetVendorsQuery, Result<List<Vendor>>>
{
    private readonly IERPComparisonTool _erpComparisonTool;

    public GetVendorsQueryHandler(IERPComparisonTool erpComparisonTool)
    {
        _erpComparisonTool = erpComparisonTool;
    }

    public async Task<Result<List<Vendor>>> Handle(GetVendorsQuery request, CancellationToken cancellationToken)
    {
        return await _erpComparisonTool.GetVendorsAsync(cancellationToken);
    }
}
