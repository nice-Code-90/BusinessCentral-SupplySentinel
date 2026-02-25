using SupplySentinel.Domain.Enums;

namespace SupplySentinel.Domain.Entities;

public class SupplyDocument
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public byte[] RawContent { get; private set; } = Array.Empty<byte>();
    public string Source { get; private set; } = string.Empty; // pl. "Email", "Upload"
    public SyncStatus ProcessingStatus { get; private set; }
    public DateTime ReceivedAt { get; private set; }

    public SupplyDocument(Guid id, string fileName, byte[] rawContent, string source)
    {
        Id = id;
        FileName = fileName;
        RawContent = rawContent;
        Source = source;
        ProcessingStatus = SyncStatus.Pending;
        ReceivedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(SyncStatus status) => ProcessingStatus = status;
}