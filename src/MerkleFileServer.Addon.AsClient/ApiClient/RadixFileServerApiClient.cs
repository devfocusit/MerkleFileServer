using MerkleFileServer.Addon.AsClient.ApiClient.Contract;

namespace MerkleFileServer.Addon.AsClient.ApiClient
{
    public class MerkleFileServerApiClient : IMerkleFileServerApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MerkleFileServerApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IReadOnlyList<HashesViewModel>> GetHashes(string host)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var uri = $"{host}/hashes";

            var result = await httpClient.Get<IReadOnlyList<HashesViewModel>>(new Uri(uri));

            return result;
        }

        public async Task<FilePieceViewModel?> TryDownloadPiece(string host, string hash, int pieceIndex)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var uri = $"{host}/piece/{hash}/{pieceIndex}";

            var result = await httpClient.Get<FilePieceViewModel?>(new Uri(uri));

            return result;
        }
    }
}
