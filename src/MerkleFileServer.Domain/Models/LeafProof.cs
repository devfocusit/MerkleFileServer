namespace MerkleFileServer.Domain.Models
{
    public record LeafProof(FileId FileId, int LeafIndex, IReadOnlyList<string> Hashes);
}
