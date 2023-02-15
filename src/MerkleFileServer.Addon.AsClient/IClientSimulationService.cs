namespace MerkleFileServer.Addon.AsClient
{
    public interface IClientSimulationService
    {
        Task<ClientDownloadResponse> Download(string? rootHash, string destinationFile);
    }
}
