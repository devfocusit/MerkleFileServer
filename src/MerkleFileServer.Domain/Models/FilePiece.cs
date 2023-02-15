namespace MerkleFileServer.Domain.Models
{
    public record FilePiece(FileId fileId, int pieceIndex, Memory<byte> Content, IReadOnlyList<string> Proofs);
}
