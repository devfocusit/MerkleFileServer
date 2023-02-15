using MerkleFileServer.Domain.Models;

namespace MerkleFileServer.Domain.Abstractions
{
    public interface IMerkleTreeRepository
    {
        IList<HashedFile> FetchAllHashedFiles();
        LeafProof FetchProof(string rootHash, int leafIndex);
        void AddNewFile(HashedFile hashedFile);
        void AddProof(string rootHash, LeafProof proof);
    }
}
