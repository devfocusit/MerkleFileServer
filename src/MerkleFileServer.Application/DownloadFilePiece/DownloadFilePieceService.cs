using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Domain.Models;
using MerkleFileServer.Domain.Merkle;
using System;

namespace MerkleFileServer.Application.DownloadFilePiece
{
    internal class DownloadFilePieceService : IDownloadFilePieceService
    {
        private readonly IMerkleTreeRepository _merkleTreeMemoryStorage;
        private readonly IFilePieceRepository _filePieceStorage;

        public DownloadFilePieceService(IMerkleTreeRepository merkleTreeMemoryStorage, IFilePieceRepository filePieceStorage)
        {
            _merkleTreeMemoryStorage = merkleTreeMemoryStorage ?? throw new ArgumentNullException(nameof(merkleTreeMemoryStorage));
            _filePieceStorage = filePieceStorage ?? throw new ArgumentNullException(nameof(filePieceStorage));
        }

        public FilePiece TryDownload(string rootHash, int pieceIndex)
        {
            var pieceProof = _merkleTreeMemoryStorage.FetchProof(rootHash, pieceIndex);

            if (pieceProof is null)
            {
                /* This service is used by clients/peers where only some of pieces might exists - then if not found just return 404 so that client can check in other seeds or wait 
                 */
                return null;
            }

            var pieceContent = _filePieceStorage.Fetch(pieceProof.FileId.FileName, pieceIndex);

            bool isValid = new MerkleProofsValidator(rootHash).Validate(pieceContent.Span, pieceProof.Hashes);
            if (!isValid)
            {
                throw new ArgumentException($"Fetched piece ({pieceIndex}) is not valid");
            }

            return new FilePiece(pieceProof.FileId, pieceIndex, pieceContent, pieceProof.Hashes);
        }
    }
}
