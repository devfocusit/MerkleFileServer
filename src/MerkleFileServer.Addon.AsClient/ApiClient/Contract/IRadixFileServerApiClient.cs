namespace MerkleFileServer.Addon.AsClient.ApiClient.Contract
{
    public interface IMerkleFileServerApiClient
    {
        Task<IReadOnlyList<HashesViewModel>> GetHashes(string host);
        Task<FilePieceViewModel?> TryDownloadPiece(string host, string hash, int pieceIndex);
    }
}
