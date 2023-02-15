using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MerkleFileServer.Application.GetAvailableHashes
{
    internal class GetAvailableHashesService : IGetAvailableHashesService
    {
        private readonly IMerkleTreeRepository _merkleTreeMemoryStorage;

        public GetAvailableHashesService(IMerkleTreeRepository merkleTreeMemoryStorage)
        {
            _merkleTreeMemoryStorage = merkleTreeMemoryStorage ?? throw new ArgumentNullException(nameof(merkleTreeMemoryStorage));
        }

        public IReadOnlyList<HashedFile> GetHashes()
        {
            return _merkleTreeMemoryStorage.FetchAllHashedFiles().ToList().AsReadOnly();
        }
    }
}
