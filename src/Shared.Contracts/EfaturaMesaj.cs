namespace Efatura.Shared.Contracts;

public enum EfaturaOlayTipi
{
    InvoiceCreated,
    EnvelopePackaged,
    EnvelopeSent,
    EnvelopeStatus
}

public sealed record EfaturaMesaj
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public Guid EnvelopeId { get; init; }
    public EfaturaOlayTipi OlayTipi { get; init; }
    public string Vkn { get; init; } = string.Empty;
    public int FaturaSayisi { get; init; }
    public string PayloadSha256 { get; init; } = string.Empty;
    public DateTimeOffset OlusturmaZamani { get; init; } = DateTimeOffset.UtcNow;
}
