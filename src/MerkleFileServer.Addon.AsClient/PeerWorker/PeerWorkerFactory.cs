using MerkleFileServer.Addon.AsClient.ApiClient.Contract;
using MerkleFileServer.Domain.Abstractions;

namespace MerkleFileServer.Addon.AsClient.PeerWorker
{
    internal interface IPeerWorkerFactory
    {
        IPeerWorker GetWorker();
    }

    internal class PeerWorkerFactory : IPeerWorkerFactory
    {
        private IMerkleFileServerApiClient _apiClient;
        private readonly IMerkleTreeRepository _merkleTreeMemoryStorage;
        private readonly IFilePieceRepository _filePieceStorage;

        public PeerWorkerFactory(IMerkleFileServerApiClient apiClient, IMerkleTreeRepository merkleTreeMemoryStorage, IFilePieceRepository filePieceStorage)
        {
            _merkleTreeMemoryStorage = merkleTreeMemoryStorage ?? throw new ArgumentNullException(nameof(merkleTreeMemoryStorage));
            _filePieceStorage = filePieceStorage ?? throw new ArgumentNullException(nameof(filePieceStorage));
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(_apiClient));
        }

        public IPeerWorker GetWorker()
        {
            return new PeerWorker(_apiClient, _merkleTreeMemoryStorage, _filePieceStorage);
        }
    }
}
