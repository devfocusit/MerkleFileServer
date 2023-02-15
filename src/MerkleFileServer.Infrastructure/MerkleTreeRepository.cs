using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Domain.Models;
using MerkleFileServer.Infrastructure.Storage;

namespace MerkleFileServer.Infrastructure
{
    internal class MerkleTreeRepository : IMerkleTreeRepository
    {
        private readonly IStorage _storage;

        private const string ALL_HASHES_CACHE_KEY = "ALL_HASHES_CACHE_KEY";
        private const string PROOFS = "PROOF";

        public MerkleTreeRepository(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public IList<HashedFile> FetchAllHashedFiles()
        {
            var item = _storage.Read(ALL_HASHES_CACHE_KEY);

            if (item is null)
            {
                return new List<HashedFile>();
            }

            return (IList<HashedFile>)item;
        }

        public void AddNewFile(HashedFile hashedFile)
        {
            var list = (IList<HashedFile>)_storage.Read(ALL_HASHES_CACHE_KEY);
            if (list is null)
            {
                list = new List<HashedFile>();
            }

            list.Add(hashedFile);

            _storage.Store(ALL_HASHES_CACHE_KEY, list);
        }

        public LeafProof FetchProof(string rootHash, int leafIndex)
        {
            var item = _storage.Read(GetProofKey(rootHash, leafIndex));

            if (item is null)
            {
                throw new KeyNotFoundException();

            }
            return (LeafProof)item;
        }

        public void AddProof(string rootHash, LeafProof proof)
        {
            _storage.Store(GetProofKey(rootHash, proof.LeafIndex), proof);
        }

        private string GetProofKey(string rootHash, int leafIndex) => $"{PROOFS}-{rootHash}-{leafIndex}";
    }
}
