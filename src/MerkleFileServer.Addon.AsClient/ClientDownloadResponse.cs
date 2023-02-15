namespace MerkleFileServer.Addon.AsClient
{
    public record ClientDownloadResponse(string DestinationFileName, IReadOnlyList<string> Log);
}
