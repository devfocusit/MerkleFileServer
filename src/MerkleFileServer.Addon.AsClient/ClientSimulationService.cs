using MerkleFileServer.Addon.AsClient.ApiClient;
using MerkleFileServer.Addon.AsClient.ApiClient.Contract;
using MerkleFileServer.Addon.AsClient.PeerWorker;
using System.Collections.Concurrent;

namespace MerkleFileServer.Addon.AsClient
{
    internal class ClientSimulationService : IClientSimulationService
    {
        private IMerkleFileServerApiClient _apiClient;
        private IPeerWorkerFactory _peerWorkerFactory;
        private readonly ClientOptions _clientOptions;

        public ClientSimulationService(IMerkleFileServerApiClient apiClient, IPeerWorkerFactory peerWorkerFactory, ClientOptions options)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _peerWorkerFactory = peerWorkerFactory ?? throw new ArgumentNullException(nameof(peerWorkerFactory));
            _clientOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<ClientDownloadResponse> Download(string? rootHash, string destinationFile)
        {
            var matchingHash = await IsAvailable(rootHash);

            if (matchingHash is null)
            {
                return null;
            }

            var results = await DownloadFromPeers(_clientOptions.Peers, matchingHash);

            var pieces = results.SelectMany(x => x.pieces).ToList();

            var orderedPiecesp = pieces.OrderBy(x => x.Key).Select(x => x.Value).ToList();
            var file = await WriteFile(destinationFile, orderedPiecesp);

            return new ClientDownloadResponse(
                file,
                results.SelectMany(x => x.Log).ToList()
            );
        }

        private async Task<IReadOnlyList<PeerWorkerResult>> DownloadFromPeers(IReadOnlyList<string> availablePeers, HashesViewModel matchingHash)
        {
            var queue = new ConcurrentQueue<int>();
            for (int i = 0; i < matchingHash.numberOfPieces; i++)
            {
                queue.Enqueue(i);
            }

            var peersConnections = new List<Task<PeerWorkerResult>>();
            foreach (var p in availablePeers)
            {
                var worker = _peerWorkerFactory.GetWorker();
                peersConnections.Add(worker.Start(matchingHash.rootHash, p, queue));
            }

            await Task.WhenAll(peersConnections);

            var results = peersConnections.Select(x => x.Result).ToList();

            var pieces = results.SelectMany(x => x.pieces).ToList();

            if (!queue.IsEmpty || pieces.Count != matchingHash.numberOfPieces)
            {
                throw new InvalidDataException("Not pieces have been downloaded");
            }

            return results;
        }

        private async Task<HashesViewModel?> IsAvailable(string? rootHash)
        {
            var hashes = await _apiClient.GetHashes(_clientOptions.TrustedServer);

            HashesViewModel matchingHash;
            if (rootHash is not null)
            {
                matchingHash = hashes.FirstOrDefault(x => x.rootHash == rootHash);
            }
            else
            {
                matchingHash = hashes.First();
            }

            return matchingHash;
        }

        private async Task<string> WriteFile(string fileName, IReadOnlyList<FilePieceViewModel> pieces)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    foreach (var p in pieces)
                    {
                        fs.Write(p.content.Span);
                    }

                    return fs.Name;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return null;
            }
        }
    }
}
