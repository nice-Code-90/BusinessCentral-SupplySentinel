using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplySentinel.Application.UseCases.Documents;
using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ISender _sender;

    public DocumentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        var command = new UploadDocumentCommand(file.FileName, stream.ToArray(), file.ContentType);
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(new { DocumentId = result.Value, Message = "File uploaded successfully. Use this ID to analyze." });
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeDocument([FromBody] AnalyzeSupplierDocumentCommand command)
    {
        Result result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(new { Message = "Document analysis started", DocumentId = command.DocumentId });
    }
}