using MerkleFileServer.Addon.AsClient.ApiClient.Contract;
using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Domain.Merkle;
using MerkleFileServer.Domain.Models;
using System.Collections.Concurrent;

namespace MerkleFileServer.Addon.AsClient.PeerWorker
{
    internal class PeerWorker : IPeerWorker
    {
        private IMerkleFileServerApiClient _apiClient;
        private readonly IMerkleTreeRepository _merkleTreeMemoryStorage;
        private readonly IFilePieceRepository _filePieceStorage;

        public PeerWorker(IMerkleFileServerApiClient apiClient, IMerkleTreeRepository merkleTreeMemoryStorage, IFilePieceRepository filePieceStorage)
        {
            _merkleTreeMemoryStorage = merkleTreeMemoryStorage ?? throw new ArgumentNullException(nameof(merkleTreeMemoryStorage));
            _filePieceStorage = filePieceStorage ?? throw new ArgumentNullException(nameof(filePieceStorage));
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(_apiClient));
        }

        public async Task<PeerWorkerResult> Start(string rootHash, string peer, ConcurrentQueue<int> queue)
        {
            var results = new Dictionary<int, FilePieceViewModel>();
            var log = new List<string>();

            var notFoundIndexes = new List<int>();

            while (queue.TryDequeue(out int index))
            {
                if (notFoundIndexes.Contains(index))
                {
                    /* This could lead to infinitive loop but the assumption is that if piece is not found it shall be available in other peers 
                     * so that index will be dequeued by next peerWorker who shall have it
                     * or otherwise client waits until it appears in one of peers
                     */

                    //ToDo: add circut breaker
                    continue;
                }

                log.Add($"Downloading piece {index} from peer {peer}");

                await Task.Delay(200); // Simulate some delay so that other workers have chance to chip in

                var piece = await _apiClient.TryDownloadPiece(peer, rootHash, index);

                if (piece is null)
                {
                    notFoundIndexes.Add(index);

                    queue.Enqueue(index);

                    continue;
                }

                //Validate piece with proofs
                var validator = new MerkleProofsValidator(rootHash);

                if (!validator.Validate(piece.content.Span, piece.proofs))
                {
                    throw new InvalidDataException();
                }

                /* Store piece and proofs in the same way like original LoadFileHashService is doing that
                 * so that it can be found the same way also if this instance is serving as seed
                 */
                _filePieceStorage.Save(piece.fileName, index, piece.content);

                _merkleTreeMemoryStorage.AddProof(rootHash, new LeafProof(new FileId(piece.fileName), index, piece.proofs));

                results.Add(index, piece);
            }

            return new PeerWorkerResult(results, log);
        }
    }

    public record PeerWorkerResult(Dictionary<int, FilePieceViewModel> pieces, IList<string> Log);
}
